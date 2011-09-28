﻿// PeerCastStation, a P2P streaming servent.
// Copyright (C) 2011 Ryuichi Sakamoto (kumaryu@kumaryu.net)
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
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
    : SourceStreamFactoryBase
  {
    public PCPSourceStreamFactory(PeerCast peercast)
      : base(peercast)
    {
    }

    public override string Name { get { return "pcp"; } }
    public override ISourceStream Create(Channel channel, Uri tracker)
    {
      return new PCPSourceStream(PeerCast, channel, tracker);
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

  public class PCPSourceStream : SourceStreamBase
  {
    private enum State
    {
      None = 0,
      Connecting,
      Handshaking,
      Receiving,
      RelayRequesting,
      WaitRequestResponse,
      Retrying,
    };
    private State state;
    private TcpClient client = null;
    private bool hostInfoUpdated = true;
    private System.Threading.AutoResetEvent changedEvent = new System.Threading.AutoResetEvent(true);

    private const int PCP_VERSION    = 1218;
    private const int PCP_VERSION_VP = 27;

    public Host Uphost { get; private set; }

    public bool StartConnection(Host host)
    {
      if (host!=null && host.GlobalEndPoint!=null) {
        client = new TcpClient();
        IPEndPoint point;
        if (PeerCast.GlobalAddress!=null &&
            PeerCast.GlobalAddress.Equals(host.GlobalEndPoint.Address) &&
            host.LocalEndPoint!=null) {
          point = host.LocalEndPoint;
        }
        else {
          point = host.GlobalEndPoint;
        }
        try {
          client.Connect(point);
          var stream = client.GetStream();
          StartConnection(stream, stream);
          Uphost = host;
          Logger.Debug("Connected: {0}", point);
          return true;
        }
        catch (SocketException e) {
          Logger.Debug("Connection Failed: {0}", point);
          Logger.Debug(e);
          OnError();
          return false;
        }
      }
      else {
        Stop(StopReason.NoHost);
        return false;
      }
    }

    protected override void EndConnection()
    {
      base.EndConnection();
      if (client!=null) client.Close();
      client = null;
    }

    public void IgnoreHost(Host host)
    {
      if (host!=null) {
        Logger.Debug("Host {0}({1}) is ignored", host.GlobalEndPoint, host.SessionID.ToString("N"));
      }
      Channel.IgnoreHost(host);
    }

    protected override void DoStop(SourceStreamBase.StopReason reason)
    {
      EndConnection();
      switch (reason) {
      case StopReason.UserShutdown:
        Status = SourceStreamStatus.Idle;
        state = State.None;
        base.DoStop(reason);
        break;
      case StopReason.NoHost:
        Status = SourceStreamStatus.Error;
        state = State.None;
        base.DoStop(reason);
        break;
      case StopReason.UserReconnect:
      case StopReason.UnavailableError:
        IgnoreHost(Uphost);
        Status = SourceStreamStatus.Searching;
        state = State.Connecting;
        break;
      case StopReason.OffAir:
      case StopReason.ConnectionError:
        if (Uphost==null || Uphost.Equals(Channel.SourceHost)) {
          Status = SourceStreamStatus.Error;
          state = State.None;
          base.DoStop(reason);
        }
        else {
          Status = SourceStreamStatus.Searching;
          IgnoreHost(Uphost);
          state = State.Connecting;
        }
        break;
      default:
        base.DoStop(reason);
        break;
      }
    }

    public virtual void SendRelayRequest()
    {
      Logger.Debug("Sending Relay request: /channel/{0}", Channel.ChannelID.ToString("N"));
      var req = String.Format(
        "GET /channel/{0} HTTP/1.0\r\n" +
        "x-peercast-pcp:1\r\n" +
        "\r\n", Channel.ChannelID.ToString("N"));
      Send(System.Text.Encoding.UTF8.GetBytes(req));
    }

    public virtual RelayRequestResponse RecvRelayRequestResponse()
    {
      RelayRequestResponse response = null;
      if (Recv(s => { response = RelayRequestResponseReader.Read(s); })) {
        Logger.Debug("Relay response: {0}", response.StatusCode);
        return response;
      }
      else {
        return null;
      }
    }

    public virtual void SendPCPHelo()
    {
      Logger.Debug("Handshake Started");
      var helo = new AtomCollection();
      helo.SetHeloAgent(PeerCast.AgentName);
      helo.SetHeloSessionID(PeerCast.SessionID);
      if (PeerCast.IsFirewalled.HasValue) {
        if (PeerCast.IsFirewalled.Value) {
          //Do nothing
        }
        else {
          helo.SetHeloPort((short)PeerCast.LocalEndPoint.Port);
        }
      }
      else {
        helo.SetHeloPing((short)PeerCast.LocalEndPoint.Port);
      }
      helo.SetHeloVersion(PCP_VERSION);
      Send(new Atom(Atom.PCP_HELO, helo));
    }

    private void Channel_HostInfoUpdated(object sender, EventArgs e)
    {
      SyncContext.Post(dummy => {
        hostInfoUpdated = true;
        changedEvent.Set();
      }, null);
    }

    protected override void OnStarted()
    {
      base.OnStarted();
      Logger.Debug("Started");
      Channel.PropertyChanged += Channel_HostInfoUpdated;
      PeerCast.AccessController.PropertyChanged += Channel_HostInfoUpdated;
      Status = SourceStreamStatus.Searching;
      state = State.Connecting;
    }

    protected override void OnStopped()
    {
      Channel.PropertyChanged -= Channel_HostInfoUpdated;
      PeerCast.AccessController.PropertyChanged -= Channel_HostInfoUpdated;
      Logger.Debug("Finished");
      base.OnStopped();
    }

    /// <summary>
    /// 現在のチャンネルとPeerCastの状態からHostパケットを作ります
    /// </summary>
    /// <returns>作ったPCP_HOSTパケット</returns>
    public virtual Atom CreateHostPacket()
    {
      var host = new AtomCollection();
      host.SetHostChannelID(Channel.ChannelID);
      host.SetHostSessionID(PeerCast.SessionID);
      var globalendpoint = PeerCast.GlobalEndPoint ?? new IPEndPoint(IPAddress.Loopback, 7144);
      host.AddHostIP(globalendpoint.Address);
      host.AddHostPort((short)globalendpoint.Port);
      var localendpoint = PeerCast.LocalEndPoint ?? new IPEndPoint(IPAddress.Loopback, 7144);
      host.AddHostIP(localendpoint.Address);
      host.AddHostPort((short)localendpoint.Port);
      host.SetHostNumListeners(Channel.LocalDirects);
      host.SetHostNumRelays(Channel.LocalRelays);
      host.SetHostUptime(Channel.Uptime);
      if (Channel.Contents.Count > 0) {
        host.SetHostOldPos((uint)(Channel.Contents.Oldest.Position & 0xFFFFFFFFU));
        host.SetHostNewPos((uint)(Channel.Contents.Newest.Position & 0xFFFFFFFFU));
      }
      host.SetHostVersion(PCP_VERSION);
      host.SetHostVersionVP(PCP_VERSION_VP);
      host.SetHostFlags1(
        (PeerCast.AccessController.IsChannelRelayable(Channel) ? PCPHostFlags1.Relay : 0) |
        (PeerCast.AccessController.IsChannelPlayable(Channel) ? PCPHostFlags1.Direct : 0) |
        ((!PeerCast.IsFirewalled.HasValue || PeerCast.IsFirewalled.Value) ? PCPHostFlags1.Firewalled : 0) |
        PCPHostFlags1.Receiving); //TODO:受信中かどうかちゃんと判別する
      if (Uphost != null) {
        var endpoint = Uphost.GlobalEndPoint;
        if (endpoint != null) {
          host.SetHostUphostIP(endpoint.Address);
          host.SetHostUphostPort(endpoint.Port);
        }
      }
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
      bcst.SetBcstFrom(PeerCast.SessionID);
      bcst.SetBcstGroup(group);
      bcst.SetBcstHops(0);
      bcst.SetBcstTTL(11);
      bcst.SetBcstVersion(PCP_VERSION);
      bcst.SetBcstVersionVP(PCP_VERSION_VP);
      bcst.SetBcstChannelID(Channel.ChannelID);
      bcst.Add(packet);
      return new Atom(Atom.PCP_BCST, bcst);
    }

    public virtual void BroadcastHostInfo()
    {
      Logger.Debug("Broadcasting host info");
      Channel.Broadcast(null, CreateBroadcastPacket(BroadcastGroup.Trackers, CreateHostPacket()), BroadcastGroup.Trackers);
      hostInfoUpdated = false;
    }

    private void OnConnecting()
    {
      state = State.Connecting;
      var host = Channel.SelectSourceHost(SourceHostSelection.Receiving);
      if (host!=null) {
        Logger.Debug("{0} is selected as source", host.GlobalEndPoint);
        if (StartConnection(host)) {
          SendRelayRequest();
          state = State.WaitRequestResponse;
        }
      }
      else {
        Logger.Debug("No selectable host");
        Stop(StopReason.NoHost);
      }
    }

    private void OnWaitRequestResponse()
    {
      RecvEvent.WaitOne(1);
      var res = RecvRelayRequestResponse();
      if (res!=null) {
        if (res.StatusCode==200 || res.StatusCode==503) {
          SendPCPHelo();
          state = State.Handshaking;
        }
        else if (res.StatusCode==404) {
          Stop(StopReason.OffAir);
        }
        else {
          Stop(StopReason.UnavailableError);
        }
      }
    }

    private void OnHandshaking()
    {
      RecvEvent.WaitOne(1);
      var atom = RecvAtom();
      if (atom==null) return;
      else if (atom.Name==Atom.PCP_OLEH) {
        OnPCPOleh(atom);
        state = State.Receiving;
      }
      else if (atom.Name==Atom.PCP_QUIT) {
        OnPCPQuit(atom);
      }
    }

    private int LastHostInfoUpdated = 0;
    private void OnReceiving()
    {
      Status = SourceStreamStatus.Receiving;
      WaitHandle.WaitAny(new WaitHandle[] { RecvEvent, changedEvent }, 1);
      if ((Environment.TickCount-LastHostInfoUpdated>=10000 && hostInfoUpdated) ||
           Environment.TickCount-LastHostInfoUpdated>=120000) {
        BroadcastHostInfo();
        LastHostInfoUpdated = Environment.TickCount;
      }
      ProcessAtom(RecvAtom());
    }

    protected override void OnIdle()
    {
      base.OnIdle();
      switch (state) {
      case State.None:
        break;
      case State.Connecting:
        OnConnecting();
        break;
      case State.WaitRequestResponse:
        OnWaitRequestResponse();
        break;
      case State.Handshaking:
        OnHandshaking();
        break;
      case State.Receiving:
        OnReceiving();
        break;
      case State.Retrying:
        state = State.Connecting;
        break;
      }
    }

    protected Atom RecvAtom()
    {
      Atom res = null;
      if (Recv(s => { res = AtomReader.Read(s); })) {
        return res;
      }
      else {
        return null;
      }
    }

    protected void ProcessAtom(Atom atom)
    {
      if (atom==null) return;
      else if (atom.Name==Atom.PCP_OK)         OnPCPOk(atom);
      else if (atom.Name==Atom.PCP_CHAN)       OnPCPChan(atom);
      else if (atom.Name==Atom.PCP_CHAN_PKT)   OnPCPChanPkt(atom);
      else if (atom.Name==Atom.PCP_CHAN_INFO)  OnPCPChanInfo(atom);
      else if (atom.Name==Atom.PCP_CHAN_TRACK) OnPCPChanTrack(atom);
      else if (atom.Name==Atom.PCP_BCST)       OnPCPBcst(atom);
      else if (atom.Name==Atom.PCP_HOST)       OnPCPHost(atom);
      else if (atom.Name==Atom.PCP_QUIT)       OnPCPQuit(atom);
    }

    protected void OnPCPOleh(Atom atom)
    {
      var rip = atom.Children.GetHeloRemoteIP();
      if (rip!=null) {
        switch (rip.AddressFamily) {
        case AddressFamily.InterNetwork:
          if (PeerCast.GlobalAddress==null ||
              Utils.GetAddressLocality(PeerCast.GlobalAddress)<=Utils.GetAddressLocality(rip)) {
            PeerCast.GlobalAddress = rip;
          }
          break;
        case AddressFamily.InterNetworkV6:
          if (PeerCast.GlobalAddress6==null ||
              Utils.GetAddressLocality(PeerCast.GlobalAddress6)<=Utils.GetAddressLocality(rip)) {
            PeerCast.GlobalAddress6 = rip;
          }
          break;
        }
      }
      var port = atom.Children.GetHeloPort();
      if (port.HasValue) {
        PeerCast.IsFirewalled = port.Value==0;
      }
      Logger.Debug("Handshake Finished: {0}", PeerCast.GlobalAddress);
    }

    protected void OnPCPOk(Atom atom)
    {
    }

    protected void OnPCPChan(Atom atom)
    {
      foreach (var c in atom.Children) {
        ProcessAtom(c);
      }
    }

    protected void OnPCPChanPkt(Atom atom)
    {
      var pkt_type = atom.Children.GetChanPktType();
      var pkt_data = atom.Children.GetChanPktData();
      if (pkt_type!=null && pkt_data!=null) {
        if (pkt_type==Atom.PCP_CHAN_PKT_TYPE_HEAD) {
          long pkt_pos = atom.Children.GetChanPktPos() ?? 0;
          long last_pos = 0;
          if (Channel.Contents.Newest!=null) {
            last_pos = Channel.Contents.Newest.Position;
          }
          else if (Channel.ContentHeader!=null) {
            last_pos = Channel.ContentHeader.Position;
          }
          if (pkt_pos<=(last_pos&0xFFFFFFFFU)-0x80000000) {
            pkt_pos += (last_pos&0x7FFFFFFF00000000) + 0x100000000;
          }
          else {
            pkt_pos += (last_pos&0x7FFFFFFF00000000);
          }
          Channel.ContentHeader = new Content(pkt_pos, pkt_data);
        }
        else if (pkt_type==Atom.PCP_CHAN_PKT_TYPE_DATA) {
          if (atom.Children.GetChanPktPos()!=null) {
            long pkt_pos = atom.Children.GetChanPktPos().Value;
            long last_pos = 0;
            if (Channel.Contents.Newest!=null) {
              last_pos = Channel.Contents.Newest.Position;
            }
            else if (Channel.ContentHeader!=null) {
              last_pos = Channel.ContentHeader.Position;
            }
            if (pkt_pos<=(last_pos&0xFFFFFFFFU)-0x80000000) {
              pkt_pos += (last_pos&0x7FFFFFFF00000000) + 0x100000000;
            }
            else {
              pkt_pos += (last_pos&0x7FFFFFFF00000000);
            }
            Channel.Contents.Add(new Content(pkt_pos, pkt_data));
          }
        }
        else if (pkt_type==Atom.PCP_CHAN_PKT_TYPE_META) {
        }
      }
    }

    protected void OnPCPChanInfo(Atom atom)
    {
      Channel.ChannelInfo = new ChannelInfo(atom.Children);
      hostInfoUpdated = true;
      changedEvent.Set();
    }

    protected void OnPCPChanTrack(Atom atom)
    {
      Channel.ChannelTrack = new ChannelTrack(atom.Children);
    }

    protected void OnPCPBcst(Atom atom)
    {
      var dest = atom.Children.GetBcstDest();
      if (dest==null || dest==PeerCast.SessionID) {
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
          dest != PeerCast.SessionID &&
          ttl>1) {
        var bcst = new AtomCollection(atom.Children);
        bcst.SetBcstTTL((byte)(ttl - 1));
        bcst.SetBcstHops((byte)(hops + 1));
        Channel.Broadcast(Uphost, new Atom(atom.Name, bcst), group.Value);
      }
    }

    protected void OnPCPHost(Atom atom)
    {
      var session_id = atom.Children.GetHostSessionID();
      if (session_id!=null) {
        var node = Channel.Nodes.FirstOrDefault(x => x.SessionID.Equals(session_id));
        var host = new HostBuilder(node);
        if (node==null) {
          host.SessionID = (Guid)session_id;
        }
        host.Extra.Update(atom.Children);
        host.DirectCount = atom.Children.GetHostNumListeners() ?? 0;
        host.RelayCount = atom.Children.GetHostNumRelays() ?? 0;
        var flags1 = atom.Children.GetHostFlags1();
        if (flags1 != null) {
          host.IsFirewalled  = (flags1.Value & PCPHostFlags1.Firewalled) != 0;
          host.IsTracker     = (flags1.Value & PCPHostFlags1.Tracker) != 0;
          host.IsRelayFull   = (flags1.Value & PCPHostFlags1.Relay) == 0;
          host.IsDirectFull  = (flags1.Value & PCPHostFlags1.Direct) == 0;
          host.IsReceiving   = (flags1.Value & PCPHostFlags1.Receiving) != 0;
          host.IsControlFull = (flags1.Value & PCPHostFlags1.ControlIn) == 0;
        }

        int addr_count = 0;
        var ip = new IPEndPoint(IPAddress.Any, 0);
        foreach (var c in atom.Children) {
          if (c.Name==Atom.PCP_HOST_IP) {
            IPAddress addr;
            if (c.TryGetIPv4Address(out addr)) {
              ip.Address = addr;
              if (ip.Port!=0) {
                if (addr_count==0 && (host.GlobalEndPoint==null || host.GlobalEndPoint.Equals(ip))) {
                  host.GlobalEndPoint = ip;
                }
                if (addr_count==1 && (host.LocalEndPoint==null || host.LocalEndPoint.Equals(ip))) {
                  host.LocalEndPoint = ip;
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
                if (addr_count==0 && (host.GlobalEndPoint==null || host.GlobalEndPoint.Equals(ip))) {
                  host.GlobalEndPoint = ip;
                }
                if (addr_count==1 && (host.LocalEndPoint==null || host.LocalEndPoint.Equals(ip))) {
                  host.LocalEndPoint = ip;
                }
                ip = new IPEndPoint(IPAddress.Any, 0);
                addr_count++;
              }
            }
          }
        }
        Channel.AddNode(host.ToHost());
      }
    }

    protected void OnPCPQuit(Atom atom)
    {
      if (atom.GetInt32()==Atom.PCP_ERROR_QUIT+Atom.PCP_ERROR_UNAVAILABLE) {
        Stop(StopReason.UnavailableError);
      }
      else {
        Stop(StopReason.OffAir);
      }
    }

    protected override void DoReconnect()
    {
      base.DoReconnect();
      Stop(StopReason.UserReconnect);
    }

    protected override void DoPost(Host from, Atom packet)
    {
      if (Uphost!=from) {
        Send(packet);
      }
    }

    public PCPSourceStream(PeerCast peercast, Channel channel, Uri source_uri)
      : base(peercast, channel, source_uri)
    {
      Logger.Debug("Initialized: Channel {0}, Source {1}",
        channel!=null ? channel.ChannelID.ToString("N") : "(null)",
        source_uri);
    }
  }
}
