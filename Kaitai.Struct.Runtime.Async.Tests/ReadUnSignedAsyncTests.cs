﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
  public class StreamReadUnSignedAsyncTests : ReadUnSignedAsyncTests
  {
    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderReadUnSignedAsyncTests : ReadUnSignedAsyncTests
  {
    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(System.IO.Pipelines.PipeReader.Create(new MemoryStream(data)));
  }

  public abstract class ReadUnSignedAsyncTests
  {
    protected abstract KaitaiAsyncStream Create(byte[] data);

    [Theory]
    [MemberData(nameof(IntegralData.Integral1Data), MemberType = typeof(IntegralData))]
    public async Task ReadU1Async_Test( /*u*/ sbyte expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      Assert.Equal((byte) expected, await kaitaiStreamSUT.ReadU1Async());
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral2Data), MemberType = typeof(IntegralData))]
    public async Task ReadU2beAsync_Test( /*u*/ short expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      Assert.Equal((ushort) expected, await kaitaiStreamSUT.ReadU2beAsync());
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral4Data), MemberType = typeof(IntegralData))]
    public async Task ReadU4beAsync_Test( /*u*/ int expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      Assert.Equal((uint) expected, await kaitaiStreamSUT.ReadU4beAsync());
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral8Data), MemberType = typeof(IntegralData))]
    public async Task ReadU8beAsync_Test( /*u*/ long expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      Assert.Equal((ulong) expected, await kaitaiStreamSUT.ReadU8beAsync());
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral2Data), MemberType = typeof(IntegralData))]
    public async Task ReadU2leAsync_Test( /*u*/ short expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      Assert.Equal((ushort) expected, await kaitaiStreamSUT.ReadU2leAsync());
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral4Data), MemberType = typeof(IntegralData))]
    public async Task ReadU4leAsync_Test( /*u*/ int expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      Assert.Equal((uint) expected, await kaitaiStreamSUT.ReadU4leAsync());
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral8Data), MemberType = typeof(IntegralData))]
    public async Task ReadU8leAsync_Test( /*u*/ long expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      Assert.Equal((ulong) expected, await kaitaiStreamSUT.ReadU8leAsync());
    }
  }
}