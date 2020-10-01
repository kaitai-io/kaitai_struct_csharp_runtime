using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
  public class StreamReadUnSignedAsyncTests : ReadUnSignedAsyncTests
  {
    public StreamReadUnSignedAsyncTests() : base(false)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class StreamReadUnSignedAsyncCancelledTests : ReadUnSignedAsyncTests
  {
    public StreamReadUnSignedAsyncCancelledTests() : base(true)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderReadUnSignedAsyncTests : ReadUnSignedAsyncTests
  {
    public PipeReaderReadUnSignedAsyncTests() : base(false)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(PipeReader.Create(new MemoryStream(data)));
  }

  public class PipeReaderReadUnSignedAsyncCancelledTests : ReadUnSignedAsyncTests
  {
    public PipeReaderReadUnSignedAsyncCancelledTests() : base(true)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(PipeReader.Create(new MemoryStream(data)));
  }


  public abstract class ReadUnSignedAsyncTests : CancelableTestsBase
  {
    protected ReadUnSignedAsyncTests(bool isTestingCancellation) : base(isTestingCancellation)
    {
    }

    protected abstract KaitaiAsyncStream Create(byte[] data);

    [Theory]
    [MemberData(nameof(IntegralData.Integral1Data), MemberType = typeof(IntegralData))]
    public async Task ReadU1Async_Test( /*u*/ sbyte expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () => Assert.Equal((byte) expected, await kaitaiStreamSUT.ReadU1Async(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral2Data), MemberType = typeof(IntegralData))]
    public async Task ReadU2beAsync_Test( /*u*/ short expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () =>
        Assert.Equal((ushort) expected, await kaitaiStreamSUT.ReadU2beAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral4Data), MemberType = typeof(IntegralData))]
    public async Task ReadU4beAsync_Test( /*u*/ int expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () =>
        Assert.Equal((uint) expected, await kaitaiStreamSUT.ReadU4beAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral8Data), MemberType = typeof(IntegralData))]
    public async Task ReadU8beAsync_Test( /*u*/ long expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(
        async () => Assert.Equal((ulong) expected, await kaitaiStreamSUT.ReadU8beAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral2Data), MemberType = typeof(IntegralData))]
    public async Task ReadU2leAsync_Test( /*u*/ short expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      await Evaluate(async () =>
        Assert.Equal((ushort) expected, await kaitaiStreamSUT.ReadU2leAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral4Data), MemberType = typeof(IntegralData))]
    public async Task ReadU4leAsync_Test( /*u*/ int expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      await Evaluate(async () =>
        Assert.Equal((uint) expected, await kaitaiStreamSUT.ReadU4leAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral8Data), MemberType = typeof(IntegralData))]
    public async Task ReadU8leAsync_Test( /*u*/ long expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      await Evaluate(
        async () => Assert.Equal((ulong) expected, await kaitaiStreamSUT.ReadU8leAsync(CancellationToken)));
    }
  }
}