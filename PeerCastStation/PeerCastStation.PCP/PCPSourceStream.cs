﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PeerCastStation.Core;
using System.Text.RegularExpressions;

namespace PeerCastStation.PCP
{
  public class PCPSourceStreamFactory
    : ISourceStreamFactory
  {
    private PeerCast peercast;
    public PCPSourceStreamFactory(PeerCast peercast)
    {
      this.peercast = peercast;
    }

    public string Name { get { return "pcp"; } }
    public ISourceStream Create(Channel channel, Uri tracker)
    {
      return new PCPSourceStream(peercast, channel, tracker);
    }
  }

  public class RelayRequestResponse
  {
    public int StatusCode     { get; set; }
    public int? PCPVersion    { get; set; }
    public string ContentType { get; set; }
    public long? StreamPos    { get; set; }
    public RelayRequestResponse(IEnumerable<string> responses)
    {
      this.PCPVersion = null;
      this.ContentType = null;
      this.StreamPos = null;
      foreach (var res in responses) {
        Match match = null;
        if ((match = Regex.Match(res, @"^HTTP/1.\d (\d+) .*$")).Success) {
          this.StatusCode = Convert.ToInt32(match.Groups[1].Value);
        }
        if ((match = Regex.Match(res, @"Content-Type:\s*(\S+)\s*$")).Success) {
          this.ContentType = match.Groups[1].Value;
        }
        if ((match = Regex.Match(res, @"x-peercast-pcp:\s*(\d+)\s*$")).Success) {
          this.PCPVersion = Convert.ToInt32(match.Groups[1].Value);
        }
        if ((match = Regex.Match(res, @"x-peercast-pos:\s*(\d+)\s*$")).Success) {
          this.StreamPos = Convert.ToInt64(match.Groups[1].Value);
        }
      }
    }
  }

  public static class RelayRequestResponseReader
  {
    public static RelayRequestResponse Read(Stream stream)
    {
      string line = null;
      var responses = new List<string>();
      var buf = new List<byte>();
      while (line!="") {
        var value = stream.ReadByte();
        if (value<0) {
          throw new EndOfStreamException();
        }
        buf.Add((byte)value);
        if (buf.Count >= 2 && buf[buf.Count - 2] == '\r' && buf[buf.Count - 1] == '\n') {
          line = System.Text.Encoding.UTF8.GetString(buf.ToArray(), 0, buf.Count - 2);
          if (line!="") responses.Add(line);
          buf.Clear();
        }
      }
      return new RelayRequestResponse(responses);
    }
  }

  public interface IStreamState
  {
    IStreamState Process();
  }

  public class PCPSourceConnectState : IStreamState
  {
    public PCPSourceStream Owner { get; private set; }
    public Host Host { get; private set; }

    public PCPSourceConnectState(PCPSourceStream owner, Host host)
    {
      Owner = owner;
      Host = host;
    }

    public IStreamState Process()
    {
      if (Host!=null) {
        Owner.Status = SourceStreamStatus.Connecting;
        if (Owner.Connect(Host)) {
          return new PCPSourceRelayRequestState(Owner);
        }
        else {
          return new PCPSourceClosedState(Owner, CloseReason.ConnectionError);
        }
      }
      else {
        return new PCPSourceClosedState(Owner, CloseReason.NodeNotFound);
      }
    }
  }

  public class PCPSourceRelayRequestState : IStreamState
  {
    public PCPSourceStream Owner { get; private set; }
    public PCPSourceRelayRequestState(PCPSourceStream owner)
    {
      Owner = owner;
    }

    public IStreamState Process()
    {
      Owner.SendRelayRequest();
      return new PCPSourceRecvRelayResponseState(Owner);
    }
  }

  public class PCPSourceRecvRelayResponseState : IStreamState
  {
    public PCPSourceStream Owner { get; private set; }
    public PCPSourceRecvRelayResponseState(PCPSourceStream owner)
    {
      Owner = owner;
    }

