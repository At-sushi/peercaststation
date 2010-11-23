﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Net.Sockets;

namespace PeerCastStation.Core
{
  /// <summary>
  /// 指定されたプラグインを読み込むためのインターフェース
  /// </summary>
  public interface IPlugInLoader
  {
    /// <summary>
    /// プラグインローダの名前を取得します
    /// </summary>
    string Name { get; }
    /// <summary>
    /// ファイルからプラグインを読み込みます
    /// </summary>
    /// <param name="uri">読み込むファイルのURI</param>
    /// <returns>読み込めた場合はプラグインのインスタンス、読み込めなかった場合はnull</returns>
    IPlugIn Load(Uri uri);
  }

  /// <summary>
  /// プラグインのインスタンスを表すインターフェースです
  /// </summary>
  public interface IPlugIn
  {
    /// <summary>
    /// プラグインの名前を取得します
    /// </summary>
    string Name { get; }
    /// <summary>
    /// プラグインが提供する拡張名のリストを取得します
    /// </summary>
    ICollection<string> Extensions { get; }
    /// <summary>
    /// プラグインの説明を取得します
    /// </summary>
    string Description { get; }
    /// <summary>
    /// プラグインの取得元URIを取得します
    /// </summary>
    Uri Contact { get; }
    /// <summary>
    /// Coreインスタンスへのプラグインの登録を行ないます
    /// </summary>
    /// <param name="core">登録先のCoreインスタンス</param>
    void Register(Core core);
    /// <summary>
    /// Coreインスタンスへのプラグイン登録を解除します
    /// </summary>
    /// <param name="core">登録解除するCoreインスタンス</param>
    void Unregister(Core core);
  }

  /// <summary>
  /// YellowPageのインターフェースです
  /// </summary>
  public interface IYellowPage
  {
    /// <summary>
    /// YwlloePageに関連付けられた名前を取得します
    /// </summary>
    string Name { get; }
    /// <summary>
    /// YellowPageのURLを取得します
    /// </summary>
    Uri    Uri  { get; }
    /// <summary>
    /// チャンネルIDからトラッカーを検索し取得します
    /// </summary>
    /// <param name="channel_id">検索するチャンネルID</param>
    /// <returns>見付かった場合は接続先URI、見付からなかった場合はnull</returns>
    Uri FindTracker(Guid channel_id);
    /// <summary>
    /// YellowPageの持っているチャンネル一覧を取得します
    /// </summary>
    /// <returns>取得したチャンネル一覧。取得できなければ空のリスト</returns>
    ICollection<ChannelInfo> ListChannels();
    /// <summary>
    /// YellowPageにチャンネルを載せます
    /// </summary>
    /// <param name="channel">載せるチャンネル</param>
    void Announce(Channel channel);
  }

  /// <summary>
  /// YellowPageのインスタンスを作成するためのファクトリインターフェースです
  /// </summary>
  public interface IYellowPageFactory
  {
    /// <summary>
    /// このYellowPageFactoryが扱うプロトコルの名前を取得します
    /// </summary>
    string Name { get; }
    /// <summary>
    /// YellowPageインスタンスを作成し返します
    /// </summary>
    /// <param name="name">YellowPageに関連付けられる名前</param>
    /// <param name="uri">YellowPageのURI</param>
    /// <returns>IYellowPageのインスタンス</returns>
    IYellowPage Create(string name, Uri uri);
  }

  /// <summary>
  /// 上流からチャンネルにContentを追加するストリームを表すインターフェースです
  /// </summary>
  public interface ISourceStream
  {
    /// <summary>
    /// 指定したホストを起点にストリームの取得を開始します
    /// </summary>
    /// <param name="tracker">ストリーム取得の起点</param>
    /// <param name="channel">取得ストリームの追加先チャンネル</param>
    void Start(Uri tracker, Channel channel);
    /// <summary>
    /// ストリームの取得を終了します
    /// </summary>
    void Close();
  }

  /// <summary>
  /// SourceStreamのインスタンスを作成するファクトリインターフェースです
  /// </summary>
  public interface ISourceStreamFactory
  {
    /// <summary>
    /// このSourceStreamFactoryが扱うプロトコルの名前を取得します
    /// </summary>
    string Name { get; }
    /// <summary>
    /// URIからプロトコルを判別しSourceStreamのインスタンスを作成します。
    /// </summary>
    /// <param name="tracker">プロトコル判別用のURI</param>
    /// <returns>プロトコルが適合していればSourceStreamのインスタンス、それ以外はnull</returns>
    ISourceStream Create(Uri tracker);
  }

  /// <summary>
  /// 下流にチャンネルのContentを流すストリームを表わすインターフェースです
  /// </summary>
  public interface IOutputStream
  {
    /// <summary>
    /// 指定されたStreamへChannelのContentを流しはじめます
    /// </summary>
    /// <param name="stream">書き込み先のストリーム</param>
    /// <param name="channel">情報を流す元のチャンネル</param>
    void Start(Stream stream, Channel channel);
    /// <summary>
    /// ストリームへの書き込みを終了します
    /// </summary>
    void Close();
  }

