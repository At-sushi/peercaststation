$: << File.join(File.dirname(__FILE__), '..', 'PeerCastStation.Core', 'bin', 'Debug')
require 'PeerCastStation.Core.dll'
require 'test/unit'

PCSCore = PeerCastStation::Core

class TC_HTTPRequest < Test::Unit::TestCase
  def test_construct
    value = System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
      'Connection:close',
      'User-Agent:hoge hoge',
    ])
    req = PCSCore::HTTPRequest.new(value)
    assert_equal('GET', req.Method)
    assert_kind_of(System::Uri, req.uri)
    assert(req.uri.is_absolute_uri)
    assert_equal('http',      req.uri.scheme)
    assert_equal('localhost', req.uri.host)
    assert_equal('/stream/9778E62BDC59DF56F9216D0387F80BF2.wmv', req.uri.absolute_path)
  end
end

class TC_HTTPRequestReader < Test::Unit::TestCase
  def test_read
    stream = System::IO::MemoryStream.new([
      "GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1\r\n",
      "Connection:close\r\n",
      "User-Agent:hoge hoge\r\n",
      "\r\n"
    ].join)
    req = nil
    assert_nothing_raised {
      req = PCSCore::HTTPRequestReader.read(stream)
    }
    assert_kind_of(PCSCore::HTTPRequest, req)
    assert_equal('GET', req.Method)
    assert_kind_of(System::Uri, req.uri)
    assert(req.uri.is_absolute_uri)
    assert_equal('http',      req.uri.scheme)
    assert_equal('localhost', req.uri.host)
    assert_equal('/stream/9778E62BDC59DF56F9216D0387F80BF2.wmv', req.uri.absolute_path)
  end

  def test_read_failed
    stream = System::IO::MemoryStream.new([
      "GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1\r\n",
    ].join)
    assert_raise(System::IO::EndOfStreamException) {
      PCSCore::HTTPRequestReader.read(stream)
    }
  end
end

class TC_HTTPOutputStreamFactory < Test::Unit::TestCase
  def setup
    endpoint = System::Net::IPEndPoint.new(System::Net::IPAddress.any, 7147)
    @peercast = PeerCastStation::Core::PeerCast.new(endpoint)
  end

  def teardown
    @peercast.close if @peercast and not @peercast.is_closed
  end

  def test_construct
    factory = PCSCore::HTTPOutputStreamFactory.new(@peercast)
    assert_equal('HTTP', factory.Name)
  end

  def test_parse_channel_id
    factory = PCSCore::HTTPOutputStreamFactory.new(@peercast)
    channel_id = factory.ParseChannelID([
      "GET /html/ja/index.html HTTP/1.1\r\n",
      "Connection:close\r\n",
      "User-Agent:hoge hoge\r\n",
      "\r\n"
    ].join)
    assert_nil(channel_id)
    channel_id = factory.ParseChannelID([
      "GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1\r\n",
      "Connection:close\r\n",
      "User-Agent:hoge hoge\r\n",
    ].join)
    assert_nil(channel_id)
    channel_id = factory.ParseChannelID([
      "GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1\r\n",
      "Connection:close\r\n",
      "User-Agent:hoge hoge\r\n",
      "\r\n"
    ].join)
    assert_equal(System::Guid.new('9778E62BDC59DF56F9216D0387F80BF2'.to_clr_string), channel_id)
    channel_id = factory.ParseChannelID([
      "POST /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1\r\n",
      "Connection:close\r\n",
      "User-Agent:hoge hoge\r\n",
      "\r\n"
    ].join)
    assert_nil(channel_id)
  end

  def test_create
    channel = PCSCore::Channel.new(
      System::Guid.new('9778E62BDC59DF56F9216D0387F80BF2'.to_clr_string), 
      System::Uri.new('http://localhost:7147/'))
    factory = PCSCore::HTTPOutputStreamFactory.new(@peercast)
    stream = System::IO::MemoryStream.new('hogehoge')
    header = [
      "GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1\r\n",
      "Connection:close\r\n",
      "User-Agent:hoge hoge\r\n",
      "\r\n"
    ].join
    output_stream = factory.create(stream, channel, header)
    assert_not_nil(output_stream)
    assert_equal(@peercast, output_stream.PeerCast)
    assert_equal(stream, output_stream.stream)
    assert_equal(channel, output_stream.channel)

    header = [
      "GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1\r\n",
    ].join
    output_stream = factory.create(stream, channel, header)
    assert_nil(output_stream)
  end
end

