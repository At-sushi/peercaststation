﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PeerCastStation.Core;
using PeerCastStation.FLV.AMF;

namespace PeerCastStation.FLV.RTMP
{
	public class RTMPPlayConnection
		: RTMPConnection
	{
		private static Logger logger = new Logger(typeof(RTMPPlayConnection));
		public RTMPOutputStream owner;
		public Channel  Channel  { get; private set; }
		public long     StreamId { get; private set; }

		public RTMPPlayConnection(
			RTMPOutputStream owner,
			System.IO.Stream input_stream,
			System.IO.Stream output_stream)
			: base(input_stream, output_stream)
		{
			this.owner = owner;
		}

		protected override void OnClose()
		{
			base.OnClose();
			if (this.Channel!=null) {
				this.Channel.ContentChanged -= OnContentChanged;
			}
		}

		private class StreamName
		{
			public string Name { get; set; }
			public Dictionary<string,string> Parameters { get; set; }

			public StreamName()
			{
				this.Name = "";
				this.Parameters = new Dictionary<string,string>();
			}

			public override string ToString()
			{
				return String.Join("?",
					Uri.EscapeDataString(this.Name),
					String.Join("&",
						this.Parameters.Select(kv =>
							Uri.EscapeDataString(kv.Key) + "=" +
							Uri.EscapeDataString(kv.Value)
						)
					)
				);
			}

			public static StreamName Parse(string str)
			{
				var result = new StreamName();
				var param_begin = str.IndexOf('?');
				if (param_begin<0) {
					result.Name = str;
					return result;
				}
				result.Name = Uri.UnescapeDataString(str.Substring(0, param_begin));
				var params_str = str.Substring(param_begin+1);
				foreach (var param_str in params_str.Split('&')) {
					var idx = param_str.IndexOf('=');
					if (idx<0) continue;
					var key = Uri.UnescapeDataString(param_str.Substring(0, idx));
					var val = Uri.UnescapeDataString(param_str.Substring(idx+1));
					result.Parameters[key] = val;
				}
				return result;
			}
		}

		protected override async Task OnCommandPlay(CommandMessage msg, CancellationToken cancel_token)
		{
			var stream_name = StreamName.Parse((string)msg.Arguments[0]);
			var start       = msg.Arguments.Count>1 ? (int)msg.Arguments[1] : -2;
			var duration    = msg.Arguments.Count>2 ? (int)msg.Arguments[2] : -1;
			var reset       = msg.Arguments.Count>3 ? (bool)msg.Arguments[3] : false;
			var channel_id  = Guid.Parse(stream_name.Name);
			//TODO: チャンネルIDがパースできなかった時にエラーを返す
			var tracker_uri =
				stream_name.Parameters.ContainsKey("tip") ?
				OutputStreamBase.CreateTrackerUri(channel_id, stream_name.Parameters["tip"]) :
				null;
			this.Channel = owner.RequestChannel(channel_id, tracker_uri);
			//TODO: チャンネルが見つからなかった時にエラーを返す
			//TODO: チャンネルがFLVじゃなかった場合もエラーを返す
			logger.Debug("Play: {0}, {1}, {2}, {3}", stream_name.ToString(), start, duration, reset);
			this.StreamId = msg.StreamId;
			await SendMessage(2, new UserControlMessage.StreamBeginMessage(this.Now, 0, msg.StreamId), cancel_token);
			var status_start = CommandMessage.Create(
				this.ObjectEncoding,
				this.Now,
				msg.StreamId,
				"onStatus",
				msg.TransactionId+1,
				null,
				new AMFValue(new AMFObject {
					{ "level",       "status" },
					{ "code",        "NetStream.Play.Start" },
					{ "description", stream_name.ToString() },
				})
			);
			await SendMessage(3, status_start, cancel_token);
			if (reset) {
				var status_reset = CommandMessage.Create(
					this.ObjectEncoding,
					this.Now,
					msg.StreamId,
					"onStatus",
					msg.TransactionId+1,
					null,
					new AMFValue(new AMFObject {
						{ "level",       "status" },
						{ "code",        "NetStream.Play.Reset" },
						{ "description", stream_name.ToString() },
					})
				);
				await SendMessage(3, status_reset, cancel_token);
			}
			var result = CommandMessage.Create(
				this.ObjectEncoding,
				this.Now,
				msg.StreamId,
				"_result",
				msg.TransactionId,
				null
			);
			if (msg.TransactionId!=0) {
				await SendMessage(3, result, cancel_token);
			}
			this.Channel.ContentChanged += OnContentChanged;
			OnContentChanged(this, new EventArgs());
		}

		private Content headerPacket = null;
		private Content lastPacket = null;
		private object locker = new object();
		private void OnContentChanged(object sender, EventArgs args)
		{
			lock (locker) {
				var new_header = Channel.ContentHeader;
				if (new_header!=headerPacket) {
					headerPacket = Channel.ContentHeader;
					PostContent(headerPacket);
					lastPacket = headerPacket;
				}
				if (headerPacket==null) return;
				IEnumerable<Content> contents;
				contents = Channel.Contents.GetNewerContents(lastPacket.Stream, lastPacket.Timestamp, lastPacket.Position);
				foreach (var content in contents) {
					PostContent(content);
					lastPacket = content;
				}
			}
		}

		class RTMPContentSink
			: IRTMPContentSink
		{
			private RTMPPlayConnection connection;
			public RTMPContentSink(RTMPPlayConnection conn)
			{
				this.connection = conn;
			}

			public void OnFLVHeader()
			{
			}

			public void OnData(DataMessage msg)
			{
				this.connection.PostMessage(3, msg);
			}

			public void OnVideo(RTMPMessage msg)
			{
				this.connection.PostMessage(3, msg);
			}

			public void OnAudio(RTMPMessage msg)
			{
				this.connection.PostMessage(3, msg);
			}
		}

		private System.IO.MemoryStream contentBuffer = new System.IO.MemoryStream();
		private FLVFileParser fileParser = new FLVFileParser();
		private RTMPContentSink contentSink;
		private void PostContent(Content content)
		{
			var pos = contentBuffer.Position;
			contentBuffer.Write(content.Data, 0, content.Data.Length);
			contentBuffer.Position = pos;
			if (contentSink==null) contentSink = new RTMPContentSink(this);
			fileParser.Read(contentBuffer, contentSink);
			if (contentBuffer.Position!=0) {
				var new_buf = new System.IO.MemoryStream();
				var trim_pos = contentBuffer.Position;
				contentBuffer.Close();
				var buf = contentBuffer.ToArray();
				new_buf.Write(buf, (int)trim_pos, (int)(buf.Length-trim_pos));
				new_buf.Position = 0;
				contentBuffer = new_buf;
			}
		}

	}
}
