using System.Threading.Tasks;

namespace Kaitai.Async
{
  public interface IReaderContext
  {
    long Position { get; }
    ValueTask<long> GetSize();
    ValueTask<bool> IsEof();
    ValueTask SeekAsync(long position);
    ValueTask<byte> ReadByteAsync();
    ValueTask<byte[]> ReadBytesAsync(long count);
    ValueTask<byte[]> ReadBytesFullAsync();
  }
}