  /// <summary>
  /// OutputStreamのインスタンスを作成するファクトリインターフェースです
  /// </summary>
  public interface IOutputStreamFactory
  {
    /// <summary>
    /// このOutputStreamが扱うプロトコルの名前を取得します
    /// </summary>
    string Name { get; }
    /// <summary>
    /// OutpuStreamのインスタンスを作成します
    /// </summary>
    /// <returns>OutputStream</returns>
    IOutputStream Create();
    /// <summary>
    /// クライアントのリクエストからチャンネルIDを取得し返します
    /// </summary>
    /// <param name="header">クライアントから受け取ったリクエスト</param>
    /// <returns>headerからチャンネルIDを取得できた場合はチャンネルID、できなかった場合はnull</returns>
    Guid? ParseChannelID(byte[] header);
  }

  /// <summary>
  /// 接続情報を保持するクラスです
  /// </summary>
  public class Host
  {
    /// <summary>
    /// ホストが持つアドレス情報のリストを返します
    /// </summary>
    public IList<IPEndPoint> Addresses { get; private set; }
    /// <summary>
    /// ホストのセッションIDを取得および設定します
    /// </summary>
    public Guid SessionID { get; set; }
    /// <summary>
    /// ホストのブロードキャストIDを取得および設定します
    /// </summary>
    public Guid BroadcastID { get; set; }
    /// <summary>
    /// ホストへの接続が可能かどうかを取得および設定します
    /// </summary>
    public bool IsFirewalled { get; set; }
    /// <summary>
    /// ホストの拡張リストを取得します
    /// </summary>
    public IList<string> Extensions { get; private set; }
    /// <summary>
    /// その他のホスト情報リストを取得します
    /// </summary>
    public AtomCollection Extra { get; private set; }

    /// <summary>
    /// ホスト情報を初期化します
    /// </summary>
    public Host()
    {
      Addresses    = new List<IPEndPoint>();
      SessionID    = Guid.Empty;
      BroadcastID  = Guid.Empty;
      IsFirewalled = false;
      Extensions   = new List<string>();
      Extra        = new AtomCollection();
    }
  }

  /// <summary>
  /// 主にAtomの名前などに使われる4文字の識別子クラスです
  /// </summary>
  public class ID4
  {
    private static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding(false);
    private byte[] value = new byte[4] { 0, 0, 0, 0 };

    /// <summary>
    /// 文字列からID4インスタンスを初期化します
    /// </summary>
    /// <param name="name">設定する4文字以内のASCII文字列</param>
    public ID4(string name)
    {
      var nameb = encoding.GetBytes(name);
      if (nameb.Length > 4) {
        throw new ArgumentException("ID4 length must be 4 or less.");
      }
      nameb.CopyTo(value, 0);
    }

    /// <summary>
    /// バイト列からID4インスタンスを初期化します
    /// </summary>
    /// <param name="value">設定する4バイト以内のバイト配列</param>
    public ID4(byte[] value)
    {
      if (value.Length > 4) {
        throw new ArgumentException("ID4 length must be 4 or less.");
      }
      value.CopyTo(this.value, 0);
    }

    /// <summary>
    /// バイト列の一部からID4インスタンスを初期化します
    /// </summary>
    /// <param name="value">設定する4バイト以内のバイト配列</param>
    /// <param name="index">valueの先頭からのオフセット</param>
    public ID4(byte[] value, int index)
    {
      Array.Copy(value, index, this.value, 0, Math.Min(value.Length-index, 4));
    }

    /// <summary>
    /// 保持しているバイト列を取得します
    /// </summary>
    /// <returns>4バイトのバイト配列</returns>
    public byte[] GetBytes()
    {
      return value;
    }

    /// <summary>
    /// 保持しているバイト列を文字列として取得します
    /// </summary>
    /// <returns>文字列に変換された値</returns>
    public override string ToString()
    {
      return encoding.GetString(value.TakeWhile((x) => { return x != 0; }).ToArray());
    }

    public override int GetHashCode()
    {
      return value[0] << 24 | value[0] << 16 | value[0] << 8 | value[0] << 0;
    }

    public override bool Equals(object obj)
    {
      var x = obj as ID4;
      if (x != null) {
        var vx = x.GetBytes();
        return
          vx.Length == value.Length &&
          value[0] == vx[0] &&
          value[1] == vx[1] &&
          value[2] == vx[2] &&
          value[3] == vx[3];
      }
      else {
        return false;
      }
    }
  }

