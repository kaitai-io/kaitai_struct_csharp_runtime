using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
  public class StreamReadDecimalAsyncTests : ReadDecimalAsyncTests
  {
    public StreamReadDecimalAsyncTests() : base(false)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderReadDecimalAsyncTests : ReadDecimalAsyncTests
  {
    public PipeReaderReadDecimalAsyncTests() : base(false)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(PipeReader.Create(new MemoryStream(data)));
  }

  public class StreamReadDecimalAsyncCancelledTests : ReadDecimalAsyncTests
  {
    public StreamReadDecimalAsyncCancelledTests() : base(true)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderReadDecimalAsyncCancelledTests : ReadDecimalAsyncTests
  {
    public PipeReaderReadDecimalAsyncCancelledTests() : base(true)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(PipeReader.Create(new MemoryStream(data)));
  }

  public abstract class ReadDecimalAsyncTests : CancelableTestsBase
  {
    protected ReadDecimalAsyncTests(bool isTestingCancellation) : base(isTestingCancellation)
    {
    }

    protected abstract KaitaiAsyncStream Create(byte[] data);

    [Theory]
    [MemberData(nameof(DecimalData.Decimal4Data), MemberType = typeof(DecimalData))]
    public async Task ReadF4beAsync_Test(float expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadF4beAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(DecimalData.Decimal4Data), MemberType = typeof(DecimalData))]
    public async Task ReadF4leAsync_Test(float expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadF4leAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(DecimalData.Decimal8Data), MemberType = typeof(DecimalData))]
    public async Task ReadF8beAsync_Test(double expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent.Reverse().ToArray());

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadF8beAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(DecimalData.Decimal8Data), MemberType = typeof(DecimalData))]
    public async Task ReadF8leAsync_Test(double expected, byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () => Assert.Equal(expected, await kaitaiStreamSUT.ReadF8leAsync(CancellationToken)));
    }
  }
}