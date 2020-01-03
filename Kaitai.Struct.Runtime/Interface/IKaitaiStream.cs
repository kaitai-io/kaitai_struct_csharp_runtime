namespace Kaitai
{
    public interface IKaitaiStream : IKaitaiStreamBase
    {
        /// <summary>
        /// Seek to a specific position from the beginning of the stream
        /// </summary>
        /// <param name="position">The position to seek to</param>
        void Seek(long position);

        /// <summary>
        /// Read a signed byte from the stream
        /// </summary>
        /// <returns></returns>
        sbyte ReadS1();

        /// <summary>
        /// Read a signed short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        short ReadS2be();

        /// <summary>
        /// Read a signed int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        int ReadS4be();

        /// <summary>
        /// Read a signed long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        long ReadS8be();

        /// <summary>
        /// Read a signed short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        short ReadS2le();

        /// <summary>
        /// Read a signed int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        int ReadS4le();

        /// <summary>
        /// Read a signed long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        long ReadS8le();

        /// <summary>
        /// Read an unsigned byte from the stream
        /// </summary>
        /// <returns></returns>
        byte ReadU1();

        /// <summary>
        /// Read an unsigned short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        ushort ReadU2be();

        /// <summary>
        /// Read an unsigned int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        uint ReadU4be();

        /// <summary>
        /// Read an unsigned long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        ulong ReadU8be();

        /// <summary>
        /// Read an unsigned short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        ushort ReadU2le();

        /// <summary>
        /// Read an unsigned int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        uint ReadU4le();

        /// <summary>
        /// Read an unsigned long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        ulong ReadU8le();

        /// <summary>
        /// Read a single-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        float ReadF4be();

        /// <summary>
        /// Read a double-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        double ReadF8be();

        /// <summary>
        /// Read a single-precision floating point value from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        float ReadF4le();

        /// <summary>
        /// Read a double-precision floating point value from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        double ReadF8le();

        ulong ReadBitsInt(int n);
        ulong ReadBitsIntLe(int n);

        /// <summary>
        /// Read a fixed number of bytes from the stream
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        byte[] ReadBytes(long count);

        /// <summary>
        /// Read a fixed number of bytes from the stream
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        byte[] ReadBytes(ulong count);

        /// <summary>
        /// Read all the remaining bytes from the stream until the end is reached
        /// </summary>
        /// <returns></returns>
        byte[] ReadBytesFull();

        /// <summary>
        /// Read a terminated string from the stream
        /// </summary>
        /// <param name="terminator">The string terminator value</param>
        /// <param name="includeTerminator">True to include the terminator in the returned string</param>
        /// <param name="consumeTerminator">True to consume the terminator byte before returning</param>
        /// <param name="eosError">True to throw an error when the EOS was reached before the terminator</param>
        /// <returns></returns>
        byte[] ReadBytesTerm(byte terminator, bool includeTerminator, bool consumeTerminator, bool eosError);

        /// <summary>
        /// Read a specific set of bytes and assert that they are the same as an expected result
        /// </summary>
        /// <param name="expected">The expected result</param>
        /// <returns></returns>
        byte[] EnsureFixedContents(byte[] expected);
    }
}