  /// <summary>
  /// PCPプロトコルの基本通信単位を表すクラスです。
  /// 4文字以下の名前と対応する値を保持します
  /// </summary>
  public class Atom
  {
    public static readonly ID4 PCP_HELO                   = new ID4("helo");
    public static readonly ID4 PCP_HELO_AGENT             = new ID4("agnt");
    public static readonly ID4 PCP_HELO_OSTYPE            = new ID4("ostp");
    public static readonly ID4 PCP_HELO_SESSIONID         = new ID4("sid");
    public static readonly ID4 PCP_HELO_PORT              = new ID4("port");
    public static readonly ID4 PCP_HELO_PING              = new ID4("ping");
    public static readonly ID4 PCP_HELO_PONG              = new ID4("pong");
    public static readonly ID4 PCP_HELO_REMOTEIP          = new ID4("rip");
    public static readonly ID4 PCP_HELO_VERSION           = new ID4("ver");
    public static readonly ID4 PCP_HELO_BCID              = new ID4("bcid");
    public static readonly ID4 PCP_HELO_DISABLE           = new ID4("dis");
    public static readonly ID4 PCP_OLEH                   = new ID4("oleh");
    public static readonly ID4 PCP_OK                     = new ID4("ok");
    public static readonly ID4 PCP_CHAN                   = new ID4("chan");
    public static readonly ID4 PCP_CHAN_ID                = new ID4("id");
    public static readonly ID4 PCP_CHAN_BCID              = new ID4("bcid");
    public static readonly ID4 PCP_CHAN_PKT               = new ID4("pkt");
    public static readonly ID4 PCP_CHAN_PKT_TYPE          = new ID4("type");
    public static readonly ID4 PCP_CHAN_PKT_HEAD          = new ID4("head");
    public static readonly ID4 PCP_CHAN_PKT_META          = new ID4("meta");
    public static readonly ID4 PCP_CHAN_PKT_POS           = new ID4("pos");
    public static readonly ID4 PCP_CHAN_PKT_DATA          = new ID4("data");
    public static readonly ID4 PCP_CHAN_PKT_TYPE_HEAD     = new ID4("head");
    public static readonly ID4 PCP_CHAN_PKT_TYPE_META     = new ID4("meta");
    public static readonly ID4 PCP_CHAN_PKT_TYPE_DATA     = new ID4("data");
    public static readonly ID4 PCP_CHAN_INFO              = new ID4("info");
    public static readonly ID4 PCP_CHAN_INFO_TYPE         = new ID4("type");
    public static readonly ID4 PCP_CHAN_INFO_BITRATE      = new ID4("bitr");
    public static readonly ID4 PCP_CHAN_INFO_GENRE        = new ID4("gnre");
    public static readonly ID4 PCP_CHAN_INFO_NAME         = new ID4("name");
    public static readonly ID4 PCP_CHAN_INFO_URL          = new ID4("url");
    public static readonly ID4 PCP_CHAN_INFO_DESC         = new ID4("desc");
    public static readonly ID4 PCP_CHAN_INFO_COMMENT      = new ID4("cmnt");
    public static readonly ID4 PCP_CHAN_INFO_PPFLAGS      = new ID4("pflg");
    public static readonly ID4 PCP_CHAN_TRACK             = new ID4("trck");
    public static readonly ID4 PCP_CHAN_TRACK_TITLE       = new ID4("titl");
    public static readonly ID4 PCP_CHAN_TRACK_CREATOR     = new ID4("crea");
    public static readonly ID4 PCP_CHAN_TRACK_URL         = new ID4("url");
    public static readonly ID4 PCP_CHAN_TRACK_ALBUM       = new ID4("albm");
    public static readonly ID4 PCP_BCST                   = new ID4("bcst");
    public static readonly ID4 PCP_BCST_TTL               = new ID4("ttl");
    public static readonly ID4 PCP_BCST_HOPS              = new ID4("hops");
    public static readonly ID4 PCP_BCST_FROM              = new ID4("from");
    public static readonly ID4 PCP_BCST_DEST              = new ID4("dest");
    public static readonly ID4 PCP_BCST_GROUP             = new ID4("grp");
    public static readonly ID4 PCP_BCST_CHANID            = new ID4("cid");
    public static readonly ID4 PCP_BCST_VERSION           = new ID4("vers");
    public static readonly ID4 PCP_BCST_VERSION_VP        = new ID4("vrvp");
    public static readonly ID4 PCP_BCST_VERSION_EX_PREFIX = new ID4("vexp");
    public static readonly ID4 PCP_BCST_VERSION_EX_NUMBER = new ID4("vexn");
    public static readonly ID4 PCP_HOST                   = new ID4("host");
    public static readonly ID4 PCP_HOST_ID                = new ID4("id");
    public static readonly ID4 PCP_HOST_IP                = new ID4("ip");
    public static readonly ID4 PCP_HOST_PORT              = new ID4("port");
    public static readonly ID4 PCP_HOST_CHANID            = new ID4("cid");
    public static readonly ID4 PCP_HOST_NUML              = new ID4("numl");
    public static readonly ID4 PCP_HOST_NUMR              = new ID4("numr");
    public static readonly ID4 PCP_HOST_UPTIME            = new ID4("uptm");
    public static readonly ID4 PCP_HOST_TRACKER           = new ID4("trkr");
    public static readonly ID4 PCP_HOST_VERSION           = new ID4("ver");
    public static readonly ID4 PCP_HOST_VERSION_VP        = new ID4("vevp");
    public static readonly ID4 PCP_HOST_VERSION_EX_PREFIX = new ID4("vexp");
    public static readonly ID4 PCP_HOST_VERSION_EX_NUMBER = new ID4("vexn");
    public static readonly ID4 PCP_HOST_CLAP_PP           = new ID4("clap");
    public static readonly ID4 PCP_HOST_OLDPOS            = new ID4("oldp");
    public static readonly ID4 PCP_HOST_NEWPOS            = new ID4("newp");
    public static readonly ID4 PCP_HOST_FLAGS1            = new ID4("flg1");
    public static readonly ID4 PCP_HOST_UPHOST_IP         = new ID4("upip");
    public static readonly ID4 PCP_HOST_UPHOST_PORT       = new ID4("uppt");
    public static readonly ID4 PCP_HOST_UPHOST_HOPS       = new ID4("uphp");
    public static readonly ID4 PCP_QUIT                   = new ID4("quit");

    private byte[] value = null;
    private AtomCollection children = null;

