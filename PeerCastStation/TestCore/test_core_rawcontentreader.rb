# PeerCastStation, a P2P streaming servent.
# Copyright (C) 2011 Ryuichi Sakamoto (kumaryu@kumaryu.net)
# 
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
# 
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
# 
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.
$: << File.join(File.dirname(__FILE__), '..', 'PeerCastStation.Core', 'bin', 'Debug')
require 'PeerCastStation.Core.dll'
require 'test/unit'
PCSCore = PeerCastStation::Core unless defined?(PCSCore)
using_clr_extensions PeerCastStation::Core

class TC_CoreRawContentReader < Test::Unit::TestCase
  def setup
    @peercast = PeerCastStation::Core::PeerCast.new
    @channel = PCSCore::Channel.new(
      @peercast,
      System::Guid.new('9778E62BDC59DF56F9216D0387F80BF2'.to_clr_string), 
      System::Uri.new('http://127.0.0.1:8888/'))
  end
  
  def teardown
    @peercast.stop if @peercast
  end

  def test_construct
    reader = nil
    assert_nothing_raised do
      reader = PCSCore::RawContentReader.new
    end
    assert_equal("RAW", reader.name)
    assert(reader.respond_to?(:create_obj_ref))
  end

  def test_read_empty
    stream = System::IO::MemoryStream.new
    reader = PCSCore::RawContentReader.new
    assert_raises(System::IO::EndOfStreamException) do
      content = reader.read(@channel, stream)
    end
  end

  def test_read
    stream = System::IO::MemoryStream.new("header\ncontent1\ncontent2\n")
    reader = PCSCore::RawContentReader.new
    chan_info = PCSCore::AtomCollection.new
    chan_info.set_chan_info_name('foobar')
    @channel.channel_info = PCSCore::ChannelInfo.new(chan_info)
    content = reader.read(@channel, stream)
    assert_nil(content.channel_track)
    assert_equal('RAW',    content.channel_info.content_type)
    assert_equal('foobar', content.channel_info.name)
    assert_equal(0, content.content_header.position)
    assert_equal(0, content.content_header.data.length)
    assert_equal(1, content.contents.count)
    assert_equal("header\ncontent1\ncontent2\n", content.contents[0].data.to_a.pack('C*'))
  end

  def test_read_many
    stream = System::IO::MemoryStream.new
    data = Array.new(10000) {|i| i%256 }.pack('C*')
    stream.write(data, 0, data.bytesize)
    stream.position = 0
    reader = PCSCore::RawContentReader.new
    content = reader.read(@channel, stream)
    assert_nil(content.channel_track)
    assert_equal('RAW', content.channel_info.content_type)
    assert_equal(0,          content.content_header.position)
    assert_equal(0,          content.content_header.data.length)
    assert_equal(2,          content.contents.count)
    assert_equal(0,          content.contents[0].position)
    assert_equal(8192,       content.contents[0].data.length)
    assert_equal(8192,       content.contents[1].position)
    assert_equal(10000-8192, content.contents[1].data.length)
  end

  def test_read_continue
    stream = System::IO::MemoryStream.new
    data = Array.new(10000) {|i| i%256 }.pack('C*')
    reader = PCSCore::RawContentReader.new
    @channel.content_header = PCSCore::Content.new(30000, 'header')
    @channel.contents.add(PCSCore::Content.new(13093, 'foobar'))
    stream.write(data, 0, data.bytesize)
    stream.position = 0
    content = reader.read(@channel, stream)

    assert_nil(content.channel_info)
    assert_nil(content.channel_track)
    assert_nil(content.content_header)
    assert_equal(2, content.contents.count)
    assert_equal(30006,      content.contents[0].position)
    assert_equal(8192,       content.contents[0].data.length)
    assert_equal(30006+8192, content.contents[1].position)
    assert_equal(10000-8192, content.contents[1].data.length)
  end
end

