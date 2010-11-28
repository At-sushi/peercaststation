﻿using System;
using System.Collections.Generic;
using System.Net;

namespace PeerCastStation.Core
{
  public static class AtomCollectionExtensions
  {
    public static int? GetIntFrom(AtomCollection collection, ID4 name)
    {
      var atom = collection.FindByName(name);
      int value = 0;
      if (atom != null && atom.TryGetInt32(out value)) {
        return value;
      }
      else {
        return null;
      }
    }

    public static short? GetShortFrom(AtomCollection collection, ID4 name)
    {
      var atom = collection.FindByName(name);
      short value = 0;
      if (atom != null && atom.TryGetInt16(out value)) {
        return value;
      }
      else {
        return null;
      }
    }

    public static byte? GetByteFrom(AtomCollection collection, ID4 name)
    {
      var atom = collection.FindByName(name);
      byte value = 0;
      if (atom != null && atom.TryGetByte(out value)) {
        return value;
      }
      else {
        return null;
      }
    }

    public static Guid? GetIDFrom(AtomCollection collection, ID4 name)
    {
      var atom = collection.FindByName(name);
      byte[] value = null;
      if (atom != null && atom.TryGetBytes(out value) && value.Length==16) {
        return new Guid(value);
      }
      else {
        return null;
      }
    }

    public static ID4? GetID4From(AtomCollection collection, ID4 name)
    {
      var atom = collection.FindByName(name);
      byte[] value = null;
      if (atom != null && atom.TryGetBytes(out value) && value.Length==4) {
        return new ID4(value);
      }
      else {
        return null;
      }
    }

    public static byte[] GetBytesFrom(AtomCollection collection, ID4 name)
    {
      var atom = collection.FindByName(name);
      byte[] value = null;
      if (atom != null && atom.TryGetBytes(out value)) {
        return value;
      }
      else {
        return null;
      }
    }

    public static IPAddress GetIPAddressFrom(AtomCollection collection, ID4 name)
    {
      var atom = collection.FindByName(name);
      byte[] value = null;
      if (atom != null && atom.TryGetBytes(out value) && value.Length==4) {
        var ip_ary = new byte[value.Length];
        value.CopyTo(ip_ary, 0);
        Array.Reverse(ip_ary);
        return new IPAddress(ip_ary);
      }
      else {
        return null;
      }
    }

    public static string GetStringFrom(AtomCollection collection, ID4 name)
    {
      string res = null;
      var atom = collection.FindByName(name);
      if (atom != null && atom.TryGetString(out res)) {
        return res;
      }
      else {
        return null;
      }
    }

    public static Atom GetAtomFrom(AtomCollection collection, ID4 name)
    {
      return collection.FindByName(name);
    }

    public static Atom GetHelo(this AtomCollection collection)
    {
      return GetAtomFrom(collection, Atom.PCP_HELO);
    }

    public static string GetHeloAgent(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_HELO_AGENT);
    }

    public static Guid? GetHeloSessionID(this AtomCollection collection)
    {
      return GetIDFrom(collection, Atom.PCP_HELO_SESSIONID);
    }

    public static short? GetHeloPort(this AtomCollection collection)
    {
      return GetShortFrom(collection, Atom.PCP_HELO_PORT);
    }

    public static short? GetHeloPing(this AtomCollection collection)
    {
      return GetShortFrom(collection, Atom.PCP_HELO_PING);
    }

    public static IPAddress GetHeloRemoteIP(this AtomCollection collection)
    {
      return GetIPAddressFrom(collection, Atom.PCP_HELO_REMOTEIP);
    }

