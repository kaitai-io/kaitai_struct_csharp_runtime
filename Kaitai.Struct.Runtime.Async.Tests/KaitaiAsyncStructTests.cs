using System.IO;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
  public class StreamKaitaiAsyncStructTests : KaitaiAsyncStructTests
  {
    protected override KaitaiAsyncStream Create(byte[] data) => new KaitaiAsyncStream(data);
  }

  public class PipeReaderKaitaiAsyncStructTests : KaitaiAsyncStructTests
  {
    protected override KaitaiAsyncStream Create(byte[] data) =>
      new KaitaiAsyncStream(System.IO.Pipelines.PipeReader.Create(new MemoryStream(data)));
  }

  public abstract class KaitaiAsyncStructTests
  {
    protected abstract KaitaiAsyncStream Create(byte[] data);

    private class FooKaitaiAsyncStruct : KaitaiAsyncStruct
    {
      public FooKaitaiAsyncStruct(KaitaiAsyncStream kaitaiStream) : base(kaitaiStream)
      {
      }
    }

    [Fact]
    public void M_Io_IsSet()
    {
      var kaitaiAsyncStream = Create(new byte[0]);
      var kaitaiAsyncStructSUT = new FooKaitaiAsyncStruct(kaitaiAsyncStream);

      Assert.Equal(kaitaiAsyncStream, kaitaiAsyncStructSUT.M_Io);
    }
  }
}