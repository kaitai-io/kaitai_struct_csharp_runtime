using System.Threading;
using System.Threading.Tasks;

namespace Kaitai.Async
{
  public interface IReaderContext
  {
    long Position { get; }
    ValueTask<long> GetSizeAsync(CancellationToken cancellationToken = default);
    ValueTask<bool> IsEofAsync(CancellationToken cancellationToken = default);
    ValueTask SeekAsync(long position, CancellationToken cancellationToken = default);
    ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default);
    ValueTask<byte[]> ReadBytesAsync(long count, CancellationToken cancellationToken = default);
    ValueTask<byte[]> ReadBytesFullAsync(CancellationToken cancellationToken = default);
  }
}