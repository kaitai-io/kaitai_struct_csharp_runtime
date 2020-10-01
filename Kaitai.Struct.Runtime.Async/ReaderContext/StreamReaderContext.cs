using System;
using System.IO;
using System.Threading;
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

    public virtual async ValueTask<long> GetSizeAsync(CancellationToken cancellationToken = default)
    {
      await CheckIsCancellationRequested(cancellationToken);

            return _baseStream.Length;
    }

    public virtual async ValueTask<bool> IsEofAsync(CancellationToken cancellationToken = default)
    {
      await CheckIsCancellationRequested(cancellationToken);

            return _baseStream.Position >= _baseStream.Length;
    }

    public virtual async ValueTask SeekAsync(long position, CancellationToken cancellationToken = default)
    {
      await CheckIsCancellationRequested(cancellationToken);

      _baseStream.Seek(position, SeekOrigin.Begin);
    }

    public virtual async ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default) =>
      (byte) await AsyncBinaryReader.ReadSByteAsync(cancellationToken);

    public virtual async ValueTask<byte[]> ReadBytesAsync(long count, CancellationToken cancellationToken = default)
    {
      if (count < 0 || count > int.MaxValue)
      {
        throw new ArgumentOutOfRangeException(
          $"requested {count} bytes, while only non-negative int32 amount of bytes possible");
      }

      await CheckIsCancellationRequested(cancellationToken);

      var bytes = await AsyncBinaryReader.ReadBytesAsync((int) count, cancellationToken);
      if (bytes.Length < count)
      {
        throw new EndOfStreamException($"requested {count} bytes, but got only {bytes.Length} bytes");
      }

      return bytes;
    }

    public virtual async ValueTask<byte[]> ReadBytesFullAsync(CancellationToken cancellationToken = default) =>
      await ReadBytesAsync(_baseStream.Length - _baseStream.Position, cancellationToken);

    private static async Task CheckIsCancellationRequested(CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
        await Task.FromCanceled(cancellationToken);
      }
    }
  }
}