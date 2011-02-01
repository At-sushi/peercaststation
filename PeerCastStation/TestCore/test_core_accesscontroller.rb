$: << File.join(File.dirname(__FILE__), '..', 'PeerCastStation.Core', 'bin', 'Debug')
require 'PeerCastStation.Core.dll'
require 'test/unit'
using_clr_extensions PeerCastStation::Core

class TC_CoreAccessController < Test::Unit::TestCase
  def setup
    endpoint = System::Net::IPEndPoint.new(System::Net::IPAddress.any, 7147)
    @peercast = PeerCastStation::Core::PeerCast.new(endpoint)
  end
  
  def teardown
    @peercast.close if @peercast and not @peercast.is_closed
  end

  def new_output(type, is_local, bitrate)
    output_stream_type = PeerCastStation::Core::OutputStreamType.play
    case type
    when :play
      output_stream_type = PeerCastStation::Core::OutputStreamType.play
    when :relay
      output_stream_type = PeerCastStation::Core::OutputStreamType.relay
    end
    output = MockOutputStream.new(output_stream_type)
    output.is_local = is_local
    output.upstream_rate = bitrate
    output
  end

  def new_channel(bitrate)
    channel = PeerCastStation::Core::Channel.new(@peercast, System::Guid.empty, System::Uri.new('mock://localhost'))
    channel.channel_info.extra.set_chan_info_bitrate(bitrate)
    @peercast.channels.add(channel)
    channel
  end

  def test_construct
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    assert_equal(@peercast, ac.PeerCast)
    assert_equal(0, ac.max_relays)
    assert_equal(0, ac.max_relays_per_channel)
    assert_equal(0, ac.max_plays)
    assert_equal(0, ac.max_plays_per_channel)
    assert_equal(0, ac.max_upstream_rate)
  end

  def test_is_channel_relayable_empty
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    assert(ac.is_channel_relayable(@channel))
  end

  def test_is_channel_relayable_all_reset
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel = new_channel(7144)
    channel.output_streams.add(new_output(:play, true, 0))
    channel.output_streams.add(new_output(:relay, false, 7144))
    assert(ac.is_channel_relayable(channel))
  end

  def test_is_channel_relayable_max_relays
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel1 = new_channel(7144)
    channel1.output_streams.add(new_output(:play, true, 0))
    channel1.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 1
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 0
    assert(!ac.is_channel_relayable(channel1))

    ac.max_relays             = 2
    ac.max_relays_per_channel = 0
    ac.max_plays              = 1
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 0
    assert(ac.is_channel_relayable(channel1))

    channel2 = new_channel(7144)
    channel2.output_streams.add(new_output(:play, true, 0))
    channel2.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 2
    ac.max_relays_per_channel = 0
    ac.max_plays              = 1
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 0
    assert(!ac.is_channel_relayable(channel1))
    assert(!ac.is_channel_relayable(channel2))
  end

  def test_is_channel_relayable_upstream_rate_max_relay
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel = new_channel(7144)
    channel.output_streams.add(new_output(:play, true, 0))
    channel.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144
    assert(!ac.is_channel_relayable(channel))
  end

  def test_is_channel_relayable_upstream_rate_max_play
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel = new_channel(7144)
    channel.output_streams.add(new_output(:play, false, 7144))
    channel.output_streams.add(new_output(:relay, true, 0))
    ac.max_relays             = 2
    ac.max_relays_per_channel = 0
    ac.max_plays              = 1
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144
    assert(!ac.is_channel_relayable(channel))
  end

  def test_is_channel_relayable_upstream_rate
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel1 = new_channel(7144)
    channel1.output_streams.add(new_output(:play, true, 0))
    channel1.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144*2
    assert(ac.is_channel_relayable(channel1))

    channel2 = new_channel(7144)
    channel2.output_streams.add(new_output(:play, false, 7144))
    channel2.output_streams.add(new_output(:relay, true, 0))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144*2
    assert(!ac.is_channel_relayable(channel1))
    assert(!ac.is_channel_relayable(channel2))
  end

  def test_is_channel_relayable_upstream_rate_by_output_stream
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel1 = new_channel(7144)
    channel1.output_streams.add(new_output(:play, true, 0))
    channel1.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144
    assert(!ac.is_channel_relayable(channel1))

    os = new_output(:relay, true, 0)
    assert(ac.is_channel_relayable(channel1, os))
  end

  def test_is_channel_relayable_per_channel
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel1 = new_channel(7144)
    channel1.output_streams.add(new_output(:play, true, 0))
    channel1.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 2
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 0
    assert(ac.is_channel_relayable(channel1))

    channel2 = new_channel(7144)
    channel2.output_streams.add(new_output(:play, true, 0))
    channel2.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 2
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 1
    ac.max_upstream_rate      = 0
    assert(ac.is_channel_relayable(channel1))
    assert(ac.is_channel_relayable(channel2))

    channel2.output_streams.add(new_output(:play, true, 0))
    channel2.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 2
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 1
    ac.max_upstream_rate      = 0
    assert(ac.is_channel_relayable(channel1))
    assert(!ac.is_channel_relayable(channel2))
  end

  def test_is_channel_playable
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    assert(ac.is_channel_playable(@channel))
  end

  def test_is_channel_playable_all_reset
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel = new_channel(7144)
    channel.output_streams.add(new_output(:play, true, 0))
    channel.output_streams.add(new_output(:relay, false, 7144))
    assert(ac.is_channel_playable(channel))
  end

  def test_is_channel_playable_max_plays
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel1 = new_channel(7144)
    channel1.output_streams.add(new_output(:play, true, 0))
    channel1.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 1
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 0
    assert(!ac.is_channel_playable(channel1))

    ac.max_relays             = 1
    ac.max_relays_per_channel = 0
    ac.max_plays              = 2
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 0
    assert(ac.is_channel_playable(channel1))

    channel2 = new_channel(7144)
    channel2.output_streams.add(new_output(:play, true, 0))
    channel2.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 1
    ac.max_relays_per_channel = 0
    ac.max_plays              = 2
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 0
    assert(!ac.is_channel_playable(channel1))
    assert(!ac.is_channel_playable(channel2))
  end

  def test_is_channel_playable_upstream_rate_max_relay
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel = new_channel(7144)
    channel.output_streams.add(new_output(:play, true, 0))
    channel.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144
    assert(!ac.is_channel_playable(channel))
  end

  def test_is_channel_relayable_upstream_rate_max_play
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel = new_channel(7144)
    channel.output_streams.add(new_output(:play, false, 7144))
    channel.output_streams.add(new_output(:relay, true, 0))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144
    assert(!ac.is_channel_playable(channel))
  end

  def test_is_channel_playable_upstream_rate
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel1 = new_channel(7144)
    channel1.output_streams.add(new_output(:play, true, 0))
    channel1.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144*2
    assert(ac.is_channel_playable(channel1))

    channel2 = new_channel(7144)
    channel2.output_streams.add(new_output(:play, false, 7144))
    channel2.output_streams.add(new_output(:relay, true, 0))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144*2
    assert(!ac.is_channel_playable(channel1))
    assert(!ac.is_channel_playable(channel2))
  end

  def test_is_channel_playable_upstream_rate_by_output_stream
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel1 = new_channel(7144)
    channel1.output_streams.add(new_output(:play, true, 0))
    channel1.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 0
    ac.max_upstream_rate      = 7144
    assert(!ac.is_channel_playable(channel1))

    os = new_output(:play, true, 0)
    assert(ac.is_channel_playable(channel1, os))
  end

  def test_is_channel_playable_per_channel
    ac = PeerCastStation::Core::AccessController.new(@peercast)
    channel1 = new_channel(7144)
    channel1.output_streams.add(new_output(:play, true, 0))
    channel1.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 0
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 2
    ac.max_upstream_rate      = 0
    assert(ac.is_channel_playable(channel1))

    channel2 = new_channel(7144)
    channel2.output_streams.add(new_output(:play, true, 0))
    channel2.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 1
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 2
    ac.max_upstream_rate      = 0
    assert(ac.is_channel_playable(channel1))
    assert(ac.is_channel_playable(channel2))

    channel2.output_streams.add(new_output(:play, true, 0))
    channel2.output_streams.add(new_output(:relay, false, 7144))
    ac.max_relays             = 0
    ac.max_relays_per_channel = 1
    ac.max_plays              = 0
    ac.max_plays_per_channel  = 2
    ac.max_upstream_rate      = 0
    assert(ac.is_channel_playable(channel1))
    assert(!ac.is_channel_playable(channel2))
  end
end