    public IStreamState Process()
    {
      RelayRequestResponse res = Owner.RecvRelayRequestResponse();
      if (res!=null) {
        if (res.StatusCode==200 || res.StatusCode==503) {
          return new PCPSourcePCPHandshakeState(Owner);
        }
        else if (res.StatusCode==404) {
          return new PCPSourceClosedState(Owner, CloseReason.ChannelNotFound);
        }
        else {
          return new PCPSourceClosedState(Owner, CloseReason.AccessDenied);
        }
      }
      else {
        return this;
      }
    }
  }

  public class PCPSourcePCPHandshakeState : IStreamState
  {
    public PCPSourceStream Owner { get; private set; }
    private bool sentHelo = false;
    public PCPSourcePCPHandshakeState(PCPSourceStream owner)
    {
      Owner = owner;
    }

    public IStreamState Process()
    {
      if (!sentHelo) {
        Owner.SendPCPHelo();
        sentHelo = true;
      }
      Atom atom = Owner.RecvAtom();
      if (atom!=null && (atom.Name==Atom.PCP_OLEH || atom.Name==Atom.PCP_QUIT)) {
        var state = Owner.ProcessAtom(atom);
        if (state!=null) {
          return state;
        }
        else if (atom.Name==Atom.PCP_OLEH) {
          return new PCPSourceReceivingState(Owner);
        }
        else {
          return this;
        }
      }
      else {
        return this;
      }
    }
  }

  public class PCPSourceReceivingState : IStreamState
  {
    public PCPSourceStream Owner { get; private set; }
    public int LastHostInfoUpdated { get; set; }
    public PCPSourceReceivingState(PCPSourceStream owner)
    {
      Owner = owner;
      LastHostInfoUpdated = 0;
    }

    public IStreamState Process()
    {
      Owner.Status = SourceStreamStatus.Recieving;
      if ((Environment.TickCount-LastHostInfoUpdated>=10000 && Owner.IsHostInfoUpdated) ||
           Environment.TickCount-LastHostInfoUpdated>=120000) {
        Owner.BroadcastHostInfo();
        LastHostInfoUpdated = Environment.TickCount;
      }
      Atom atom = Owner.RecvAtom();
      if (atom!=null) {
        var state = Owner.ProcessAtom(atom);
        if (state!=null) {
          return state;
        }
        else {
          return this;
        }
      }
      else {
        return this;
      }
    }
  }

  public enum CloseReason {
    ConnectionError,
    Unavailable,
    AccessDenied,
    ChannelExit,
    ChannelNotFound,
    RetryLimit,
    NodeNotFound,
    UserShutdown,
  }
  public class PCPSourceClosedState : IStreamState
  {
    public PCPSourceStream Owner { get; private set; }
    public CloseReason CloseReason { get; private set; }
    public PCPSourceClosedState(PCPSourceStream owner, CloseReason reason)
    {
      Owner = owner;
      CloseReason = reason;
    }

    public IStreamState Process()
    {
      IStreamState res = null;
      switch (CloseReason) {
      case CloseReason.UserShutdown:
        Owner.Status = SourceStreamStatus.Idle;
        res = null;
        break;
      case CloseReason.NodeNotFound:
        Owner.Status = SourceStreamStatus.Error;
        res = null;
        break;
      case CloseReason.Unavailable:
        Owner.IgnoreHost(Owner.Uphost);
        Owner.Status = SourceStreamStatus.Searching;
        res = new PCPSourceConnectState(Owner, Owner.SelectSourceHost());
        break;
      case CloseReason.ChannelExit:
      case CloseReason.ConnectionError:
      case CloseReason.AccessDenied:
      case CloseReason.ChannelNotFound:
        if (Owner.Uphost==null || Owner.Uphost.Equals(Owner.Channel.SourceHost)) {
          Owner.Status = SourceStreamStatus.Error;
          res = null;
        }
        else {
          Owner.Status = SourceStreamStatus.Searching;
          Owner.IgnoreHost(Owner.Uphost);
          res = new PCPSourceConnectState(Owner, Owner.SelectSourceHost());
        }
        break;
      }
      Owner.Close(CloseReason);
      return res;
    }
  }

