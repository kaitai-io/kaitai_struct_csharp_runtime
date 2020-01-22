using System.IO;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
  public class StreamKaitaiReadBitsAsyncTests : ReadBitsAsyncTests
  {
    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderReadBitsAsyncTests : ReadBitsAsyncTests
  {
    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(System.IO.Pipelines.PipeReader.Create(new MemoryStream(data)));
  }

  public abstract class ReadBitsAsyncTests
  {
    protected abstract KaitaiAsyncStream Create(byte[] data);

    [Theory]
    [MemberData(nameof(BitsData.BitsBeData), MemberType = typeof(BitsData))]
    public async Task ReadBitsIntAsync_Test(ulong expected, byte[] streamContent, int bitsCount)
    {
      var kaitaiStreamSUT = Create(streamContent);

      Assert.Equal(expected, await kaitaiStreamSUT.ReadBitsIntAsync(bitsCount));
    }

    [Theory]
    [MemberData(nameof(BitsData.BitsLeData), MemberType = typeof(BitsData))]
    public async Task ReadBitsIntLeAsync_Test(ulong expected, byte[] streamContent, int bitsCount)
    {
      var kaitaiStreamSUT = Create(streamContent);

      Assert.Equal(expected, await kaitaiStreamSUT.ReadBitsIntLeAsync(bitsCount));
    }
  }
}