    public static int? GetHeloVersion(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HELO_VERSION);
    }

    public static Guid? GetHeloBCID(this AtomCollection collection)
    {
      return GetIDFrom(collection, Atom.PCP_HELO_BCID);
    }

    public static int? GetHeloDisable(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HELO_DISABLE);
    }

    public static Atom GetOleh(this AtomCollection collection)
    {
      return GetAtomFrom(collection, Atom.PCP_OLEH);
    }

    public static int? GetOk(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_OK);
    }

    public static Atom GetChan(this AtomCollection collection)
    {
      return GetAtomFrom(collection, Atom.PCP_CHAN);
    }

    public static Guid? GetChanID(this AtomCollection collection)
    {
      return GetIDFrom(collection, Atom.PCP_CHAN_ID);
    }

    public static Guid? GetChanBCID(this AtomCollection collection)
    {
      return GetIDFrom(collection, Atom.PCP_CHAN_BCID);
    }

    public static Atom GetChanPkt(this AtomCollection collection)
    {
      return GetAtomFrom(collection, Atom.PCP_CHAN_PKT);
    }

    public static ID4? GetChanPktType(this AtomCollection collection)
    {
      return GetID4From(collection, Atom.PCP_CHAN_PKT_TYPE);
    }

    public static int? GetChanPktPos(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_CHAN_PKT_POS);
    }

    public static byte[] GetChanPktData(this AtomCollection collection)
    {
      return GetBytesFrom(collection, Atom.PCP_CHAN_PKT_DATA);
    }

    public static Atom GetChanInfo(this AtomCollection collection)
    {
      return GetAtomFrom(collection, Atom.PCP_CHAN_INFO);
    }

    public static string GetChanInfoType(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_INFO_TYPE);
    }

    public static int? GetChanInfoBitrate(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_CHAN_INFO_BITRATE);
    }

    public static string GetChanInfoGenre(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_INFO_GENRE);
    }

    public static string GetChanInfoName(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_INFO_NAME);
    }

    public static string GetChanInfoURL(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_INFO_URL);
    }

    public static string GetChanInfoDesc(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_INFO_DESC);
    }

    public static string GetChanInfoComment(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_INFO_COMMENT);
    }

    public static int? GetChanInfoPPFlags(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_CHAN_INFO_PPFLAGS);
    }

    public static Atom GetChanTrack(this AtomCollection collection)
    {
      return GetAtomFrom(collection, Atom.PCP_CHAN_TRACK);
    }

    public static string GetChanTrackTitle(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_TRACK_TITLE);
    }

    public static string GetChanTrackCreator(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_TRACK_CREATOR);
    }

    public static string GetChanTrackURL(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_TRACK_URL);
    }

    public static string GetChanTrackAlbum(this AtomCollection collection)
    {
      return GetStringFrom(collection, Atom.PCP_CHAN_TRACK_ALBUM);
    }

    public static Atom GetBcst(this AtomCollection collection)
    {
      return GetAtomFrom(collection, Atom.PCP_BCST);
    }

    public static byte? GetBcstTTL(this AtomCollection collection)
    {
      return GetByteFrom(collection, Atom.PCP_BCST_TTL);
    }

    public static byte? GetBcstHops(this AtomCollection collection)
    {
      return GetByteFrom(collection, Atom.PCP_BCST_HOPS);
    }

    public static Guid? GetBcstFrom(this AtomCollection collection)
    {
      return GetIDFrom(collection, Atom.PCP_BCST_FROM);
    }

    public static Guid? GetBcstDest(this AtomCollection collection)
    {
      return GetIDFrom(collection, Atom.PCP_BCST_DEST);
    }

    public static byte? GetBcstGroup(this AtomCollection collection)
    {
      return GetByteFrom(collection, Atom.PCP_BCST_GROUP);
    }

    public static Guid? GetBcstChannelID(this AtomCollection collection)
    {
      return GetIDFrom(collection, Atom.PCP_BCST_CHANID);
    }

    public static int? GetBcstVersion(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_BCST_VERSION);
    }

    public static int? GetBcstVersionVP(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_BCST_VERSION_VP);
    }

    public static byte[] GetBcstVersionEXPrefix(this AtomCollection collection)
    {
      return GetBytesFrom(collection, Atom.PCP_BCST_VERSION_EX_PREFIX);
    }

    public static short? GetBcstVersionEXNumber(this AtomCollection collection)
    {
      return GetShortFrom(collection, Atom.PCP_BCST_VERSION_EX_NUMBER);
    }

    public static Atom GetHost(this AtomCollection collection)
    {
      return GetAtomFrom(collection, Atom.PCP_HOST);
    }

    public static Guid? GetHostSessionID(this AtomCollection collection)
    {
      return GetIDFrom(collection, Atom.PCP_HOST_ID);
    }

    public static IPAddress GetHostIP(this AtomCollection collection)
    {
      return GetIPAddressFrom(collection, Atom.PCP_HOST_IP);
    }

    public static short? GetHostPort(this AtomCollection collection)
    {
      return GetShortFrom(collection, Atom.PCP_HOST_PORT);
    }

    public static Guid? GetHostChannelID(this AtomCollection collection)
    {
      return GetIDFrom(collection, Atom.PCP_HOST_CHANID);
    }

    public static int? GetHostNumListeners(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HOST_NUML);
    }

    public static int? GetHostNumRelays(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HOST_NUMR);
    }

    public static int? GetHostUptime(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HOST_UPTIME);
    }

    public static int? GetHostVersion(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HOST_VERSION);
    }

    public static int? GetHostVersionVP(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HOST_VERSION_VP);
    }

    public static byte[] GetHostVersionEXPrefix(this AtomCollection collection)
    {
      return GetBytesFrom(collection, Atom.PCP_HOST_VERSION_EX_PREFIX);
    }

    public static short? GetHostVersionEXNumber(this AtomCollection collection)
    {
      return GetShortFrom(collection, Atom.PCP_HOST_VERSION_EX_NUMBER);
    }

    public static int? GetHostClapPP(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HOST_CLAP_PP);
    }

    public static int? GetHostOldPos(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HOST_OLDPOS);
    }

    public static int? GetHostNewPos(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HOST_NEWPOS);
    }

    public static byte? GetHostFlags1(this AtomCollection collection)
    {
      return GetByteFrom(collection, Atom.PCP_HOST_FLAGS1);
    }

    public static IPAddress GetHostUphostIP(this AtomCollection collection)
    {
      return GetIPAddressFrom(collection, Atom.PCP_HOST_UPHOST_IP);
    }

    public static int? GetHostUphostPort(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_HOST_UPHOST_PORT);
    }

    public static byte? GetHostUphostHops(this AtomCollection collection)
    {
      return GetByteFrom(collection, Atom.PCP_HOST_UPHOST_HOPS);
    }

    public static int? GetQuit(this AtomCollection collection)
    {
      return GetIntFrom(collection, Atom.PCP_QUIT);
    }
  }
}