  public class PCPSourceStream : ISourceStream
  {
    static private Logger logger = new Logger(typeof(PCPSourceStream));
    private PeerCast peercast;
    private Channel channel;
    private IStreamState state = null;

    private TcpClient connection = null;
    private NetworkStream stream = null;
    private Host uphost = null;
    private QueuedSynchronizationContext syncContext;
    private bool hostInfoUpdated = true;
    private System.Threading.AutoResetEvent changedEvent = new System.Threading.AutoResetEvent(true);

    public const int PCP_VERSION    = 1218;
    public const int PCP_VERSION_VP = 27;

    public IStreamState State { get { return state; } set { state = value; } }
    public PeerCast PeerCast { get { return peercast; } }
    public Channel Channel { get { return channel; } set { channel = value; } }
    public Host Uphost { get { return uphost; } set { uphost = value; } }
    public bool IsConnected { get { return connection!=null; } }
    public bool IsHostInfoUpdated { get { return hostInfoUpdated; } set { hostInfoUpdated = value; } }
    public event EventHandler<SourceStreamStatusChangedEventArgs> StatusChanged;
    private SourceStreamStatus status;
    public SourceStreamStatus Status {
      get {
        return status;
      }
      set {
        if (status!=value) {
          logger.Debug("ChannelStatus Changed: {0}", value);
          status = value;
          PeerCast.SynchronizationContext.Post(dummy => {
            if (StatusChanged!=null) {
              StatusChanged(this, new SourceStreamStatusChangedEventArgs(value));
            }
          }, null);
        }
      }
    } 

    MemoryStream recvStream = new MemoryStream();
    byte[] recvBuffer = new byte[8192];
    private void StartReceive()
    {
      if (stream != null) {
        try {
          stream.BeginRead(recvBuffer, 0, recvBuffer.Length, (ar) => {
            NetworkStream s = (NetworkStream)ar.AsyncState;
            try {
              int bytes = s.EndRead(ar);
              if (bytes > 0) {
                changedEvent.Set();
                syncContext.Post(x => {
                  recvStream.Seek(0, SeekOrigin.End);
                  recvStream.Write(recvBuffer, 0, bytes);
                  recvStream.Seek(0, SeekOrigin.Begin);
                  StartReceive();
                }, null);
              }
            }
            catch (ObjectDisposedException) { }
            catch (IOException e) {
              logger.Error(e);
              syncContext.Post(dummy => {
                Close(CloseReason.ConnectionError);
              }, null);
            }
          }, stream);
        }
        catch (ObjectDisposedException) { }
        catch (IOException e) {
          logger.Error(e);
          Close(CloseReason.ConnectionError);
        }
      }
    }

    MemoryStream sendStream = new MemoryStream(8192);
    IAsyncResult sendResult = null;
    private void ProcessSend()
    {
      if (sendResult!=null && sendResult.IsCompleted) {
        try {
          stream.EndWrite(sendResult);
        }
        catch (ObjectDisposedException) {}
        catch (IOException e) {
          logger.Error(e);
          Close(CloseReason.ConnectionError);
        }
        sendResult = null;
      }
      if (stream!=null && sendResult==null && sendStream.Length>0) {
        var buf = sendStream.ToArray();
        sendStream.SetLength(0);
        sendStream.Position = 0;
        try {
          sendResult = stream.BeginWrite(buf, 0, buf.Length, (ar) => {
            changedEvent.Set();
          }, null);
        }
        catch (ObjectDisposedException) {}
        catch (IOException e) {
          logger.Error(e);
          Close(CloseReason.ConnectionError);
        }
      }
    }

    public virtual void Send(byte[] bytes)
    {
      sendStream.Write(bytes, 0, bytes.Length);
    }

    public virtual void Send(Atom atom)
    {
      AtomWriter.Write(sendStream, atom);
    }

