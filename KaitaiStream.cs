using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Kaitai
{
    /// <summary>
    /// The base Kaitai steam which exposes an API for the Kaitai Struct framework.
    /// It's based off a <code>BinaryReader</code>, which is a little-endian reader.
    /// </summary>
    public partial class KaitaiStream : BinaryReader
    {
        #region Constructors

        public KaitaiStream(Stream stream) : base(stream)
        {

        }

        ///<summary>
        /// Creates a KaitaiStream backed by a file (RO)
        ///</summary>
        public KaitaiStream(string file) : base(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {

        }

        ///<summary>
        ///Creates a KaitaiStream backed by a byte buffer
        ///</summary>
        public KaitaiStream(byte[] bytes) : base(new MemoryStream(bytes))
        {

        }

        #endregion

        #region Stream positioning

        /// <summary>
        /// Check if the stream position is at the end of the stream
        /// </summary>
        /// <returns>True if the pointer is at the end of the stream</returns>
        public bool IsEof()
        {
            return BaseStream.Position >= BaseStream.Length;
        }

        /// <summary>
        /// Seek to a specific position from the beginning of the stream
        /// </summary>
        /// <param name="position">The position to seek to</param>
        public void Seek(long position)
        {
            BaseStream.Seek(position, SeekOrigin.Begin);
        }

        /// <summary>
        /// Get the current position in the stream
        /// </summary>
        /// <returns></returns>
        public long Pos()
        {
            return BaseStream.Position;
        }

        /// <summary>
        /// Get the total length of the stream
        /// </summary>
        /// <returns></returns>
        public long Size
        {
            get
            {
                return BaseStream.Length;
            }
        }

        #endregion

        /// <summary>
        /// Read a fixed number of bytes from the stream
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        public byte[] ReadBytes(long count)
        {
            var bytes = base.ReadBytes((int) count);
            if (bytes.Length < count)
                throw new EndOfStreamException("requested " + count + " bytes, but got only " + bytes.Length + " bytes");
            return bytes;
        }

        /// <summary>
        /// Read bytes from the stream in little endian format and convert them to the endianness of the current platform
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>An array of bytes that matches the endianness of the current platform</returns>
        protected byte[] ReadBytesNormalisedLittleEndian(int count)
        {
            var bytes = ReadBytes(count);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Read bytes from the stream in big endian format and convert them to the endianness of the current platform
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>An array of bytes that matches the endianness of the current platform</returns>
        protected byte[] ReadBytesNormalisedBigEndian(int count)
        {
            var bytes = ReadBytes(count);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        #region Integer types

        /// <summary>
        /// Read an unsigned byte from the stream
        /// </summary>
        /// <returns></returns>
        public byte ReadU1()
        {
            return ReadByte();
        }

        /// <summary>
        /// Read a signed byte from the stream
        /// </summary>
        /// <returns></returns>
        public sbyte ReadS1()
        {
            return ReadSByte();
        }

        /// <summary>
        /// Read an unsigned short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public ushort ReadU2le()
        {
            return BitConverter.ToUInt16(ReadBytesNormalisedLittleEndian(2), 0);
        }

        /// <summary>
        /// Read a signed short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public short ReadS2le()
        {
            return BitConverter.ToInt16(ReadBytesNormalisedLittleEndian(2), 0);
        }

        /// <summary>
        /// Read an unsigned int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public uint ReadU4le()
        {
            return BitConverter.ToUInt32(ReadBytesNormalisedLittleEndian(4), 0);
        }

        /// <summary>
        /// Read a signed int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public int ReadS4le()
        {
            return BitConverter.ToInt32(ReadBytesNormalisedLittleEndian(4), 0);
        }

        /// <summary>
        /// Read an unsigned long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public ulong ReadU8le()
        {
            return BitConverter.ToUInt64(ReadBytesNormalisedLittleEndian(8), 0);
        }

        /// <summary>
        /// Read a signed long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public long ReadS8le()
        {
            return BitConverter.ToInt64(ReadBytesNormalisedLittleEndian(8), 0);
        }

        /// <summary>
        /// Read an unsigned short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public ushort ReadU2be()
        {
            return BitConverter.ToUInt16(ReadBytesNormalisedBigEndian(2), 0);
        }

        /// <summary>
        /// Read a signed short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public short ReadS2be()
        {
            return BitConverter.ToInt16(ReadBytesNormalisedBigEndian(2), 0);
        }

        /// <summary>
        /// Read an unsigned int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public uint ReadU4be()
        {
            return BitConverter.ToUInt32(ReadBytesNormalisedBigEndian(4), 0);
        }

        /// <summary>
        /// Read a signed int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public int ReadS4be()
        {
            return BitConverter.ToInt32(ReadBytesNormalisedBigEndian(4), 0);
        }

        /// <summary>
        /// Read an unsigned long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public ulong ReadU8be()
        {
            return BitConverter.ToUInt64(ReadBytesNormalisedBigEndian(8), 0);
        }

        /// <summary>
        /// Read a signed long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public long ReadS8be()
        {
            return BitConverter.ToInt64(ReadBytesNormalisedBigEndian(8), 0);
        }

        #endregion

        #region Floating point types

        /// <summary>
        /// Read a single-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public float ReadF4be()
        {
            return BitConverter.ToSingle(ReadBytesNormalisedBigEndian(4), 0);
        }

        /// <summary>
        /// Read a double-precision floating point value from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public double ReadF8be()
        {
            return BitConverter.ToDouble(ReadBytesNormalisedBigEndian(8), 0);
        }

        /// <summary>
        /// Read a single-precision floating point value from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public float ReadF4le()
        {
            return BitConverter.ToSingle(ReadBytesNormalisedLittleEndian(4), 0);
        }

        /// <summary>
        /// Read a double-precision floating point value from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public double ReadF8le()
        {
            return BitConverter.ToDouble(ReadBytesNormalisedLittleEndian(8), 0);
        }

        #endregion

        /// <summary>
        /// Read all the remaining bytes from the stream until the end is reached
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytesFull()
        {
            return ReadBytes(BaseStream.Length - BaseStream.Position);
        }

        /// <summary>
        /// Read a specific set of bytes and assert that they are the same as an expected result
        /// </summary>
        /// <param name="length">The number of bytes to read</param>
        /// <param name="expected">The expected result</param>
        /// <returns></returns>
        public byte[] EnsureFixedContents(int length, byte[] expected)
        {
            var bytes = ReadBytes(length);
            if (!bytes.SequenceEqual(expected))
            {
                throw new Exception($"Expected bytes: {Convert.ToBase64String(expected)}, Instead got: {Convert.ToBase64String(bytes)}");
            }
            return bytes;
        }

        /// <summary>
        /// Read a string until the end of the stream is reached
        /// </summary>
        /// <param name="encoding">The string encoding to use</param>
        /// <returns></returns>
        public string ReadStrEos(string encoding)
        {
            return System.Text.Encoding.GetEncoding(encoding).GetString(ReadBytesFull());
        }

        /// <summary>
        /// Read a string of a specific length from the stream
        /// </summary>
        /// <param name="length">The number of bytes to read</param>
        /// <param name="encoding">The string encoding to use</param>
        /// <returns></returns>
        public string ReadStrByteLimit(long length, string encoding)
        {
            return System.Text.Encoding.GetEncoding(encoding).GetString(ReadBytes(length));
        }

        /// <summary>
        /// Read a terminated string from the stream
        /// </summary>
        /// <param name="encoding">The string encoding to use</param>
        /// <param name="terminator">The string terminator value</param>
        /// <param name="includeTerminator">True to include the terminator in the returned string</param>
        /// <param name="consumeTerminator">True to consume the terminator byte before returning</param>
        /// <param name="eosError">True to throw an error when the EOS was reached before the terminator</param>
        /// <returns></returns>
        public string ReadStrz(string encoding, byte terminator, bool includeTerminator, bool consumeTerminator, bool eosError)
        {
            var bytes = new System.Collections.Generic.List<byte>();
            while (true)
            {
                if (IsEof())
                {
                    if (eosError) throw new EndOfStreamException($"End of stream reached, but no terminator `{terminator}` found");
                    break;
                }

                var b = ReadByte();
                if (b == terminator)
                {
                    if (includeTerminator) bytes.Add(b);
                    if (!consumeTerminator) Seek(Pos() - 1);
                    break;
                }
                bytes.Add(b);
            }
            return System.Text.Encoding.GetEncoding(encoding).GetString(bytes.ToArray());
        }

        #region Byte array processing

        /// <summary>
        /// Performs XOR processing with given data, XORing every byte of the input with a single value.
        /// </summary>
        /// <param name="value">The data toe process</param>
        /// <param name="key">The key value to XOR with</param>
        /// <returns>Processed data</returns>
        public byte[] ProcessXor(byte[] value, int key)
        {
            var result = new byte[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                result[i] = (byte)(value[i] ^ key);
            }
            return result;
        }

        /// <summary>
        /// Performs XOR processing with given data, XORing every byte of the input with a key
        /// array, repeating from the beginning of the key array if necessary
        /// </summary>
        /// <param name="value">The data toe process</param>
        /// <param name="key">The key array to XOR with</param>
        /// <returns>Processed data</returns>
        public byte[] ProcessXor(byte[] value, byte[] key)
        {
            var keyLen = key.Length;
            var result = new byte[value.Length];
            for (int i = 0, j = 0; i < value.Length; i++, j = (j + 1) % keyLen)
            {
                result[i] = (byte)(value[i] ^ key[j]);
            }
            return result;
        }

        /// <summary>
        /// Performs a circular left rotation shift for a given buffer by a given amount of bits.
        /// Pass a negative amount to rotate right.
        /// </summary>
        /// <param name="data">The data to rotate</param>
        /// <param name="amount">The number of bytes to rotate by</param>
        /// <param name="groupSize"></param>
        /// <returns></returns>
        public byte[] ProcessRotateLeft(byte[] data, int amount, int groupSize)
        {
            if (amount > 7 || amount < -7) throw new ArgumentException("Rotation of more than 7 cannot be performed.", nameof(amount));
            if (amount < 0) amount += 8; // Rotation of -2 is the same as rotation of +6

            var r = new byte[data.Length];
            switch (groupSize)
            {
                case 1:
                    for (var i = 0; i < data.Length; i++)
                    {
                        var bits = data[i];
                        // http://stackoverflow.com/a/812039
                        r[i] = (byte) ((bits << amount) | (bits >> (8 - amount)));
                    }
                    break;
                default:
                    throw new NotImplementedException($"Unable to rotate a group of {groupSize} bytes yet");
            }
            return r;
        }

        /// <summary>
        /// Inflates a deflated zlib byte stream
        /// </summary>
        /// <param name="data">The data to deflate</param>
        /// <returns>The deflated result</returns>
        public byte[] ProcessZlib(byte[] data)
        {
            // See RFC 1950 (https://tools.ietf.org/html/rfc1950)
            // zlib adds a header to DEFLATE streams - usually 2 bytes,
            // but can be 6 bytes if FDICT is set.
            // There's also 4 checksum bytes at the end of the stream.

            var zlibCmf = data[0];
            if ((zlibCmf & 0x0F) != 0x08) throw new NotSupportedException("Only the DEFLATE algorithm is supported for zlib data.");

            const int zlibFooter = 4;
            var zlibHeader = 2;

            // If the FDICT bit (0x20) is 1, then the 4-byte dictionary is included in the header, we need to skip it
            var zlibFlg = data[1];
            if ((zlibFlg & 0x20) == 0x20) zlibHeader += 4;

            using (var ms = new MemoryStream(data, zlibHeader, data.Length - (zlibHeader + zlibFooter)))
            {
                using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    using (var target = new MemoryStream())
                    {
                        ds.CopyTo(target);
                        return target.ToArray();
                    }
                }
            }
        }

        #endregion
    }
}
