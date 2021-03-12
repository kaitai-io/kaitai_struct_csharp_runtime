using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kaitai.Async
{
  public interface IKaitaiAsyncStream : IKaitaiStreamBase
  {
    /// <summary>
    /// Check if the stream position is at the end of the stream
    /// </summary>
    ValueTask<bool> IsEofAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the total length of the stream (ie. file size)
    /// </summary>
    ValueTask<long> GetSizeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seek to a specific position from the beginning of the stream
    /// </summary>
    /// <param name="position">The position to seek to</param>
    /// <param name="cancellationToken"></param>
    Task SeekAsync(long position, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a signed byte from the stream
    /// </summary>
    /// <returns></returns>
    Task<sbyte> ReadS1Async(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a signed short from the stream (big endian)
    /// </summary>
    /// <returns></returns>
    Task<short> ReadS2beAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a signed int from the stream (big endian)
    /// </summary>
    /// <returns></returns>
    Task<int> ReadS4beAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a signed long from the stream (big endian)
    /// </summary>
    /// <returns></returns>
    Task<long> ReadS8beAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a signed short from the stream (little endian)
    /// </summary>
    /// <returns></returns>
    Task<short> ReadS2leAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a signed int from the stream (little endian)
    /// </summary>
    /// <returns></returns>
    Task<int> ReadS4leAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a signed long from the stream (little endian)
    /// </summary>
    /// <returns></returns>
    Task<long> ReadS8leAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read an unsigned byte from the stream
    /// </summary>
    /// <returns></returns>
    Task<byte> ReadU1Async(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read an unsigned short from the stream (big endian)
    /// </summary>
    /// <returns></returns>
    Task<ushort> ReadU2beAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read an unsigned int from the stream (big endian)
    /// </summary>
    /// <returns></returns>
    Task<uint> ReadU4beAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read an unsigned long from the stream (big endian)
    /// </summary>
    /// <returns></returns>
    Task<ulong> ReadU8beAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read an unsigned short from the stream (little endian)
    /// </summary>
    /// <returns></returns>
    Task<ushort> ReadU2leAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read an unsigned int from the stream (little endian)
    /// </summary>
    /// <returns></returns>
    Task<uint> ReadU4leAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read an unsigned long from the stream (little endian)
    /// </summary>
    /// <returns></returns>
    Task<ulong> ReadU8leAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a single-precision floating point value from the stream (big endian)
    /// </summary>
    /// <returns></returns>
    Task<float> ReadF4beAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a double-precision floating point value from the stream (big endian)
    /// </summary>
    /// <returns></returns>
    Task<double> ReadF8beAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a single-precision floating point value from the stream (little endian)
    /// </summary>
    /// <returns></returns>
    Task<float> ReadF4leAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a double-precision floating point value from the stream (little endian)
    /// </summary>
    /// <returns></returns>
    Task<double> ReadF8leAsync(CancellationToken cancellationToken = default);

    [Obsolete("use ReadBitsIntBe instead")]
    Task<ulong> ReadBitsIntAsync(int n, CancellationToken cancellationToken = default);
    
    Task<ulong> ReadBitsIntBeAsync(int n, CancellationToken cancellationToken = default);

    Task<ulong> ReadBitsIntLeAsync(int n, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a fixed number of bytes from the stream
    /// </summary>
    /// <param name="count">The number of bytes to read</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<byte[]> ReadBytesAsync(long count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a fixed number of bytes from the stream
    /// </summary>
    /// <param name="count">The number of bytes to read</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<byte[]> ReadBytesAsync(ulong count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read all the remaining bytes from the stream until the end is reached
    /// </summary>
    /// <returns></returns>
    Task<byte[]> ReadBytesFullAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a terminated string from the stream
    /// </summary>
    /// <param name="terminator">The string terminator value</param>
    /// <param name="includeTerminator">True to include the terminator in the returned string</param>
    /// <param name="consumeTerminator">True to consume the terminator byte before returning</param>
    /// <param name="eosError">True to throw an error when the EOS was reached before the terminator</param>
    /// <returns></returns>
    Task<byte[]> ReadBytesTermAsync(byte terminator,
      bool includeTerminator,
      bool consumeTerminator,
      bool eosError,
      CancellationToken cancellationToken = default);

    /// <summary>
    /// Read a specific set of bytes and assert that they are the same as an expected result
    /// </summary>
    /// <param name="expected">The expected result</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("use explicit \"if\" using ByteArrayCompare method instead")]
    Task<byte[]> EnsureFixedContentsAsync(byte[] expected, CancellationToken cancellationToken = default);
  }
}