    static private MemoryStream dropStream(MemoryStream s)
    {
      var res = new MemoryStream((int)Math.Max(8192, s.Length - s.Position));
      res.Write(s.GetBuffer(), (int)s.Position, (int)(s.Length - s.Position));
      res.Position = 0;
      return res;
    }

    public void ProcessEvents()
    {
      if (syncContext!=null) {
        syncContext.ProcessAll();
      }
      changedEvent.WaitOne(1);
    }

    public virtual Host SelectSourceHost()
    {
      Host res = null;
      peercast.SynchronizationContext.Send(dummy => {
        res = channel.SelectSourceHost();
      }, null);
      if (res!=null) {
        logger.Debug("{0} is selected as source", res.GlobalEndPoint);
        return res;
      }
      else {
        logger.Debug("No selectable host");
        return null;
      }
    }

    public virtual bool Connect(Host host)
    {
      if (host!=null && host.GlobalEndPoint!=null) {
        connection = new TcpClient();
        IPEndPoint point;
        if (peercast.GlobalAddress!=null &&
            peercast.GlobalAddress.Equals(host.GlobalEndPoint.Address) &&
            host.LocalEndPoint!=null) {
          point = host.LocalEndPoint;
        }
        else {
          point = host.GlobalEndPoint;
        }
        try {
          connection.Connect(point);
          stream = connection.GetStream();
          sendStream.SetLength(0);
          sendStream.Position = 0;
          recvStream.SetLength(0);
          recvStream.Position = 0;
          uphost = host;
          StartReceive();
          logger.Debug("Connected: {0}", point);
          return true;
        }
        catch (SocketException e) {
          logger.Debug("Connection Failed: {0}", point);
          logger.Debug(e);
          connection.Close();
          connection = null;
          if (stream!=null) {
            if (sendResult!=null) {
              try {
                stream.EndWrite(sendResult);
              }
              catch (ObjectDisposedException) {}
              catch (IOException) {}
              sendResult = null;
            }
            stream.Close();
          }
          stream = null;
          sendStream.SetLength(0);
          sendStream.Position = 0;
          recvStream.SetLength(0);
          recvStream.Position = 0;
          return false;
        }
      }
      else {
        return false;
      }
    }

    public virtual void IgnoreHost(Host host)
    {
      peercast.SynchronizationContext.Send(dummy => {
        if (host!=null) {
          logger.Debug("Host {0}({1}) is ignored", host.GlobalEndPoint, host.SessionID.ToString("N"));
        }
        channel.IgnoreHost(host);
      }, null);
    }

    public virtual void Close(CloseReason reason)
    {
      if (connection != null) {
        logger.Debug("Closed by {0}", reason);
        if (stream!=null) {
          if (sendResult!=null) {
            try {
              stream.EndWrite(sendResult);
            }
            catch (ObjectDisposedException) {}
            catch (IOException) {}
            sendResult = null;
          }
          stream.Close();
        }
        connection.Close();
        stream = null;
        connection = null;
        sendStream.SetLength(0);
        sendStream.Position = 0;
        recvStream.SetLength(0);
        recvStream.Position = 0;
      }
    }

    public virtual void SendRelayRequest()
    {
      logger.Debug("Sending Relay request: /channel/{0}", channel.ChannelInfo.ChannelID.ToString("N"));
      var req = String.Format(
        "GET /channel/{0} HTTP/1.0\r\n" +
        "x-peercast-pcp:1\r\n" +
        "\r\n", channel.ChannelInfo.ChannelID.ToString("N"));
      Send(System.Text.Encoding.UTF8.GetBytes(req));
    }

    public bool Recv(Action<Stream> proc)
    {
      bool res = false;
      recvStream.Seek(0, SeekOrigin.Begin);
      try {
        proc(recvStream);
        recvStream = dropStream(recvStream);
        res = true;
      }
      catch (EndOfStreamException) {
      }
      return res;
    }