    /// <summary>
    /// 名前を取得します
    /// </summary>
    public ID4 Name  { get; private set; }

    /// <summary>
    /// 子ATOMを保持しているかどうかを取得します
    /// </summary>
    public bool HasChildren { get { return children!=null; } }

    /// <summary>
    /// 値を保持しているかどうかを取得します
    /// </summary>
    public bool HasValue { get { return value!=null; } }

    /// <summary>
    /// 子Atomのコレクションを取得します。値を保持している場合はnullを返します
    /// </summary>
    public AtomCollection Children { get { return children; } }

    /// <summary>
    /// 保持している値をbyteとして取得します。
    /// </summary>
    /// <returns>保持している値</returns>
    /// <exception cref="FormatException">
    /// 値の長さが合わない、または値を保持していません
    /// </exception>
    public byte GetByte()
    {
      byte res;
      if (TryGetByte(out res)) {
        return res;
      }
      else {
        throw new FormatException();
      }
    }

    /// <summary>
    /// 保持している値をbyteとして取得しようと試みます。
    /// </summary>
    /// <param name="res">保持している値の書き込み先</param>
    /// <returns>値がbyteとして解析できた場合はtrue、そうでない場合はfalse</returns>
    public bool TryGetByte(out byte res)
    {
      if (value != null && value.Length == 1) {
        res = value[0];
        return true;
      }
      else {
        res = 0;
        return false;
      }
    }

    /// <summary>
    /// 保持している値をInt16として取得します。
    /// </summary>
    /// <returns>保持している値</returns>
    /// <exception cref="FormatException">
    /// 値の長さが合わない、または値を保持していません
    /// </exception>
    public short GetInt16()
    {
      short res;
      if (TryGetInt16(out res)) {
        return res;
      }
      else {
        throw new FormatException();
      }
    }

    /// <summary>
    /// 保持している値をInt16として取得しようと試みます。
    /// </summary>
    /// <param name="res">保持している値の書き込み先</param>
    /// <returns>値がInt16として解析できた場合はtrue、そうでない場合はfalse</returns>
    public bool TryGetInt16(out short res)
    {
      if (value != null && value.Length == 2) {
        res = BitConverter.ToInt16(value, 0);
        return true;
      }
      else {
        res = 0;
        return false;
      }
    }

    /// <summary>
    /// 保持している値をInt32として取得します。
    /// </summary>
    /// <returns>保持している値</returns>
    /// <exception cref="FormatException">
    /// 値の長さが合わない、または値を保持していません
    /// </exception>
    public int GetInt32()
    {
      int res;
      if (TryGetInt32(out res)) {
        return res;
      }
      else {
        throw new FormatException();
      }
    }

    /// <summary>
    /// 保持している値をInt32として取得しようと試みます。
    /// </summary>
    /// <param name="res">保持している値の書き込み先</param>
    /// <returns>値がInt32として解析できた場合はtrue、そうでない場合はfalse</returns>
    public bool TryGetInt32(out int res)
    {
      if (value != null && value.Length == 4) {
        res = BitConverter.ToInt32(value, 0);
        return true;
      }
      else {
        res = 0;
        return false;
      }
    }

    /// <summary>
    /// 保持している値を文字列として取得します。
    /// </summary>
    /// <returns>保持している値</returns>
    /// <exception cref="FormatException">
    /// 値がNULL文字で終端されていない、または値を保持していません
    /// </exception>
    public string GetString()
    {
      string res;
      if (TryGetString(out res)) {
        return res;
      }
      else {
        throw new FormatException();
      }
    }

    /// <summary>
    /// 保持している値をInt32として取得しようと試みます。
    /// </summary>
    /// <param name="res">保持している値の書き込み先</param>
    /// <returns>値がInt32として解析できた場合はtrue、そうでない場合はfalse</returns>
    public bool TryGetString(out string res)
    {
      if (value != null && value[value.Length - 1] == 0) {
        res = System.Text.Encoding.UTF8.GetString(value, 0, value.Length - 1);
        return true;
      }
      else {
        res = "";
        return false;
      }
    }

    /// <summary>
    /// 保持している値をバイト配列として取得します
    /// </summary>
    /// <exception cref="FormatException">
    /// 値を保持していません
    /// </exception>
    /// <returns>値のバイト列</returns>
    public byte[] GetBytes()
    {
      if (value != null) {
        return value;
      }
      else {
        throw new FormatException();
      }
    }

    /// <summary>
    /// 名前と値を指定してAtomを初期化します。
    /// </summary>
    /// <param name="name">4文字以下の名前</param>
    /// <param name="value">byte値</param>
    public Atom(ID4 name, byte value)
    {
      Name = name;
      this.value = new byte[] { value };
    }

    /// <summary>
    /// 名前と値を指定してAtomを初期化します。
    /// </summary>
    /// <param name="name">4文字以下の名前</param>
    /// <param name="value">Int16値</param>
    public Atom(ID4 name, short value)
    {
      Name = name;
      this.value = BitConverter.GetBytes(value);
      if (!BitConverter.IsLittleEndian) Array.Reverse(this.value);
    }

    /// <summary>
    /// 名前と値を指定してAtomを初期化します。
    /// </summary>
    /// <param name="name">4文字以下の名前</param>
    /// <param name="value">Int32値</param>
    public Atom(ID4 name, int value)
    {
      Name = name;
      this.value = BitConverter.GetBytes(value);
      if (!BitConverter.IsLittleEndian) Array.Reverse(this.value);
    }

