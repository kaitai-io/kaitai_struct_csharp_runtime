using System;
using System.IO;
using System.Threading.Tasks;
using Overby.Extensions.AsyncBinaryReaderWriter;

namespace Kaitai.Async
{
  public class StreamReaderContext : IReaderContext
  {
    private readonly Stream _baseStream;
    protected readonly AsyncBinaryReader AsyncBinaryReader;

    public StreamReaderContext(Stream stream)
    {
      _baseStream = stream;
      AsyncBinaryReader = new AsyncBinaryReader(_baseStream);
    }

    public long Position => _baseStream.Position;
    public virtual ValueTask<long> GetSizeAsync() => new ValueTask<long>(_baseStream.Length);

    public virtual ValueTask<bool> IsEofAsync() =>
      new ValueTask<bool>(_baseStream.Position >= _baseStream.Length);

    public virtual ValueTask SeekAsync(long position)
    {
      _baseStream.Seek(position, SeekOrigin.Begin);
      return new ValueTask();
    }

    public virtual async ValueTask<byte> ReadByteAsync() => (byte) await AsyncBinaryReader.ReadSByteAsync();

    public virtual async ValueTask<byte[]> ReadBytesAsync(long count)
    {
      if (count < 0 || count > int.MaxValue)
      {
        throw new ArgumentOutOfRangeException(
          $"requested {count} bytes, while only non-negative int32 amount of bytes possible");
      }

      var bytes = await AsyncBinaryReader.ReadBytesAsync((int) count);
      if (bytes.Length < count)
      {
        throw new EndOfStreamException($"requested {count} bytes, but got only {bytes.Length} bytes");
      }

      return bytes;
    }

    public virtual async ValueTask<byte[]> ReadBytesFullAsync() =>
      await ReadBytesAsync(_baseStream.Length - _baseStream.Position);
  }
}