using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Kaitai
{
    /// <summary>
    /// The base Kaitai stream which exposes an API for the Kaitai Struct framework.
    /// It's based off a <c>BinaryReader</c>, which is a little-endian reader.
    /// </summary>
    public partial class KaitaiStream : BinaryReader
    {
        #region Constructors

        /// <summary>
        /// Create a KaitaiStream backed by abstract stream. It could be in-memory buffer or open file.
        /// </summary>
        public KaitaiStream(Stream stream) : base(stream)
        {
        }

        /// <summary>
        /// Create a KaitaiStream by opening a file in read-only binary mode (FileStream).
        /// </summary>
        public KaitaiStream(string filename) : base(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        /// <summary>
        /// Create a KaitaiStream backed by in-memory buffer.
        /// </summary>
        public KaitaiStream(byte[] data) : base(new MemoryStream(data))
        {
        }

        /// <summary>
        /// Temporary 64-bit buffer for leftover bits left from an unaligned bit 
        /// read operation. Following unaligned bit operations would consume bits 
        /// left in this buffer first, and then, if needed, would continue 
        /// consuming bytes from the stream.
        /// </summary>
        private ulong Bits = 0;
        /// <summary>
        /// Number of bits left in <c>Bits</c> buffer.
        /// </summary>
        private int BitsLeft = 0;

        /// <summary>
        /// Caches the current platform endianness and allows emitted bytecode to be optimized. Thanks to @Arlorean.
        /// https://github.com/kaitai-io/kaitai_struct_csharp_runtime/pull/9
        /// </summary>
        static readonly bool IsLittleEndian = BitConverter.IsLittleEndian;

        #endregion

        #region Stream positioning

        /// <summary>
        /// Check if the stream position is at the end of the stream (at EOF).
        /// WARNING: This requires a stream that supports seeking (memory-based or file-based).
        /// </summary>
        public bool IsEof
        {
            get { return BaseStream.Position >= BaseStream.Length; }
        }

        /// <summary>
        /// Move the stream to a specified absolute position.
        /// WARNING: This requires a stream that supports seeking (memory-based or file-based).
        /// </summary>
        /// <param name="position">The position to seek to, as non-negative integer</param>
        public void Seek(long position)
        {
            BaseStream.Seek(position, SeekOrigin.Begin);
        }

        /// <summary>
        /// Get the current position within the stream.
        /// WARNING: This requires a stream that supports seeking (memory-based or file-based).
        /// </summary>
        public long Pos
        {
            get { return BaseStream.Position; }
        }

        /// <summary>
        /// Get the total length of the stream (ie. file size).
        /// WARNING: This requires a stream that supports seeking (memory-based or file-based).
        /// </summary>
        public long Size
        {
            get { return BaseStream.Length; }
        }

        #endregion

        #region Integer types

        #region Signed

        /// <summary>
        /// Read a signed byte (1 byte) from the stream.
        /// </summary>
        public sbyte ReadS1()
        {
            return ReadSByte();
        }

        #region Big-endian

        /// <summary>
        /// Read a signed short (2 bytes) from the stream (big endian).
        /// </summary>
        public short ReadS2be()
        {
            return BitConverter.ToInt16(ReadBytesNormalisedBigEndian(2), 0);
        }

        /// <summary>
        /// Read a signed int (4 bytes) from the stream (big endian).
        /// </summary>
        public int ReadS4be()
        {
            return BitConverter.ToInt32(ReadBytesNormalisedBigEndian(4), 0);
        }

        /// <summary>
        /// Read a signed long (8 bytes) from the stream (big endian).
        /// </summary>
        public long ReadS8be()
        {
            return BitConverter.ToInt64(ReadBytesNormalisedBigEndian(8), 0);
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Read a signed short (2 bytes) from the stream (little endian).
        /// </summary>
        public short ReadS2le()
        {
            return BitConverter.ToInt16(ReadBytesNormalisedLittleEndian(2), 0);
        }

        /// <summary>
        /// Read a signed int (4 bytes) from the stream (little endian).
        /// </summary>
        public int ReadS4le()
        {
            return BitConverter.ToInt32(ReadBytesNormalisedLittleEndian(4), 0);
        }

        /// <summary>
        /// Read a signed long (8 bytes) from the stream (little endian).
        /// </summary>
        public long ReadS8le()
        {
            return BitConverter.ToInt64(ReadBytesNormalisedLittleEndian(8), 0);
        }

        #endregion

        #endregion

        #region Unsigned

        /// <summary>
        /// Read an unsigned byte (1 bytes) from the stream.
        /// </summary>
        public byte ReadU1()
        {
            return ReadByte();
        }

        #region Big-endian

        /// <summary>
        /// Read an unsigned short (2 bytes) from the stream (big endian).
        /// </summary>
        public ushort ReadU2be()
        {
            return BitConverter.ToUInt16(ReadBytesNormalisedBigEndian(2), 0);
        }

        /// <summary>
        /// Read an unsigned int (4 bytes) from the stream (big endian).
        /// </summary>
        public uint ReadU4be()
        {
            return BitConverter.ToUInt32(ReadBytesNormalisedBigEndian(4), 0);
        }

        /// <summary>
        /// Read an unsigned long (8 bytes) from the stream (big endian).
        /// </summary>
        public ulong ReadU8be()
        {
            return BitConverter.ToUInt64(ReadBytesNormalisedBigEndian(8), 0);
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Read an unsigned short (2 bytes) from the stream (little endian).
        /// </summary>
        public ushort ReadU2le()
        {
            return BitConverter.ToUInt16(ReadBytesNormalisedLittleEndian(2), 0);
        }

        /// <summary>
        /// Read an unsigned int (4 bytes) from the stream (little endian).
        /// </summary>
        public uint ReadU4le()
        {
            return BitConverter.ToUInt32(ReadBytesNormalisedLittleEndian(4), 0);
        }

        /// <summary>
        /// Read an unsigned long (8 bytes) from the stream (little endian).
        /// </summary>
        public ulong ReadU8le()
        {
            return BitConverter.ToUInt64(ReadBytesNormalisedLittleEndian(8), 0);
        }

        #endregion

        #endregion

        #endregion

        #region Floating point types

        #region Big-endian

        /// <summary>
        /// Read a single-precision floating point value (4 bytes) from the stream (big endian).
        /// </summary>
        public float ReadF4be()
        {
            return BitConverter.ToSingle(ReadBytesNormalisedBigEndian(4), 0);
        }

        /// <summary>
        /// Read a double-precision floating point value (8 bytes) from the stream (big endian).
        /// </summary>
        public double ReadF8be()
        {
            return BitConverter.ToDouble(ReadBytesNormalisedBigEndian(8), 0);
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Read a single-precision floating point value (4 bytes) from the stream (little endian).
        /// </summary>
        public float ReadF4le()
        {
            return BitConverter.ToSingle(ReadBytesNormalisedLittleEndian(4), 0);
        }

        /// <summary>
        /// Read a double-precision floating point value (8 bytes) from the stream (little endian).
        /// </summary>
        public double ReadF8le()
        {
            return BitConverter.ToDouble(ReadBytesNormalisedLittleEndian(8), 0);
        }

        #endregion

        #endregion

        #region Unaligned bit values

        /// <summary>
        /// Clears the temporary buffer which holds not-yet-consumed parts of 
        /// the byte, which might have left over after last unaligned bit read 
        /// operation. Effectively, aligns the pointer in the stream to next 
        /// whole byte, discarding the rest of partially byte, if it existed. 
        /// Does nothing if unaligned bit reading is not used.
        /// </summary>
        public void AlignToByte()
        {
            Bits = 0;
            BitsLeft = 0;
        }

        /// <summary>
        /// Read next n bits from the stream. By convention, starts from most 
        /// significant bits first and goes to least significant bits.
        /// </summary>
        /// <remarks>
        /// If n is not a multiple of 8, then bits left over from partially 
        /// consumed byte would be stored in stream internal buffer. Subsequent 
        /// calls to this method would consume bits from that buffer first, and 
        /// then proceed with fetching the new bytes from the stream.
        /// </remarks>
        public ulong ReadBitsInt(int n)
        {
            int bitsNeeded = n - BitsLeft;
            if (bitsNeeded > 0)
            {
                // 1 bit  => 1 byte
                // 8 bits => 1 byte
                // 9 bits => 2 bytes
                int bytesNeeded = ((bitsNeeded - 1) / 8) + 1;
                byte[] buf = ReadBytes(bytesNeeded);
                for (int i = 0; i < buf.Length; i++)
                {
                    Bits <<= 8;
                    Bits |= buf[i];
                    BitsLeft += 8;
                }
            }

            // raw mask with required number of 1s, starting from lowest bit
            ulong mask = GetMaskOnes(n);
            // shift mask to align with highest bits available in "bits"
            int shiftBits = BitsLeft - n;
            mask = mask << shiftBits;
            // derive reading result
            ulong res = (Bits & mask) >> shiftBits;
            // clear top bits that we've just read => AND with 1s
            BitsLeft -= n;
            mask = GetMaskOnes(BitsLeft);
            Bits &= mask;

            return res;
        }

        private static ulong GetMaskOnes(int n)
        {
            return n == 64 ? 0xffffffffffffffffUL : (1UL << n) - 1;
        }

        #endregion

        #region Byte arrays

        /// <summary>
        /// Read a fixed number of bytes from the stream.
        /// </summary>
        /// <param name="count">Amount of bytes to read, as non-negative integer</param>
        public byte[] ReadBytes(long count)
        {
            if (count < 0 || count > Int32.MaxValue)
                throw new ArgumentOutOfRangeException("requested " + count + " bytes, while only non-negative int32 amount of bytes possible");
            byte[] bytes = base.ReadBytes((int) count);
            if (bytes.Length < count)
                throw new EndOfStreamException("requested " + count + " bytes, but stream  returned only " + bytes.Length + " bytes");
            return bytes;
        }

        /// <summary>
        /// Read a fixed number of bytes from the stream.
        /// </summary>
        /// <param name="count">Amount of bytes to read, as non-negative integer</param>
        public byte[] ReadBytes(ulong count)
        {
            if (count > Int32.MaxValue)
                throw new ArgumentOutOfRangeException("requested " + count + " bytes, while only non-negative int32 amount of bytes possible");
            byte[] bytes = base.ReadBytes((int)count);
            if (bytes.Length < (int)count)
                throw new EndOfStreamException("requested " + count + " bytes, but got only " + bytes.Length + " bytes");
            return bytes;
        }

        /// <summary>
        /// Read bytes from the stream in little endian format and convert them to the endianness of the current platform.
        /// </summary>
        /// <param name="count">Amount of bytes to read, as non-negative integer</param>
        protected byte[] ReadBytesNormalisedLittleEndian(int count)
        {
            byte[] bytes = ReadBytes(count);
            if (!IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Read bytes from the stream in big endian format and convert them to the endianness of the current platform.
        /// </summary>
        /// <param name="count">Amount of bytes to read, as non-negative integer</param>
        protected byte[] ReadBytesNormalisedBigEndian(int count)
        {
            byte[] bytes = ReadBytes(count);
            if (IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Read all the remaining bytes from the stream until EOF is reached.
        /// WARNING: This requires a stream that supports seeking (memory-based or file-based).
        /// </summary>
        public byte[] ReadBytesFull()
        {
            return ReadBytes(BaseStream.Length - BaseStream.Position);
        }

        /// <summary>
        /// Read bytes from the stream, until either terminating byte or EOF is encountered.
        /// </summary>
        /// <param name="terminator">The terminating byte, as integer</param>
        /// <param name="includeTerminator">True to include the terminator in the returned bytes</param>
        /// <param name="consumeTerminator">True to consume the terminator before returning bytes</param>
        /// <param name="eosError">True to throw an error when the EOF was reached before the terminator</param>
        public byte[] ReadBytesTerm(byte terminator, bool includeTerminator, bool consumeTerminator, bool eosError)
        {
            List<byte> bytes = new List<byte>();
            while (true)
            {
                if (IsEof)
                {
                    if (eosError) throw new EndOfStreamException(string.Format("End of stream reached, but no terminator {0} found", terminator));
                    break;
                }

                byte b = ReadByte();
                if (b == terminator)
                {
                    if (includeTerminator) bytes.Add(b);
                    if (!consumeTerminator) Seek(Pos - 1);
                    break;
                }
                bytes.Add(b);
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Read a matching amount of bytes and assert that it matches the expected data. Returns same data, or throws an exception.
        /// </summary>
        /// <param name="expected">The expected data, as byte array</param>
        public byte[] EnsureFixedContents(byte[] expected)
        {
            byte[] bytes = ReadBytes(expected.Length);

            if (ByteArrayEqual(bytes, expected) == false)
            {
                throw new Exception(string.Format("Expected: {0} , Actual: {1}", Convert.ToBase64String(expected), Convert.ToBase64String(bytes) ));
            }
            
            return bytes;
        }

        /// <summary>
        /// Perform right-to-left strip on a byte array.
        /// </summary>
        /// <param name="src">The data, as byte array</param>
        /// <param name="padByte">The padding byte, as integer</param>
        public static byte[] BytesStripRight(byte[] src, byte padByte)
        {
            int newLen = src.Length;
            while (newLen > 0 && src[newLen - 1] == padByte)
                newLen--;

            byte[] dst = new byte[newLen];
            Array.Copy(src, dst, newLen);
            return dst;
        }

        /// <summary>
        /// Perform left-to-right search of specified terminating byte, and cutoff remaining bytes.
        /// </summary>
        /// <param name="src">The data, as byte array</param>
        /// <param name="terminator">The terminating byte, as integer</param>
        /// <param name="includeTerminator">True to include the terminating byte in result</param>
        public static byte[] BytesTerminate(byte[] src, byte terminator, bool includeTerminator)
        {
            int newLen = 0;
            int maxLen = src.Length;

            while (newLen < maxLen && src[newLen] != terminator)
                newLen++;

            if (includeTerminator && newLen < maxLen)
                newLen++;

            byte[] dst = new byte[newLen];
            Array.Copy(src, dst, newLen);
            return dst;
        }

        #endregion

        #region Byte array processing

        /// <summary>
        /// Perform XOR processing between given data and single-byte key.
        /// </summary>
        /// <param name="value">The data to process, as byte array</param>
        /// <param name="key">The key to XOR with, as integer</param>
        public byte[] ProcessXor(byte[] value, int key)
        {
            byte[] result = new byte[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                result[i] = (byte)(value[i] ^ key);
            }
            return result;
        }

        /// <summary>
        /// Perform XOR processing between given data and multiple-byte key.
        /// </summary>
        /// <param name="value">The data to process, as byte array</param>
        /// <param name="key">The key to XOR with, as byte array</param>
        public byte[] ProcessXor(byte[] value, byte[] key)
        {
            int keyLen = key.Length;
            byte[] result = new byte[value.Length];
            for (int i = 0, j = 0; i < value.Length; i++, j = (j + 1) % keyLen)
            {
                result[i] = (byte)(value[i] ^ key[j]);
            }
            return result;
        }

        /// <summary>
        /// Perform circular left rotation shift for a given data by a given amount of bits.
        /// Pass a negative amount to rotate right.
        /// </summary>
        /// <param name="data">The data to rotate, as byte array</param>
        /// <param name="amount">The amount to rotate by (in bits), as integer</param>
        /// <param name="groupSize">The size of group in which rotation happens, as non-negative integer</param>
        public byte[] ProcessRotateLeft(byte[] data, int amount, int groupSize)
        {
            if (amount > 7 || amount < -7) throw new ArgumentException("Rotation of more than 7 cannot be performed.", "amount");
            if (amount < 0) amount += 8; // Rotation of -2 is the same as rotation of +6

            byte[] r = new byte[data.Length];
            switch (groupSize)
            {
                case 1:
                    for (int i = 0; i < data.Length; i++)
                    {
                        byte bits = data[i];
                        // http://stackoverflow.com/a/812039
                        r[i] = (byte) ((bits << amount) | (bits >> (8 - amount)));
                    }
                    break;
                default:
                    throw new NotImplementedException(string.Format("Unable to rotate a group of {0} bytes yet", groupSize));
            }
            return r;
        }

        /// <summary>
        /// Inflate a previously deflated zlib byte stream.
        /// </summary>
        /// <param name="data">The data to deflate, as byte array</param>
        public byte[] ProcessZlib(byte[] data)
        {
            // See RFC 1950 (https://tools.ietf.org/html/rfc1950)
            // zlib adds a header to DEFLATE streams - usually 2 bytes,
            // but can be 6 bytes if FDICT is set.
            // There's also 4 checksum bytes at the end of the stream.

            byte zlibCmf = data[0];
            if ((zlibCmf & 0x0F) != 0x08) throw new NotSupportedException("Only the DEFLATE algorithm is supported for zlib data.");

            const int zlibFooter = 4;
            int zlibHeader = 2;

            // If the FDICT bit (0x20) is 1, then the 4-byte dictionary is included in the header, we need to skip it
            byte zlibFlg = data[1];
            if ((zlibFlg & 0x20) == 0x20) zlibHeader += 4;

            using (MemoryStream ms = new MemoryStream(data, zlibHeader, data.Length - (zlibHeader + zlibFooter)))
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    using (MemoryStream target = new MemoryStream())
                    {
                        ds.CopyTo(target);
                        return target.ToArray();
                    }
                }
            }
        }

        #endregion

        #region Misc utility methods

        /// <summary>
        /// Perform modulo operation between two integers. 
        /// NOTE: Result is always non-negative and within `0..b-1`.
        /// </summary>
        /// <remarks>
        /// This method is required because C# lacks a "true" modulo operator, 
        /// the % operator rather being the "division remainder" operator. 
        /// </remarks>
        /// <param name="a">The value to be divided, as integer</param>
        /// <param name="b">The value to divide by, as positive integer</param>
        public static int Mod(int a, int b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            int r = a % b;
            if (r < 0) r += b;
            return r;
        }

        /// <summary>
        /// Perform modulo operation between two integers. 
        /// NOTE: Result is always non-negative and within `0..b-1`.
        /// </summary>
        /// <remarks>
        /// This method is required because C# lacks a "true" modulo operator, 
        /// the % operator rather being the "division remainder" operator. 
        /// </remarks>
        /// <param name="a">The value to be divided, as integer</param>
        /// <param name="b">The value to divide by, as positive integer</param>
        public static long Mod(long a, long b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            long r = a % b;
            if (r < 0) r += b;
            return r;
        }

        /// <summary>
        /// Compare two byte arrays in lexicographical order. The arrays may not be equal length.
        /// </summary>
        /// <returns>Negative number if a is less than b, <c>0</c> if a is equal to b, positive number if a is greater than b.</returns>
        /// <param name="a">First byte array to compare</param>
        /// <param name="b">Second byte array to compare</param>
        public static int ByteArrayCompare(byte[] a, byte[] b)
        {
            if (a == b)
                return 0;
            int al = a.Length;
            int bl = b.Length;
            int minLen = al < bl ? al : bl;

            for (int i = 0; i < minLen; i++) 
            {
                int cmp = a[i] - b[i];
                if (cmp != 0)
                    return cmp;
            }

            // Reached the end of at least one of the arrays
            return al == bl ? 0 : al - bl;
        }

        /// <summary>
        /// Compare two byte arrays for exact equality.
        /// </summary>
        /// <remarks>
        /// .NET has Array.Equals and == operators, but those check object identity.
        /// </remarks>
        public static bool ByteArrayEqual(byte[] a, byte[] b)
        {
            if (a == b)
                return true;

            if (a.Length != b.Length)
                return false;

            for (int i=0; i<a.Length; i++)
                if (a[i] != b[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Check if byte array is all zeroes.
        /// </summary>
        public static bool IsByteArrayZero(byte[] a)
        {
            for (int i=0; i<a.Length; i++)
                if (a[i] != 0)
                    return false;

            return true;
        }

        #endregion
    }
}
