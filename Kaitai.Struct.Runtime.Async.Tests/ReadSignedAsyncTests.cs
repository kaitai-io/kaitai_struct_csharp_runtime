using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
  public class StreamReadSignedAsyncTests : ReadSignedAsyncTests
  {
    public StreamReadSignedAsyncTests() : base(false)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderReadSignedAsyncTests : ReadSignedAsyncTests
  {
    public PipeReaderReadSignedAsyncTests() : base(false)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(PipeReader.Create(new MemoryStream(data)));
  }

  public class StreamReadSignedAsyncCancelledTests : ReadSignedAsyncTests
  {
    public StreamReadSignedAsyncCancelledTests() : base(true)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderReadSignedAsyncCancelledTests : ReadSignedAsyncTests
  {
    public PipeReaderReadSignedAsyncCancelledTests() : base(true)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(PipeReader.Create(new MemoryStream(data)));
  }

  public abstract class ReadSignedAsyncTests : CancelableTestsBase
  {
    protected ReadSignedAsyncTests(bool isTestingCancellation) : base(isTestingCancellation)
    {
    }

    protected abstract KaitaiAsyncStream Create(byte[] data);

    [Theory]
    [MemberData(nameof(IntegralData.Integral1Data), MemberType = typeof(IntegralData))]
    public async Task ReadS1Async_Test(sbyte expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async ()=>Assert.Equal(expected, await kaitaiStreamSUT.ReadS1Async(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral2Data), MemberType = typeof(IntegralData))]
    public async Task ReadS2beAsync_Test(short expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadS2beAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral4Data), MemberType = typeof(IntegralData))]
    public async Task ReadS4beAsync_Test(int expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadS4beAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral8Data), MemberType = typeof(IntegralData))]
    public async Task ReadS8beAsync_Test(long expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadS8beAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral2Data), MemberType = typeof(IntegralData))]
    public async Task ReadS2leAsync_Test(short expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadS2leAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral4Data), MemberType = typeof(IntegralData))]
    public async Task ReadS4leAsync_Test(int expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadS4leAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(IntegralData.Integral8Data), MemberType = typeof(IntegralData))]
    public async Task ReadS8leAsync_Test(long expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadS8leAsync(CancellationToken)));
    }
  }
}