    public virtual RelayRequestResponse RecvRelayRequestResponse()
    {
      RelayRequestResponse response = null;
      if (Recv(s => { response = RelayRequestResponseReader.Read(s); })) {
        logger.Debug("Relay response: {0}", response.StatusCode);
        return response;
      }
      else {
        return null;
      }
    }

    public virtual void SendPCPHelo()
    {
      logger.Debug("Handshake Started");
      var helo = new Atom(Atom.PCP_HELO, new AtomCollection());
      helo.Children.SetHeloAgent(peercast.AgentName);
      helo.Children.SetHeloSessionID(peercast.SessionID);
      if (peercast.IsFirewalled.HasValue) {
        if (peercast.IsFirewalled.Value) {
          //Do nothing
        }
        else {
          helo.Children.SetHeloPort((short)peercast.LocalEndPoint.Port);
        }
      }
      else {
        helo.Children.SetHeloPing((short)peercast.LocalEndPoint.Port);
      }
      helo.Children.SetHeloVersion(PCP_VERSION);
      Send(helo);
    }

    public virtual Atom RecvAtom()
    {
      Atom res = null;
      if (recvStream.Length>=8 && Recv(s => { res = AtomReader.Read(s); })) {
        return res;
      }
      else {
        return null;
      }
    }

    private void Channel_HostInfoUpdated(object sender, EventArgs e)
    {
      if (syncContext!=null) {
        syncContext.Post(dummy => {
          hostInfoUpdated = true;
          changedEvent.Set();
        }, null);
      }
      else {
        hostInfoUpdated = true;
        changedEvent.Set();
      }
    }

    public virtual void Start()
    {
      logger.Debug("Started");
      if (this.syncContext == null) {
        this.syncContext = new QueuedSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(this.syncContext);
      }
      channel.OutputStreams.CollectionChanged += Channel_HostInfoUpdated;
      peercast.AccessController.PropertyChanged += Channel_HostInfoUpdated;
      Status = SourceStreamStatus.Searching;
      state = new PCPSourceConnectState(this, SelectSourceHost());
      while (state!=null) {
        ProcessState();
      }
      channel.OutputStreams.CollectionChanged -= Channel_HostInfoUpdated;
      peercast.AccessController.PropertyChanged -= Channel_HostInfoUpdated;
      logger.Debug("Finished");
    }

    public virtual void ProcessState()
    {
      if (state!=null) {
        state = state.Process();
      }
      ProcessSend();
      ProcessEvents();
    }

    /// <summary>
    /// 現在のチャンネルとPeerCastの状態からHostパケットを作ります
    /// </summary>
    /// <returns>作ったPCP_HOSTパケット</returns>
    public virtual Atom CreateHostPacket()
    {
      var host = new AtomCollection();
      peercast.SynchronizationContext.Send(dummy => {
        host.SetHostChannelID(channel.ChannelInfo.ChannelID);
        host.SetHostSessionID(peercast.SessionID);
        var globalendpoint = peercast.GlobalEndPoint ?? new IPEndPoint(IPAddress.Loopback, 7144);
        host.AddHostIP(globalendpoint.Address);
        host.AddHostPort((short)globalendpoint.Port);
        var localendpoint = peercast.LocalEndPoint ?? new IPEndPoint(IPAddress.Loopback, 7144);
        host.AddHostIP(localendpoint.Address);
        host.AddHostPort((short)localendpoint.Port);
        host.SetHostNumListeners(channel.OutputStreams.CountPlaying);
        host.SetHostNumRelays(channel.OutputStreams.CountRelaying);
        host.SetHostUptime(channel.Uptime);
        if (channel.Contents.Count > 0) {
          host.SetHostOldPos((int)channel.Contents.Oldest.Position);
          host.SetHostNewPos((int)channel.Contents.Newest.Position);
        }
        host.SetHostVersion(PCP_VERSION);
        host.SetHostVersionVP(PCP_VERSION_VP);
        host.SetHostFlags1(
          (peercast.AccessController.IsChannelRelayable(channel) ? PCPHostFlags1.Relay : 0) |
          (peercast.AccessController.IsChannelPlayable(channel) ? PCPHostFlags1.Direct : 0) |
          ((!peercast.IsFirewalled.HasValue || peercast.IsFirewalled.Value) ? PCPHostFlags1.Firewalled : 0) |
          PCPHostFlags1.Receiving); //TODO:受信中かどうかちゃんと判別する
        if (uphost != null) {
          var endpoint = uphost.GlobalEndPoint;
          if (endpoint != null) {
            host.SetHostUphostIP(endpoint.Address);
            host.SetHostUphostPort(endpoint.Port);
          }
        }
      }, null);
      return new Atom(Atom.PCP_HOST, host);
    }