    /// <summary>
    /// 名前と値を指定してAtomを初期化します。
    /// </summary>
    /// <param name="name">4文字以下の名前</param>
    /// <param name="value">byte配列</param>
    public Atom(ID4 name, byte[] value)
    {
      Name = name;
      this.value = value;
    }

    /// <summary>
    /// 名前と値を指定してAtomを初期化します。
    /// </summary>
    /// <param name="name">4文字以下の名前</param>
    /// <param name="value">文字列</param>
    public Atom(ID4 name, string value)
    {
      Name = name;
      var str = System.Text.Encoding.UTF8.GetBytes(value);
      this.value = new byte[str.Length + 1];
      str.CopyTo(this.value, 0);
      this.value[str.Length] = 0;
    }

    /// <summary>
    /// 名前と子のコレクションを指定してAtomを初期化します。
    /// </summary>
    /// <param name="name">4文字以下の名前</param>
    /// <param name="children">保持する子のコレクション</param>
    public Atom(ID4 name, AtomCollection children)
    {
      Name = name;
      this.children = children;
    }
  }

  /// <summary>
  /// ストリームにAtomを書き込むためのアダプタクラスです
  /// </summary>
  public class AtomWriter
    : IDisposable
  {
    private bool disposed = false;
    private Stream stream;
    private System.Text.Encoding encoding = new System.Text.UTF8Encoding(false);

    /// <summary>
    /// 指定したストリームに書き込むインスタンスを初期化します
    /// </summary>
    /// <param name="stream">書き込み先のストリーム</param>
    public AtomWriter(Stream stream)
    {
      this.stream = stream;
    }

    /// <summary>
    /// 基になるストリームを取得します
    /// </summary>
    public Stream BaseStream
    {
      get
      {
        return stream;
      }
    }

    /// <summary>
    /// 保持しているストリームを閉じます
    /// </summary>
    public void Close()
    {
      stream.Close();
    }

    /// <summary>
    /// このインスタンスによって使用されているすべてのリソースを解放します
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
      if (!disposed) {
        if (disposing) {
          stream.Dispose();
        }
        disposed = true;
      }
    }

