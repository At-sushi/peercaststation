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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PeerCastStation.Core
{
  /// <summary>
  /// チャンネルのストリーム内容を表わすクラスです
  /// </summary>
  [Serializable]
  public class Content
  {
    /// <summary>
    /// コンテントのストリーム番号を取得します
    /// </summary>
    public int Stream { get; private set; } 
    /// <summary>
    /// コンテントのストリーム開始時点からの時刻を取得します。
    /// 時刻はコンテントストリームの論理時間と一致するとは限りません
    /// </summary>
    public TimeSpan Timestamp { get; private set; } 
    /// <summary>
    /// コンテントのストリーム開始時点からのバイト位置を取得します
    /// </summary>
    public long Position { get; private set; } 
    /// <summary>
    /// コンテントの内容を取得します
    /// </summary>
    public byte[] Data   { get; private set; } 

    /// <summary>
    /// コンテントのストリーム番号、時刻、位置、内容を指定して初期化します
    /// </summary>
    /// <param name="stream">ストリーム番号</param>
    /// <param name="timestamp">時刻</param>
    /// <param name="pos">バイト位置</param>
    /// <param name="data">内容</param>
    public Content(int stream, TimeSpan timestamp, long pos, byte[] data)
    {
      Stream    = stream;
      Timestamp = timestamp;
      Position  = pos;
      Data      = data;
    }
  }

  public class ContentCollection
    : MarshalByRefObject,
      ICollection<Content>
  {
    private struct ContentKey
      : IComparable<ContentKey>
    {
      public int      Stream;
      public TimeSpan Timestamp;
      public long     Position;

      public ContentKey(int stream, TimeSpan timestamp, long position)
      {
        Stream = stream;
        Timestamp = timestamp;
        Position = position;
      }

      public int CompareTo(ContentKey other)
      {
        var s = Stream.CompareTo(other.Stream);
        if (s!=0) return s;
        var t = Timestamp.CompareTo(other.Timestamp);
        if (t!=0) return t;
        return Position.CompareTo(other.Position);
      }
    }

    private SortedList<ContentKey, Content> list = new SortedList<ContentKey, Content>();
    public long LimitPackets { get; set; }
    public ContentCollection()
    {
      LimitPackets = 100;
    }

    public event EventHandler ContentChanged;
    private void OnContentChanged()
    {
      if (ContentChanged!=null) ContentChanged(this, new EventArgs());
    }

    public int Count {
      get {
        lock (list) {
          return list.Count;
        }
      }
    }
    public bool IsReadOnly { get { return false; } }

    public void Add(Content item)
    {
      bool added = false;
      lock (list) {
        try {
          list.Add(new ContentKey(item.Stream, item.Timestamp, item.Position), item);
          added = true;
        }
        catch (ArgumentException) {}
        while (list.Count>LimitPackets && list.Count>1) {
          list.RemoveAt(0);
        }
      }
      if (added) {
        OnContentChanged();
      }
    }

    public void Clear()
    {
      lock (list) {
        list.Clear();
      }
      OnContentChanged();
    }

    public bool Contains(Content item)
    {
      lock (list) {
        return list.ContainsKey(new ContentKey(item.Stream, item.Timestamp, item.Position));
      }
    }

    public void CopyTo(Content[] array, int arrayIndex)
    {
      lock (list) {
        list.Values.CopyTo(array, arrayIndex);
      }
    }

    public bool Remove(Content item)
    {
      bool res;
      lock (list) {
        res = list.Remove(new ContentKey(item.Stream, item.Timestamp, item.Position));
      }
      if (res) {
        OnContentChanged();
        return true;
      }
      else {
        return false;
      }
    }

    IEnumerator<Content> IEnumerable<Content>.GetEnumerator()
    {
      return list.Values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return list.Values.GetEnumerator();
    }

    public Content GetOldest(int stream)
    {
      lock (list) {
        return list.Values.Where(c => c.Stream>=stream).FirstOrDefault();
      }
    }

    public Content GetNewest(int stream)
    {
      lock (list) {
        return list.Values.Where(c => c.Stream>=stream).LastOrDefault();
      }
    }

    public Content Newest
    {
      get {
        return list.Values.LastOrDefault();
      }
    }

    public Content Oldest
    {
      get {
        return list.Values.FirstOrDefault();
      }
    }

    public IList<Content> GetNewerContents(int stream, TimeSpan t, long position)
    {
      lock (list) {
        return list.Values.Where(c =>
          c.Stream>=stream &&
          (c.Timestamp>t || (c.Timestamp==t && c.Position>position))).ToArray();
      }
    }

    public Content NextOf(int stream, TimeSpan t, long position)
    {
      lock (list) {
        return list.Values.Where(c =>
          c.Stream>=stream &&
          (c.Timestamp>t || (c.Timestamp==t && c.Position>position))).FirstOrDefault();
      }
    }

    public Content NextOf(Content item)
    {
      return NextOf(item.Stream, item.Timestamp, item.Position);
    }

    public Content FindNextByPosition(int stream, long pos)
    {
      lock (list) {
        return list.Values.Where(c => c.Stream>=stream && pos<c.Position).FirstOrDefault();
      }
    }

  }
}