    /// <summary>
    /// 指定したパケットを含むブロードキャストパケットを作成します
    /// </summary>
    /// <param name="group">配送先グループ</param>
    /// <param name="packet">配送するパケット</param>
    /// <returns>作成したPCP_BCSTパケット</returns>
    public virtual Atom CreateBroadcastPacket(BroadcastGroup group, Atom packet)
    {
      var bcst = new AtomCollection();
      bcst.SetBcstFrom(peercast.SessionID);
      bcst.SetBcstGroup(group);
      bcst.SetBcstHops(0);
      bcst.SetBcstTTL(11);
      bcst.SetBcstVersion(PCP_VERSION);
      bcst.SetBcstVersionVP(PCP_VERSION_VP);
      bcst.SetBcstChannelID(channel.ChannelInfo.ChannelID);
      bcst.Add(packet);
      return new Atom(Atom.PCP_BCST, bcst);
    }

    public virtual void BroadcastHostInfo()
    {
      logger.Debug("Broadcasting host info");
      channel.Broadcast(null, CreateBroadcastPacket(BroadcastGroup.Trackers, CreateHostPacket()), BroadcastGroup.Trackers);
      hostInfoUpdated = false;
    }

    public virtual IStreamState ProcessAtom(Atom atom)
    {
           if (atom.Name==Atom.PCP_HELO)       return OnPCPHelo(atom);
      else if (atom.Name==Atom.PCP_OLEH)       return OnPCPOleh(atom);
      else if (atom.Name==Atom.PCP_OK)         return OnPCPOk(atom);
      else if (atom.Name==Atom.PCP_CHAN)       return OnPCPChan(atom);
      else if (atom.Name==Atom.PCP_CHAN_PKT)   return OnPCPChanPkt(atom);
      else if (atom.Name==Atom.PCP_CHAN_INFO)  return OnPCPChanInfo(atom);
      else if (atom.Name==Atom.PCP_CHAN_TRACK) return OnPCPChanTrack(atom);
      else if (atom.Name==Atom.PCP_BCST)       return OnPCPBcst(atom);
      else if (atom.Name==Atom.PCP_HOST)       return OnPCPHost(atom);
      else if (atom.Name==Atom.PCP_QUIT)       return OnPCPQuit(atom);
      else                                     return null;
    }

    protected virtual IStreamState OnPCPHelo(Atom atom)
    {
      logger.Debug("Helo Received");
      var res = new Atom(Atom.PCP_OLEH, new AtomCollection());
      if (connection!=null && connection.Client.RemoteEndPoint.AddressFamily==AddressFamily.InterNetwork) {
        res.Children.SetHeloRemoteIP(((IPEndPoint)connection.Client.RemoteEndPoint).Address);
      }
      res.Children.SetHeloAgent(peercast.AgentName);
      res.Children.SetHeloSessionID(peercast.SessionID);
      res.Children.SetHeloPort((short)peercast.LocalEndPoint.Port);
      res.Children.SetHeloVersion(PCP_VERSION);
      Send(res);
      return null;
    }

