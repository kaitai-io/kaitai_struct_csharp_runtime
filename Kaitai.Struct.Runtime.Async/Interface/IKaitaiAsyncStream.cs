using System.Threading.Tasks;

namespace Kaitai.Async
{
    public interface IKaitaiAsyncStream : IKaitaiStreamBase
    {
        /// <summary>
        /// Seek to a specific position from the beginning of the stream
        /// </summary>
        /// <param name="position">The position to seek to</param>
        Task SeekAsync(long position);

        /// <summary>
        /// Read a signed byte from the stream
        /// </summary>
        /// <returns></returns>
        Task<sbyte> ReadS1Async();

        /// <summary>
        /// Read a signed short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        Task<short> ReadS2beAsync();

        /// <summary>
        /// Read a signed int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        Task<int> ReadS4beAsync();

        /// <summary>
        /// Read a signed long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        Task<long> ReadS8beAsync();

        /// <summary>
        /// Read a signed short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        Task<short> ReadS2leAsync();

        /// <summary>
        /// Read a signed int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        Task<int> ReadS4leAsync();

        /// <summary>
        /// Read a signed long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        Task<long> ReadS8leAsync();

        /// <summary>
        /// Read an unsigned byte from the stream
        /// </summary>
        /// <returns></returns>
        Task<byte> ReadU1Async();

        /// <summary>
        /// Read an unsigned short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        Task<ushort> ReadU2beAsync();

        /// <summary>
        /// Read an unsigned int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        Task<uint> ReadU4beAsync();

        /// <summary>
        /// Read an unsigned long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        Task<ulong> ReadU8beAsync();

        /// <summary>
        /// Read an unsigned short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        Task<ushort> ReadU2leAsync();

        /// <summary>
        /// Read an unsigned int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        Task<uint> ReadU4leAsync();

        /// <summary>
        /// Read an unsigned long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        Task<ulong> ReadU8leAsync();

        /// <summary>
        /// Read a single-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        Task<float> ReadF4beAsync();

        /// <summary>
        /// Read a double-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        Task<double> ReadF8beAsync();

        /// <summary>
        /// Read a single-precision floating point value from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        Task<float> ReadF4leAsync();

        /// <summary>
        /// Read a double-precision floating point value from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        Task<double> ReadF8leAsync();

        Task<ulong> ReadBitsIntAsync(int n);
        Task<ulong> ReadBitsIntLeAsync(int n);

        /// <summary>
        /// Read a fixed number of bytes from the stream
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        Task<byte[]> ReadBytesAsync(long count);

        /// <summary>
        /// Read a fixed number of bytes from the stream
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        Task<byte[]> ReadBytesAsync(ulong count);

        /// <summary>
        /// Read all the remaining bytes from the stream until the end is reached
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ReadBytesFullAsync();

        /// <summary>
        /// Read a terminated string from the stream
        /// </summary>
        /// <param name="terminator">The string terminator value</param>
        /// <param name="includeTerminator">True to include the terminator in the returned string</param>
        /// <param name="consumeTerminator">True to consume the terminator byte before returning</param>
        /// <param name="eosError">True to throw an error when the EOS was reached before the terminator</param>
        /// <returns></returns>
        Task<byte[]> ReadBytesTermAsync(byte terminator, bool includeTerminator, bool consumeTerminator, bool eosError);

        /// <summary>
        /// Read a specific set of bytes and assert that they are the same as an expected result
        /// </summary>
        /// <param name="expected">The expected result</param>
        /// <returns></returns>
        Task<byte[]> EnsureFixedContentsAsync(byte[] expected);
    }
}