    /// <summary>
    /// Atom全体をストリームに書き込みます
    /// </summary>
    /// <param name="atom">書き込むAtom</param>
    public void Write(Atom atom)
    {
      var name = atom.Name.GetBytes();
      stream.Write(name, 0, name.Length);
      if (atom.HasValue) {
        var value = atom.GetBytes();
        var len = BitConverter.GetBytes(value.Length);
        if (!BitConverter.IsLittleEndian) Array.Reverse(len);
        stream.Write(len, 0, len.Length);
        stream.Write(value, 0, value.Length);
      }
      else {
        var cnt = BitConverter.GetBytes(0x80000000U | (uint)atom.Children.Count);
        if (!BitConverter.IsLittleEndian) Array.Reverse(cnt);
        stream.Write(cnt, 0, cnt.Length);
        foreach (var child in atom.Children) {
          Write(child);
        }
      }
    }
  }

  /// <summary>
  /// ストリームからAtomを読み取るアダプタクラスです
  /// </summary>
  public class AtomReader
    : IDisposable
  {
    private bool disposed = false;
    private Stream stream;
    private System.Text.Encoding encoding = new System.Text.UTF8Encoding(false);

    /// <summary>
    /// 指定したストリームから読み取るインスタンスを初期化します
    /// </summary>
    /// <param name="stream">読み取り元のストリーム</param>
    public AtomReader(Stream stream)
    {
      this.stream = stream;
    }

    /// <summary>
    /// 基になるストリームを取得します
    /// </summary>
    public Stream BaseStream
    {
      get
      {
        return stream;
      }
    }

    /// <summary>
    /// 現在のAtomReaderと基になるストリームを閉じます。 
    /// </summary>
    public void Close()
    {
      stream.Close();
    }

    /// <summary>
    /// このインスタンスによって使用されているすべてのリソースを解放します
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
      if (!disposed) {
        if (disposing) {
          stream.Dispose();
        }
        disposed = true;
      }
    }

    /// <summary>
    /// ストリームからAtomを読み取ります
    /// </summary>
    /// <returns>読み取ったAtomのインスタンス</returns>
    public Atom Read()
    {
      var header = new byte[8];
      stream.Read(header, 0, 8);
      var name = new ID4(header, 0);
      if (!BitConverter.IsLittleEndian) Array.Reverse(header, 4, 4);
      uint len = BitConverter.ToUInt32(header, 4);
      if ((len & 0x80000000U)!=0) {
        var children = new AtomCollection();
        for (var i=0; i<(len&0x7FFFFFFF); i++) {
          children.Add(Read());
        }
        return new Atom(name, children);
      }
      else {
        var value = new byte[len];
        stream.Read(value, 0, (int)len);
        return new Atom(name, value);
      }
    }
  }

  public class AtomCollection : ObservableCollection<Atom>
  {
  }

  /// <summary>
  /// チャンネルのメタデータを保持するクラスです
  /// </summary>
  public class ChannelInfo
    : INotifyPropertyChanged
  {
    private Guid channelID;
    private Uri tracker = null;
    private string name = "";
    /// <summary>
    /// 接続起点のURIを取得および設定します
    /// </summary>
    public Uri Tracker {
      get { return tracker; }
      set {
        tracker = value;
        OnPropertyChanged("Tracker");
      }
    }
    /// <summary>
    /// チャンネルIDを取得します
    /// </summary>
    public Guid ChannelID {
      get { return channelID; }
    }
    /// <summary>
    /// チャンネル名を取得および設定します
    /// </summary>
    public string Name {
      get { return name; }
      set {
        name = value;
        OnPropertyChanged("Name");
      }
    }
    private AtomCollection extra = new AtomCollection();
    /// <summary>
    /// その他のチャンネル情報を保持するリストを取得します
    /// </summary>
    public AtomCollection Extra { get { return extra; } }
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string name)
    {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(name));
      }
    }

    /// <summary>
    /// チャンネルIDを指定して新しいチャンネル情報を初期化します
    /// </summary>
    /// <param name="channel_id">チャンネルID</param>
    public ChannelInfo(Guid channel_id)
    {
      channelID = channel_id;
      extra.CollectionChanged += (sender, e) => {
        OnPropertyChanged("Extra");
      };
    }
  }

  /// <summary>
  /// チャンネルリレー中のノードを表わすクラスです
  /// </summary>
  public class Node
    : INotifyPropertyChanged
  {
    private Host host = null;
    private int relayCount = 0;
    private int directCount = 0;
    private bool isRelayFull = false;
    private bool isDirectFull = false;
    private AtomCollection extra = new AtomCollection();
    /// <summary>
    /// 接続情報を取得および設定します
    /// </summary>
    public Host Host {
      get { return host; }
      set
      {
        host = value;
        OnPropertyChanged("Host");
      }
    }
    /// <summary>
    /// リレーしている数を取得および設定します
    /// </summary>
    public int RelayCount {
      get { return relayCount; }
      set
      {
        relayCount = value;
        OnPropertyChanged("RelayCount");
      }
    }
    /// <summary>
    /// 直接視聴している数を取得および設定します
    /// </summary>
    public int DirectCount {
      get { return directCount; }
      set
      {
        directCount = value;
        OnPropertyChanged("DirectCount");
      }
    }
    /// <summary>
    /// リレー数が一杯かどうかを取得および設定します
    /// </summary>
    public bool IsRelayFull {
      get { return isRelayFull; }
      set
      {
        isRelayFull = value;
        OnPropertyChanged("IsRelayFull");
      }
    }
    /// <summary>
    /// 直接視聴数が一杯かどうかを取得および設定します
    /// </summary>
    public bool IsDirectFull {
      get { return isDirectFull; }
      set
      {
        isDirectFull = value;
        OnPropertyChanged("IsDirectFull");
      }
    }
    /// <summary>
    /// その他の情報のリストを取得します
    /// </summary>
    public AtomCollection Extra { get { return extra; } }
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string name)
    {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(name));
      }
    }

    /// <summary>
    /// 接続情報からノード情報を初期化します
    /// </summary>
    /// <param name="host">ノードの接続情報</param>
    public Node(Host host)
    {
      Host = host;
      extra.CollectionChanged += (sender, e) => {
        OnPropertyChanged("Extra");
      };
    }
  }

  /// <summary>
  /// チャンネルの状態を表します
  /// </summary>
  public enum ChannelStatus
  {
    /// <summary>
    /// 接続が開始されていません
    /// </summary>
    Idle,
    /// <summary>
    /// 接続先を検索中です
    /// </summary>
    Searching,
    /// <summary>
    /// 接続しています
    /// </summary>
    Connecting,
    /// <summary>
    /// ストリームを受け取っています
    /// </summary>
    Receiving,
    /// <summary>
    /// 接続エラーが起きました
    /// </summary>
    Error,
    /// <summary>
    /// チャンネルが閉じられました
    /// </summary>
    Closed
  }

  /// <summary>
  /// チャンネル接続を管理するクラスです
  /// </summary>
  public class Channel
    : INotifyPropertyChanged
  {
    private Uri sourceUri = null;
    private ChannelStatus status = ChannelStatus.Idle;
    private ISourceStream sourceStream = null;
    private ObservableCollection<IOutputStream> outputStreams = new ObservableCollection<IOutputStream>();
    private ObservableCollection<Node> nodes = new ObservableCollection<Node>();
    private ChannelInfo channelInfo;
    private Content contentHeader = null;
    private ObservableCollection<Content> contents = new ObservableCollection<Content>();
    private Thread sourceThread = null;
    /// <summary>
    /// チャンネルの状態を取得および設定します
    /// </summary>
    public ChannelStatus Status {
      get { return status; }
      set
      {
        status = value;
        OnPropertyChanged("Status");
      }
    }
    /// <summary>
    /// コンテント取得元のUriを取得します
    /// </summary>
    public Uri SourceUri
    {
      get { return sourceUri; }
    }
    /// <summary>
    /// ソースストリームを取得および設定します
    /// </summary>
    public ISourceStream SourceStream {
      get { return sourceStream; }
      set
      {
        sourceStream = value;
        OnPropertyChanged("SourceStream");
      }
    }
    /// <summary>
    /// 出力ストリームのリストを取得します
    /// </summary>
    public IList<IOutputStream> OutputStreams { get { return outputStreams; } }
    /// <summary>
    /// このチャンネルに関連付けられたノードリストを取得します
    /// </summary>
    public IList<Node> Nodes { get { return nodes; } }
    /// <summary>
    /// チャンネル情報を取得および設定します
    /// </summary>
    public ChannelInfo ChannelInfo { get { return channelInfo; } }
    /// <summary>
    /// ヘッダコンテントを取得および設定します
    /// </summary>
    public Content ContentHeader {
      get { return contentHeader; }
      set
      {
        contentHeader = value;
        OnPropertyChanged("ContentHeader");
        OnContentChanged();
      }
    }
    /// <summary>
    /// ヘッダを除く保持しているコンテントのリストを取得します
    /// </summary>
    public IList<Content> Contents { get { return contents; } }
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string name)
    {
      if (PropertyChanged != null) {
        PropertyChanged(this, new PropertyChangedEventArgs(name));
      }
    }
    private void OnContentChanged()
    {
      if (ContentChanged != null) {
        ContentChanged(this, new EventArgs());
      }
    }
    /// <summary>
    /// コンテントが追加および削除された時に発生するイベントです
    /// </summary>
    public event EventHandler ContentChanged;
    /// <summary>
    /// チャンネル接続が終了する時に発生するイベントです
    /// </summary>
    public event EventHandler Closed;
    private void OnClosed()
    {
      if (Closed != null) {
        Closed(this, new EventArgs());
      }
    }

    public void Start()
    {
      sourceThread = new Thread(SourceThreadFunc);
      sourceThread.Start();
    }

    private void SourceThreadFunc()
    {
      try {
        sourceStream.Start(sourceUri, this);
      }
      finally {
        sourceStream.Close();
      }
    }

    /// <summary>
    /// チャンネル接続を終了します。ソースストリームと接続している出力ストリームを全て閉じます
    /// </summary>
    public void Close()
    {
      if (Status != ChannelStatus.Closed) {
        if (sourceThread != null) {
          sourceThread.Abort();
        }
        foreach (var os in outputStreams) {
          os.Close();
        }
        Status = ChannelStatus.Closed;
        OnClosed();
      }
    }

    /// <summary>
    /// チャンネルIDとソースストリームを指定してチャンネルを初期化します
    /// </summary>
    /// <param name="channel_id">チャンネルID</param>
    /// <param name="source">ソースストリーム</param>
    /// <param name="source_uri">ソースURI</param>
    public Channel(Guid channel_id, ISourceStream source, Uri source_uri)
    {
      sourceUri = source_uri;
      sourceStream  = source;
      channelInfo   = new ChannelInfo(channel_id);
      channelInfo.PropertyChanged += (sender, e) => {
        OnPropertyChanged("ChannelInfo");
      };
      contents.CollectionChanged += (sender, e) => {
        OnPropertyChanged("Contents");
        OnContentChanged();
      };
      outputStreams.CollectionChanged += (sender, e) => {
        OnPropertyChanged("OutputStreams");
      };
      nodes.CollectionChanged += (sender, e) => {
        OnPropertyChanged("Nodes");
      };
    }
  }

  /// <summary>
  /// チャンネルのストリーム内容を表わすクラスです
  /// </summary>
  public class Content
  {
    /// <summary>
    /// コンテントの位置を取得します。
    /// 位置はバイト数や時間とか関係なくソースの出力したパケット番号です
    /// </summary>
    public long Position { get; private set; } 
    /// <summary>
    /// コンテントの内容を取得します
    /// </summary>
    public byte[] Data   { get; private set; } 

    /// <summary>
    /// コンテントの位置と内容を指定して初期化します
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="data">内容</param>
    public Content(long pos, byte[] data)
    {
      Position = pos;
      Data = data;
    }
  }

  /// <summary>
  /// PeerCastStationの主要な動作を行ない、管理するクラスです
  /// </summary>
  public class Core
  {
    public Host Host { get; set; }
    /// <summary>
    /// 登録されているプラグインローダのリストを取得します
    /// </summary>
    public IList<IPlugInLoader> PlugInLoaders { get; private set; }
    /// <summary>
    /// 読み込まれたプラグインのリストを取得します
    /// </summary>
    public ICollection<IPlugIn> PlugIns       { get; private set; }
    /// <summary>
    /// 登録されているYellowPageのリストを取得します
    /// </summary>
    public IList<IYellowPage>   YellowPages   { get; private set; }
    /// <summary>
    /// 登録されているYellowPageのプロトコルとファクトリの辞書を取得します
    /// </summary>
    public IDictionary<string, IYellowPageFactory>   YellowPageFactories   { get; private set; }
    /// <summary>
    /// 登録されているSourceStreamのプロトコルとファクトリの辞書を取得します
    /// </summary>
    public IDictionary<string, ISourceStreamFactory> SourceStreamFactories { get; private set; }
    /// <summary>
    /// 登録されているOutputStreamのリストを取得します
    /// </summary>
    public IList<IOutputStreamFactory> OutputStreamFactories { get; private set; }
    /// <summary>
    /// 接続しているチャンネルのリストを取得します
    /// </summary>
    public ICollection<Channel> Channels { get { return channels; } }
    private List<Channel> channels = new List<Channel>();

    /// <summary>
    /// 待ち受けが閉じられたかどうかを取得します
    /// </summary>
    public bool IsClosed { get; private set; }

    /// <summary>
    /// 指定したファイルをプラグインとして読み込みます
    /// </summary>
    /// <param name="uri">読み込むファイル</param>
    /// <returns>読み込めた場合はPlugInのインスタンス、それ以外はnull</returns>
    public IPlugIn LoadPlugIn(Uri uri)
    {
      foreach (var loader in PlugInLoaders) {
        var plugin = loader.Load(uri);
        if (plugin!=null) {
          plugin.Register(this);
          return plugin;
        }
      }
      return null;
    }

    /// <summary>
    /// チャンネルIDを指定してチャンネルのリレーを開始します。
    /// 接続先はYellowPageに問い合わせ取得します。
    /// </summary>
    /// <param name="channel_id">リレーを開始するチャンネルID</param>
    /// <returns>接続先が見付かった場合はChannelのインスタンス、それ以外はnull</returns>
    public Channel RelayChannel(Guid channel_id)
    {
      foreach (var yp in YellowPages) {
        var tracker = yp.FindTracker(channel_id);
        if (tracker!=null) {
          return RelayChannel(channel_id, tracker);
        }
      }
      return null;
    }

    /// <summary>
    /// 接続先を指定してチャンネルのリレーを開始します。
    /// URIから接続プロトコルも判別します
    /// </summary>
    /// <param name="channel_id">リレーするチャンネルID</param>
    /// <param name="tracker">接続起点およびプロトコル</param>
    /// <returns>Channelのインスタンス</returns>
    public Channel RelayChannel(Guid channel_id, Uri tracker)
    {
      ISourceStreamFactory source_factory = null;
      if (!SourceStreamFactories.TryGetValue(tracker.Scheme, out source_factory)) {
        throw new ArgumentException(String.Format("Protocol `{0}' is not found", tracker.Scheme));
      }
      var source_stream = source_factory.Create(tracker);
      var channel = new Channel(channel_id, source_stream, tracker);
      channels.Add(channel);
      channel.Start();
      return channel;
    }

    /// <summary>
    /// 配信を開始します。
    /// </summary>
    /// <param name="yp">チャンネル情報を載せるYellowPage</param>
    /// <param name="channel_id">チャンネルID</param>
    /// <param name="protocol">出力プロトコル</param>
    /// <param name="source">配信ソース</param>
    /// <returns>Channelのインスタンス</returns>
    public Channel BroadcastChannel(IYellowPage yp, Guid channel_id, string protocol, Uri source) { return null; }

    /// <summary>
    /// 指定したチャンネルをチャンネルリストから取り除きます
    /// </summary>
    /// <param name="channel"></param>
    public void CloseChannel(Channel channel)
    {
      channel.Close();
      channels.Remove(channel);
    }

    /// <summary>
    /// 接続待ち受けアドレスを指定してCoreを初期化します
    /// </summary>
    /// <param name="ip">接続を待ち受けるアドレス</param>
    public Core(IPEndPoint ip)
    {
      IsClosed = false;
      Host = new Host();
      Host.SessionID = Guid.NewGuid();

      PlugInLoaders = new List<IPlugInLoader>();
      PlugIns       = new List<IPlugIn>();
      YellowPages   = new List<IYellowPage>();
      YellowPageFactories = new Dictionary<string, IYellowPageFactory>();
      SourceStreamFactories = new Dictionary<string, ISourceStreamFactory>();
      OutputStreamFactories = new List<IOutputStreamFactory>();

      outputWaitThread = new Thread(OutputWaitThreadFunc);
      outputWaitThread.Start(ip);
    }

    /// <summary>
    /// 待ち受けと全ての接続を終了します
    /// </summary>
    public void Close()
    {
      IsClosed = true;
      outputWaitThread.Join();
      foreach (var output_thread in outputThreads) {
        output_thread.Abort();
      }
    }

    private List<Thread> outputThreads = new List<Thread>();
    private Thread outputWaitThread = null;
    private void OutputWaitThreadFunc(object arg)
    {
      var ip = (IPEndPoint)arg;
      var server = new TcpListener(ip);
      server.Start();
      Host.Addresses.Add((IPEndPoint)server.LocalEndpoint);
      while (!IsClosed) {
        if (server.Pending()) {
          var client = server.AcceptTcpClient();
          var output_thread = new Thread(OutputThreadFunc);
          output_thread.Start(client);
          outputThreads.Add(output_thread);
        }
        else {
          Thread.Sleep(10);
        }
      }
      server.Stop();
    }

    private void OutputThreadFunc(object arg)
    {
      var client = (TcpClient)arg;
      var stream = client.GetStream();
      IOutputStream output_stream = null;
      try {
        var header = new List<byte>();
        Guid? channel_id = null;
        while (output_stream == null && header.Count <= 1024) {
          var val = stream.ReadByte();
          if (val < 0) {
            break;
          }
          else {
            header.Add((byte)val);
          }
          var header_ary = header.ToArray();
          foreach (var factory in OutputStreamFactories) {
            channel_id = factory.ParseChannelID(header_ary);
            if (channel_id != null) {
              output_stream = factory.Create();
              break;
            }
          }
        }
        if (output_stream != null) {
          var channel = channels.Find((c) => { return c.ChannelInfo.ChannelID == channel_id; });
          output_stream.Start(stream, channel);
        }
      }
      finally {
        if (output_stream != null) {
          output_stream.Close();
        }
        stream.Close();
        client.Close();
        outputThreads.Remove(Thread.CurrentThread);
      }
    }
  }
}

