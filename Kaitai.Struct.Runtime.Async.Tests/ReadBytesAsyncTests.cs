using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
  public class StreamReadBytesAsyncTests : ReadBytesAsyncTests
  {
    public StreamReadBytesAsyncTests() : base(false)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderReadBytesAsyncTests : ReadBytesAsyncTests
  {
    public PipeReaderReadBytesAsyncTests() : base(false)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(PipeReader.Create(new MemoryStream(data)));
  }

  public class StreamReadBytesAsyncCancelledTests : ReadBytesAsyncTests
  {
    public StreamReadBytesAsyncCancelledTests() : base(true)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderReadBytesAsyncCancelledTests : ReadBytesAsyncTests
  {
    public PipeReaderReadBytesAsyncCancelledTests() : base(true)
    {
    }

    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(PipeReader.Create(new MemoryStream(data)));
  }

  public abstract class ReadBytesAsyncTests : CancelableTestsBase
  {
    protected ReadBytesAsyncTests(bool isTestingCancellation) : base(isTestingCancellation)
    {
    }

    public static IEnumerable<object[]> BytesData =>
      new List<(byte[] streamContent, int bytesCount)>
      {
        (new byte[] {0b_1101_0101}, 0),
        (new byte[] {0b_1101_0101}, 1),
        (new byte[] {0b_1101_0101, 0b_1101_0101}, 1),
        (new byte[] {0b_1101_0101, 0b_1101_0101}, 2)
      }.Select(t => new object[] {t.streamContent, t.bytesCount});

    public static IEnumerable<object[]> StringData =>
      new List<string>
      {
        "",
        "ABC",
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
      }.Select(t => new[] {Encoding.ASCII.GetBytes(t)});

    public static IEnumerable<object[]> StringWithTerminatorsData =>
      new List<(string streamContent, string expected, char terminator, bool isPresent, bool shouldInclude)>
      {
        ("", "", '\0', false, false),
        ("", "", '\0', false, true),

        ("ABC", "ABC", '\0', false, false),
        ("ABC", "ABC", '\0', false, true),

        ("ABC", "", 'A', true, false),
        ("ABC", "A", 'A', true, true),

        ("ABC", "A", 'B', true, false),
        ("ABC", "AB", 'B', true, true),

        ("ABC", "AB", 'C', true, false),
        ("ABC", "ABC", 'C', true, true)
      }.Select(t => new[]
      {
        Encoding.ASCII.GetBytes(t.streamContent), Encoding.ASCII.GetBytes(t.expected), (object) (byte) t.terminator,
        t.isPresent, t.shouldInclude
      });

    protected abstract KaitaiAsyncStream Create(byte[] data);


    [Theory]
    [MemberData(nameof(BytesData))]
    public async Task ReadBytesAsync_long_Test(byte[] streamContent, long bytesCount)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () =>
        Assert.Equal(streamContent.Take((int) bytesCount),
          await kaitaiStreamSUT.ReadBytesAsync(bytesCount, CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(BytesData))]
    public async Task ReadBytesAsync_ulong_Test(byte[] streamContent, ulong bytesCount)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () =>
        Assert.Equal(streamContent.Take((int) bytesCount),
          await kaitaiStreamSUT.ReadBytesAsync(bytesCount, CancellationToken)));
    }


    [Theory]
    [MemberData(nameof(StringData))]
    public async Task ReadBytesFullAsync_Test(byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () =>
        Assert.Equal(streamContent, await kaitaiStreamSUT.ReadBytesFullAsync(CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(StringData))]
    public async Task EnsureFixedContentsAsync_Test(byte[] streamContent)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () =>
        Assert.Equal(streamContent, await kaitaiStreamSUT.EnsureFixedContentsAsync(streamContent, CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(StringData))]
    public async Task EnsureFixedContentsAsync_ThrowsIfByteIsChanged(byte[] streamContent)
    {
      if (streamContent.Length == 0)
      {
        return;
      }

      var kaitaiStreamSUT = Create(streamContent);

      var expected = streamContent.ToArray();
      expected[0] = (byte) ~expected[0];

      await Evaluate<Exception>(async () =>
        await kaitaiStreamSUT.EnsureFixedContentsAsync(expected, CancellationToken));
    }

    [Theory]
    [MemberData(nameof(StringWithTerminatorsData))]
    public async Task ReadBytesTermAsync(byte[] streamContent,
      byte[] expected,
      byte terminator,
      bool _,
      bool shouldInclude)
    {
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () => Assert.Equal(expected,
        await kaitaiStreamSUT.ReadBytesTermAsync(terminator, shouldInclude, false, false, CancellationToken)));
    }

    [Theory]
    [MemberData(nameof(StringWithTerminatorsData))]
    public async Task ReadBytesTermAsync_ThrowsIsTerminatorNotPresent(byte[] streamContent,
      byte[] expected,
      byte terminator,
      bool terminatorIsPresent,
      bool shouldInclude)
    {
      var kaitaiStreamSUT = Create(streamContent);

      if (terminatorIsPresent)
      {
        return;
      }

      await Evaluate<EndOfStreamException>(async () =>
        await kaitaiStreamSUT.ReadBytesTermAsync(terminator, shouldInclude, false, true, CancellationToken));
    }

    [Theory]
    [MemberData(nameof(StringWithTerminatorsData))]
    public async Task ReadBytesTermAsync_ShouldNotConsumeTerminator(byte[] streamContent,
      byte[] expected,
      byte terminator,
      bool terminatorIsPresent,
      bool shouldInclude)
    {
      //Arrange
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () =>
      {
        //Act
        await kaitaiStreamSUT.ReadBytesTermAsync(terminator, shouldInclude, false, false, CancellationToken);

        //Assert
        int amountToConsume = expected.Length;
        if (expected.Length > 0 && shouldInclude && terminatorIsPresent)
        {
          amountToConsume--;
        }

        Assert.Equal(amountToConsume, kaitaiStreamSUT.Pos);
      });
    }

    [Theory]
    [MemberData(nameof(StringWithTerminatorsData))]
    public async Task ReadBytesTermAsync_ShouldConsumeTerminator(byte[] streamContent,
      byte[] expected,
      byte terminator,
      bool terminatorIsPresent,
      bool shouldInclude)
    {
      //Arrange
      var kaitaiStreamSUT = Create(streamContent);

      await Evaluate(async () =>
      {
        //Act
        await kaitaiStreamSUT.ReadBytesTermAsync(terminator, shouldInclude, true, false, CancellationToken);

        //Assert
        int amountToConsume = expected.Length;
        if (!shouldInclude && terminatorIsPresent)
        {
          amountToConsume++;
        }

        Assert.Equal(amountToConsume, kaitaiStreamSUT.Pos);
      });
    }

    [Fact]
    public async Task ReadBytesAsyncLong_LargerThanBufferInvoke_ThrowsArgumentOutOfRangeException()
    {
      var kaitaiStreamSUT = Create(new byte[0]);

      await Evaluate<EndOfStreamException>(async () => await kaitaiStreamSUT.ReadBytesAsync(1, CancellationToken));
    }

    [Fact]
    public async Task ReadBytesAsyncLong_LargerThanInt32Invoke_ThrowsArgumentOutOfRangeException()
    {
      var kaitaiStreamSUT = Create(new byte[0]);

      await Evaluate<ArgumentOutOfRangeException>(async () =>
        await kaitaiStreamSUT.ReadBytesAsync((long) int.MaxValue + 1, CancellationToken));
    }

    [Fact]
    public async Task ReadBytesAsyncLong_NegativeInvoke_ThrowsArgumentOutOfRangeException()
    {
      var kaitaiStreamSUT = Create(new byte[0]);

      await Evaluate<ArgumentOutOfRangeException>(async () =>
        await kaitaiStreamSUT.ReadBytesAsync(-1, CancellationToken));
    }

    [Fact]
    public async Task ReadBytesAsyncULong_LargerThanBufferInvoke_ThrowsArgumentOutOfRangeException()
    {
      var kaitaiStreamSUT = Create(new byte[0]);

      await Evaluate<EndOfStreamException>(async () =>
        await kaitaiStreamSUT.ReadBytesAsync((ulong) 1, CancellationToken));
    }

    [Fact]
    public async Task ReadBytesAsyncULong_LargerThanInt32Invoke_ThrowsArgumentOutOfRangeException()
    {
      var kaitaiStreamSUT = Create(new byte[0]);

      await Evaluate<ArgumentOutOfRangeException>(async () =>
        await kaitaiStreamSUT.ReadBytesAsync((ulong) int.MaxValue + 1, CancellationToken));
    }
  }
}