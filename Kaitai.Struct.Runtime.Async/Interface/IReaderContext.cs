using System.Threading.Tasks;

namespace Kaitai.Async
{
  public interface IReaderContext
  {
    long Position { get; }
    ValueTask<long> GetSizeAsync();
    ValueTask<bool> IsEofAsync();
    ValueTask SeekAsync(long position);
    ValueTask<byte> ReadByteAsync();
    ValueTask<byte[]> ReadBytesAsync(long count);
    ValueTask<byte[]> ReadBytesFullAsync();
  }
}