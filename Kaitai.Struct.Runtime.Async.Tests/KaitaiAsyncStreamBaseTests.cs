using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
  public class StreamKaitaiAsyncStreamBaseTests : KaitaiAsyncStreamBaseTests
  {
    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderKaitaiAsyncStreamBaseTests : KaitaiAsyncStreamBaseTests
  {
    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(PipeReader.Create(new MemoryStream(data)));
  }

  public abstract class KaitaiAsyncStreamBaseTests
  {
    protected abstract KaitaiAsyncStream Create(byte[] data);

    [Theory]
    [InlineData(true, 0, 0)]
    [InlineData(false, 1, 0)]
    [InlineData(false, 1, 1)]
    [InlineData(false, 1, 2)]
    [InlineData(false, 1, 3)]
    [InlineData(false, 1, 4)]
    [InlineData(false, 1, 5)]
    [InlineData(false, 1, 6)]
    [InlineData(false, 1, 7)]
    [InlineData(true, 1, 8)]
    public async Task Eof_Test(bool shouldBeEof, int streamSize, int readBitsAmount)
    {
      var kaitaiStreamSUT = Create(new byte[streamSize]);
      await kaitaiStreamSUT.ReadBitsIntAsync(readBitsAmount);
      long positionBeforeIsEof = kaitaiStreamSUT.Pos;

      if (shouldBeEof)
      {
        Assert.True(kaitaiStreamSUT.IsEof);
      }
      else
      {
        Assert.False(kaitaiStreamSUT.IsEof);
      }

      Assert.Equal(positionBeforeIsEof, kaitaiStreamSUT.Pos);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public async Task Pos_ByRead_Test(int expectedPos, int readBitsAmount)
    {
      var kaitaiStreamSUT = Create(new byte[1]);

      await kaitaiStreamSUT.ReadBytesAsync(readBitsAmount);

      Assert.Equal(expectedPos, kaitaiStreamSUT.Pos);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    public async Task Pos_BySeek_Test(int expectedPos, int position)
    {
      var kaitaiStreamSUT = Create(new byte[1]);

      await kaitaiStreamSUT.SeekAsync(position);

      Assert.Equal(expectedPos, kaitaiStreamSUT.Pos);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1_000)]
    [InlineData(10_000)]
    [InlineData(100_000)]
    [InlineData(1_000_000)]
    public void Size_Test(int streamSize)
    {
      var kaitaiStreamSUT = Create(new byte[streamSize]);
      long positionBeforeIsEof = kaitaiStreamSUT.Pos;

      Assert.Equal(streamSize, kaitaiStreamSUT.Size);
      Assert.Equal(positionBeforeIsEof, kaitaiStreamSUT.Pos);
    }

    [Fact]
    public async Task AlignToByte_Test()
    {
      //Arrange
      var kaitaiStreamSUT = Create(new byte[] {0b_1000_0000});

      ulong read = await kaitaiStreamSUT.ReadBitsIntAsync(1);
      Assert.Equal(1u, read);

      //Act
      kaitaiStreamSUT.AlignToByte();
      //Assert
      Assert.Equal(1, kaitaiStreamSUT.Pos);
    }

    [Fact]
    public void EmptyStream_NoRead_NoSeek_IsEof_ShouldBe_True()
    {
      var kaitaiStreamSUT = Create(new byte[0]);

      Assert.True(kaitaiStreamSUT.IsEof);
    }

    [Fact]
    public void EmptyStream_NoRead_NoSeek_Pos_ShouldBe_0()
    {
      var kaitaiStreamSUT = Create(new byte[0]);

      Assert.Equal(0, kaitaiStreamSUT.Pos);
    }
  }
}