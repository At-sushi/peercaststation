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
using PeerCastStation.Core;
using PeerCastStation.HTTP;

namespace PeerCastStation.Ohaoha
{
  public class OhaohaCheckOutputStreamFactory
    : OutputStreamFactoryBase
  {
    public override string Name
    {
      get { return "OhaohaCheck"; }
    }

    public override IOutputStream Create(
      Stream input_stream,
      Stream output_stream,
      EndPoint remote_endpoint,
      Guid channel_id,
      byte[] header)
    {
      return new OhaohaCheckOutputStream(PeerCast, input_stream, output_stream, remote_endpoint);
    }

    public override Guid? ParseChannelID(byte[] header)
    {
      HTTPRequest res = null;
      var stream = new MemoryStream(header);
      try {
        res = HTTPRequestReader.Read(stream);
      }
      catch (EndOfStreamException) {
      }
      stream.Close();
      if (res!=null && res.Method=="GET" && res.Uri.AbsolutePath=="/" && res.Headers.Count==0) {
        return Guid.Empty;
      }
      else {
        return null;
      }
    }

    public OhaohaCheckOutputStreamFactory(PeerCast peercast)
      : base(peercast)
    {
    }
  }

  public class OhaohaCheckOutputStream
    : OutputStreamBase
  {
    public OhaohaCheckOutputStream(
      PeerCast peercast,
      Stream input_stream,
      Stream output_stream,
      EndPoint remote_endpoint)
      : base(peercast, input_stream, output_stream, remote_endpoint, null, null)
    {
      Logger.Debug("Initialized: Remote {0}", remote_endpoint);
    }

    protected override void OnStarted()
    {
      Logger.Debug("Started");
      var response = "HTTP/1.0 302 Found\r\nLocation: /html/index.html\r\n\r\n";
      var bytes = System.Text.Encoding.UTF8.GetBytes(response);
      Send(bytes);
      Stop();
    }

    protected override void OnStopped()
    {
      Logger.Debug("Finished");
   	  base.OnStopped();
    }

    public override OutputStreamType OutputStreamType
    {
      get { return OutputStreamType.Metadata; }
    }
  }
}
