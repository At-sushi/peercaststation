﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeerCastStation.Core;
using System.IO;

namespace PeerCastStation.HTTP
{
  class HTTPLiveStreamingSegmenter : IContentSink, Channel.IHTTPLiveStreaming
  {
    protected Logger Logger { get; private set; }
    public Channel Channel { get; private set; }
    private int SegmentIndex = 1;
    private bool Drop = true;
    private double lastPcr = 0;
    private byte[] HeaderData { get; set; }
    private Ringbuffer<Channel.HLSSegment> Segments = new Ringbuffer<Channel.HLSSegment>(5);
    private MemoryStream Cache = new MemoryStream();

    public HTTPLiveStreamingSegmenter(Channel channel)
    {
      Logger = new Logger(this.GetType());
      this.Channel = channel;
      IContentSink sink = this;
      sink =
          "flvtots".Split(',')
          .Select(name => Channel.PeerCast.ContentFilters.FirstOrDefault(filter => filter.Name.ToLowerInvariant() == name.ToLowerInvariant()))
          .Where(filter => filter != null)
          .Aggregate(sink, (r, filter) => filter.Activate(r));
      Channel.AddContentSink(sink);
    }

    public void OnChannelInfo(ChannelInfo channel_info)
    {
    }

    public void OnChannelTrack(ChannelTrack channel_track)
    {
    }

    public void OnContent(Content content)
    {
      MemoryStream ms = new MemoryStream();
      ms.Write(content.Data, 0, content.Data.Length);
      ms.Position = 0;
      int r = 0;
      while (r<content.Data.Length) {
        byte[] bytes188 = new byte[188];
        ms.Read(bytes188, 0, 188);
        TSPacket tsPacket = new TSPacket(bytes188);
        if (tsPacket.keyframe) {
          Logger.Debug("HlsSegment keyframe");
          if (Cache.Length > 0) {
            Cache.Close();
            byte[] data = Cache.ToArray();
            var duration = tsPacket.program_clock_reference - lastPcr;
            Cache = new MemoryStream();
            Logger.Debug("segment index:{0} size:{1}", SegmentIndex.ToString(), data.Length.ToString());
            lastPcr = tsPacket.program_clock_reference;
            Segments.Add(new Channel.HLSSegment(SegmentIndex++, data, duration));
          }
          Drop = false;
        }
        if (!Drop) {
          if (Cache.Length == 0) {
            Cache.Write(HeaderData, 0, HeaderData.Length);
          }
          Cache.Write(bytes188, 0, 188);
          if (Cache.Length > 8 * 1024 * 1024) {
            throw new Exception("Buffer Overflow");
          }
        }
        r += 188;
      }
    }

    public void OnContentHeader(Content content_header)
    {
      this.HeaderData = content_header.Data;
    }

    public void OnStop(StopReason reason)
    {
      Channel.RemoveContentSink(this);
    }

    public IList<Channel.HLSSegment> GetSegments()
    {
      return Segments.ToArray();
    }

  }
}