    protected virtual IStreamState OnPCPOleh(Atom atom)
    {
      peercast.SynchronizationContext.Post(dummy => {
        var rip  = atom.Children.GetHeloRemoteIP();
        if (rip!=null) {
          switch (rip.AddressFamily) {
          case AddressFamily.InterNetwork:
            if (peercast.GlobalAddress==null || !peercast.GlobalAddress.Equals(rip)) {
              peercast.GlobalAddress = rip;
            }
            break;
          case AddressFamily.InterNetworkV6:
            if (peercast.GlobalAddress6==null || !peercast.GlobalAddress6.Equals(rip)) {
              peercast.GlobalAddress6 = rip;
            }
            break;
          }
        }
        var port = atom.Children.GetHeloPort();
        if (port.HasValue) {
          peercast.IsFirewalled = port.Value==0;
        }
        logger.Debug("Handshake Finished: {0}", peercast.GlobalAddress);
      }, null);
      return null;
    }

    protected virtual IStreamState OnPCPOk(Atom atom)
    {
      return null;
    }

    protected virtual IStreamState OnPCPChan(Atom atom)
    {
      IStreamState state = null;
      foreach (var c in atom.Children) {
        state = ProcessAtom(c);
      }
      return state;
    }

    protected virtual IStreamState OnPCPChanPkt(Atom atom)
    {
      var pkt_type = atom.Children.GetChanPktType();
      var pkt_data = atom.Children.GetChanPktData();
      if (pkt_type!=null && pkt_data!=null) {
        if (pkt_type==Atom.PCP_CHAN_PKT_TYPE_HEAD) {
          var pkt_pos = atom.Children.GetChanPktPos();
          peercast.SynchronizationContext.Post(dummy => {
            channel.ContentHeader = new Content((long)(pkt_pos ?? 0), pkt_data);
          }, null);
        }
        else if (pkt_type==Atom.PCP_CHAN_PKT_TYPE_DATA) {
          var pkt_pos = atom.Children.GetChanPktPos();
          if (pkt_pos != null) {
            peercast.SynchronizationContext.Post(dummy => {
              channel.Contents.Add(new Content((long)pkt_pos, pkt_data));
            }, null);
          }
        }
        else if (pkt_type==Atom.PCP_CHAN_PKT_TYPE_META) {
        }
      }
      return null;
    }

    protected virtual IStreamState OnPCPChanInfo(Atom atom)
    {
      peercast.SynchronizationContext.Post(dummy => {
        var name = atom.Children.GetChanInfoName();
        if (name != null) channel.ChannelInfo.Name = name;
        var content_type = atom.Children.GetChanInfoType();
        if (content_type!=null) channel.ChannelInfo.ContentType = content_type;
        channel.ChannelInfo.Extra.SetChanInfo(atom.Children);
      }, null);
      hostInfoUpdated = true;
      changedEvent.Set();
      return null;
    }

    protected virtual IStreamState OnPCPChanTrack(Atom atom)
    {
      peercast.SynchronizationContext.Post(dummy => {
        channel.ChannelInfo.Extra.SetChanTrack(atom.Children);
      }, null);
      return null;
    }

    protected virtual IStreamState OnPCPBcst(Atom atom)
    {
      var dest = atom.Children.GetBcstDest();
      if (dest==null || dest==peercast.SessionID) {
        foreach (var c in atom.Children) ProcessAtom(c);
      }
      var ttl = atom.Children.GetBcstTTL();
      var hops = atom.Children.GetBcstHops();
      var from = atom.Children.GetBcstFrom();
      var group = atom.Children.GetBcstGroup();
      if (ttl != null &&
          hops != null &&
          group != null &&
          from != null &&
          dest != peercast.SessionID &&
          ttl>1) {
        atom.Children.SetBcstTTL((byte)(ttl - 1));
        atom.Children.SetBcstHops((byte)(hops + 1));
        channel.Broadcast(uphost, atom, group.Value);
      }
      return null;
    }