class TC_HTTPOutputStream < Test::Unit::TestCase
  class HTTPOutputStream < PCSCore::HTTPOutputStream 
  end

  class TestHTTPOutputStream < PCSCore::HTTPOutputStream 
    def self.new(*args)
      inst = super
      inst.instance_eval do
        @body_type = PCSCore::HTTPOutputStream::BodyType.none 
        @write_enabled = true
      end
      inst
    end
    attr_accessor :body_type, :write_enabled

    def get_body_type
      @body_type
    end

    def write_bytes(bytes)
      if @write_enabled then
        super
      else
        false
      end
    end
  end

  def setup
    endpoint = System::Net::IPEndPoint.new(System::Net::IPAddress.any, 7147)
    @peercast = PeerCastStation::Core::PeerCast.new(endpoint)
    @channel = PCSCore::Channel.new(
      System::Guid.new('9778E62BDC59DF56F9216D0387F80BF2'.to_clr_string), 
      System::Uri.new('http://localhost:7147/'))
  end

  def teardown
    @peercast.close if @peercast and not @peercast.is_closed
  end

  def test_construct
    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
      'Connection:close',
      'User-Agent:hoge hoge',
    ]))
    s = System::IO::MemoryStream.new
    stream = PCSCore::HTTPOutputStream.new(@peercast, s, @channel, req)
    assert_equal(@peercast,stream.PeerCast)
    assert_equal(@channel, stream.channel)
    assert_equal(s,        stream.stream)
    assert_equal(PCSCore::OutputStreamType.play, stream.output_stream_type)
    assert(!stream.is_closed)
  end

  def test_get_body_type
    s = System::IO::MemoryStream.new

    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = HTTPOutputStream.new(@peercast, s, @channel, req)
    assert_equal(PCSCore::HTTPOutputStream::BodyType.content, stream.get_body_type)

    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /pls/9778E62BDC59DF56F9216D0387F80BF2.pls HTTP/1.1',
    ]))
    stream = HTTPOutputStream.new(@peercast, s, @channel, req)
    assert_equal(PCSCore::HTTPOutputStream::BodyType.playlist, stream.get_body_type)

    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /index.html HTTP/1.1',
    ]))
    stream = HTTPOutputStream.new(@peercast, s, @channel, req)
    assert_equal(PCSCore::HTTPOutputStream::BodyType.none, stream.get_body_type)

    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = HTTPOutputStream.new(@peercast, s, nil, req)
    assert_equal(PCSCore::HTTPOutputStream::BodyType.none, stream.get_body_type)
  end

  def test_create_response_header
    s = System::IO::MemoryStream.new

    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = TestHTTPOutputStream.new(@peercast, s, @channel, req)
    stream.body_type = PCSCore::HTTPOutputStream::BodyType.none
    head = stream.create_response_header.split(/\r\n/)
    assert_match(%r;^HTTP/1.0 404 ;, head[0])

    stream.body_type = PCSCore::HTTPOutputStream::BodyType.playlist
    head = stream.create_response_header.split(/\r\n/)
    assert_match(%r;^HTTP/1.0 404 ;, head[0])

    stream.body_type = PCSCore::HTTPOutputStream::BodyType.content
    @channel.channel_info.content_type = 'OGG'
    head = stream.create_response_header.split(/\r\n/)
    assert_match(%r;^HTTP/1.0 200 ;, head[0])
    assert(head.any? {|line| /^Content-Type:\s*#{@channel.channel_info.MIMEType}/=~line})

    stream.body_type = PCSCore::HTTPOutputStream::BodyType.content
    ['WMV', 'WMA', 'ASX'].each do |mms_type|
      @channel.channel_info.content_type = mms_type
      head = stream.create_response_header.split(/\r\n/)
      assert_match(%r;^HTTP/1.0 200 ;, head[0])
      assert(head.any? {|line| /^Content-Type:\s*application\/x-mms-framed/=~line})
      assert(head.any? {|line| /^Server:\s*Rex\/9\.0\.\d+/=~line})
    end
  end

  def test_write_response_header
    s = System::IO::MemoryStream.new
    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = TestHTTPOutputStream.new(@peercast, s, @channel, req)
    stream.body_type = PCSCore::HTTPOutputStream::BodyType.content
    @channel.channel_info.content_type = 'OGG'
    stream.write_response_header
    assert_equal(stream.create_response_header+"\r\n", s.to_array.to_a.pack('C*'))
  end

  def test_write_content_header
    s = System::IO::MemoryStream.new
    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = TestHTTPOutputStream.new(@peercast, s, @channel, req)

    @channel.content_header = nil
    s.position = 0
    assert(!stream.write_content_header)
    assert_equal(0, s.position)
    assert(!stream.is_closed)

    @channel.content_header = PCSCore::Content.new(0, 'header')
    s.position = 0
    assert(stream.write_content_header)
    assert_equal('header'.size, s.position)
    assert(!stream.is_closed)

    stream.write_enabled = false
    s.position = 0
    assert(!stream.write_content_header)
    assert_equal(0, s.position)
    assert(stream.is_closed)
  end

  def test_write_content
    s = System::IO::MemoryStream.new
    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = TestHTTPOutputStream.new(@peercast, s, @channel, req)

    @channel.contents.clear
    s.position = 0
    assert_equal(-1, stream.write_content(-1))
    assert_equal(0, s.position)
    assert(!stream.is_closed)

    @channel.contents.add(PCSCore::Content.new(0, 'content0'))
    s.position = 0
    assert_equal(0, stream.write_content(-1))
    assert_equal('content0'.size, s.position)
    assert(!stream.is_closed)

    @channel.contents.add(PCSCore::Content.new( 7, 'content1'))
    @channel.contents.add(PCSCore::Content.new(14, 'content2'))
    @channel.contents.add(PCSCore::Content.new(21, 'content3'))
    @channel.contents.add(PCSCore::Content.new(28, 'content4'))
    s.position = 0; s.set_length(0)
    assert_equal(7, stream.write_content(0))
    assert_equal('content1', s.to_array.to_a.pack('C*'))
    assert(!stream.is_closed)

    s.position = 0; s.set_length(0)
    assert_equal(14, stream.write_content(7))
    assert_equal('content2', s.to_array.to_a.pack('C*'))
    assert(!stream.is_closed)

    s.position = 0; s.set_length(0)
    assert_equal(21, stream.write_content(14))
    assert_equal('content3', s.to_array.to_a.pack('C*'))
    assert(!stream.is_closed)

    s.position = 0; s.set_length(0)
    assert_equal(28, stream.write_content(21))
    assert_equal('content4', s.to_array.to_a.pack('C*'))
    assert(!stream.is_closed)

    s.position = 0; s.set_length(0)
    assert_equal(28, stream.write_content(28))
    assert_equal(0, s.position)
    assert(!stream.is_closed)

    stream.write_enabled = false
    s.position = 0; s.set_length(0)
    assert_equal(-1, stream.write_content(-1))
    assert_equal(0, s.position)
    assert(stream.is_closed)
  end

  def test_post
    s = System::IO::MemoryStream.new
    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = TestHTTPOutputStream.new(@peercast, s, @channel, req)
    stream.post(@peercast.host, PCSCore::Atom.new(PCSCore::Atom.PCP_HELO, 1))
    assert_equal(0, s.position)
  end

  def test_close
    s = System::IO::MemoryStream.new
    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = TestHTTPOutputStream.new(@peercast, s, @channel, req)
    stream.close
    assert(stream.is_closed)
    assert(!s.can_read)
  end

  def test_write_bytes
    s = System::IO::MemoryStream.new
    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = HTTPOutputStream.new(@peercast, s, @channel, req)
    assert(stream.write_bytes('hogehoge'))
    assert_equal('hogehoge', s.to_array.to_a.pack('C*'))

    s.close
    assert(!stream.write_bytes('hogehoge'))
  end

  def test_write_response_body
    s = System::IO::MemoryStream.new
    req = PCSCore::HTTPRequest.new(System::Array[System::String].new([
      'GET /stream/9778E62BDC59DF56F9216D0387F80BF2.wmv HTTP/1.1',
    ]))
    stream = TestHTTPOutputStream.new(@peercast, s, @channel, req)
    stream.body_type = PCSCore::HTTPOutputStream::BodyType.none 
    stream.write_response_body
    assert_equal(0, s.position)

    stream.body_type = PCSCore::HTTPOutputStream::BodyType.playlist 
    stream.write_response_body
    assert_equal(0, s.position)

    stream.body_type = PCSCore::HTTPOutputStream::BodyType.content 
    @channel.content_header = PCSCore::Content.new(0, 'header')
    @channel.contents.add(PCSCore::Content.new( 6, 'content0'))
    write_thread = Thread.new {
      stream.write_response_body
    }
    sleep(0.1)
    @channel.contents.add(PCSCore::Content.new(13, 'content1'))
    sleep(0.1)
    @channel.contents.add(PCSCore::Content.new(20, 'content2'))
    sleep(0.1)
    @channel.contents.add(PCSCore::Content.new(27, 'content3'))
    sleep(0.1)
    @channel.contents.add(PCSCore::Content.new(34, 'content4'))
    sleep(0.1)
    stream.close
    write_thread.join
    assert_equal('headercontent0content1content2content3content4', s.to_array.to_a.pack('C*'))
  end
end