    protected virtual IStreamState OnPCPHost(Atom atom)
    {
      var session_id = atom.Children.GetHostSessionID();
      if (session_id!=null) {
        peercast.SynchronizationContext.Post(dummy => {
          var node = channel.Nodes.FirstOrDefault(x => x.Host.SessionID.Equals(session_id));
          if (node==null) {
            node = new Node(new Host());
            node.Host.SessionID = (Guid)session_id;
            channel.Nodes.Add(node);
          }
          node.Host.Extra.Update(atom.Children);
          node.DirectCount = atom.Children.GetHostNumListeners() ?? 0;
          node.RelayCount = atom.Children.GetHostNumRelays() ?? 0;
          var flags1 = atom.Children.GetHostFlags1();
          if (flags1 != null) {
            node.Host.IsFirewalled = (flags1.Value & PCPHostFlags1.Firewalled) != 0;
            node.IsRelayFull       = (flags1.Value & PCPHostFlags1.Relay) == 0;
            node.IsDirectFull      = (flags1.Value & PCPHostFlags1.Direct) == 0;
            node.IsReceiving       = (flags1.Value & PCPHostFlags1.Receiving) != 0;
            node.IsControlFull     = (flags1.Value & PCPHostFlags1.ControlIn) == 0;
          }

          int addr_count = 0;
          var ip = new IPEndPoint(IPAddress.Any, 0);
          foreach (var c in atom.Children) {
            if (c.Name==Atom.PCP_HOST_IP) {
              IPAddress addr;
              if (c.TryGetIPv4Address(out addr)) {
                ip.Address = addr;
                if (ip.Port!=0) {
                  if (addr_count==0 && (node.Host.GlobalEndPoint==null || node.Host.GlobalEndPoint.Equals(ip))) {
                    node.Host.GlobalEndPoint = ip;
                  }
                  if (addr_count==1 && (node.Host.LocalEndPoint==null || node.Host.LocalEndPoint.Equals(ip))) {
                    node.Host.LocalEndPoint = ip;
                  }
                  ip = new IPEndPoint(IPAddress.Any, 0);
                  addr_count++;
                }
              }
            }
            else if (c.Name==Atom.PCP_HOST_PORT) {
              short port;
              if (c.TryGetInt16(out port)) {
                ip.Port = port;
                if (ip.Address != IPAddress.Any) {
                  if (addr_count==0 && (node.Host.GlobalEndPoint==null || node.Host.GlobalEndPoint.Equals(ip))) {
                    node.Host.GlobalEndPoint = ip;
                  }
                  if (addr_count==1 && (node.Host.LocalEndPoint==null || node.Host.LocalEndPoint.Equals(ip))) {
                    node.Host.LocalEndPoint = ip;
                  }
                  ip = new IPEndPoint(IPAddress.Any, 0);
                  addr_count++;
                }
              }
            }
          }
        }, null);
      }
      return null;
    }

    protected virtual IStreamState OnPCPQuit(Atom atom)
    {
      if (atom.GetInt32() == Atom.PCP_ERROR_QUIT + Atom.PCP_ERROR_UNAVAILABLE) {
        return new PCPSourceClosedState(this, CloseReason.Unavailable);
      }
      else {
        return new PCPSourceClosedState(this, CloseReason.ChannelExit);
      }
    }

    public void Close()
    {
      if (syncContext!=null) {
        syncContext.Post((x) => {
          if (IsConnected) {
            state = new PCPSourceClosedState(this, CloseReason.UserShutdown);
          }
        }, null);
      }
      else {
        if (IsConnected) {
          state = new PCPSourceClosedState(this, CloseReason.UserShutdown);
        }
      }
    }

    public virtual void Post(Host from, Atom packet)
    {
      if (syncContext!=null) {
        syncContext.Post(x => {
          if (uphost != from) {
            Send(packet);
          }
        }
        , null);
      }
      else {
        if (uphost != from) {
          Send(packet);
        }
      }
    }

    public PCPSourceStream(PeerCast peercast, Channel channel, Uri source_uri)
    {
      logger.Debug("Initialized: Channel {0}, Source {1}",
        channel!=null ? channel.ChannelInfo.ChannelID.ToString("N") : "(null)",
        source_uri);
      this.peercast = peercast;
      this.channel = channel;
    }
  }
}
