using System;
using System.Collections.Generic;
using System.IO;

namespace Kaitai
{
    /// <summary>
    /// The base Kaitai stream which exposes an API for the Kaitai Struct framework.
    /// It's based off a <code>BinaryReader</code>, which is a little-endian reader.
    /// </summary>
    public partial class KaitaiStream : KaitaiStreamBase, IKaitaiStream
    {
        #region Constructors
        private ulong Bits;
        private int BitsLeft;
        private BinaryReader m_binaryReader;

        protected Stream BaseStream;

        public KaitaiStream(Stream stream)
        {
            BaseStream = stream;
        }

        /// <summary>
        /// Creates a KaitaiStream backed by a file (RO)
        /// </summary>
        public KaitaiStream(string file) : this(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        /// <summary>
        /// Creates a KaitaiStream backed by a byte buffer
        /// </summary>
        public KaitaiStream(byte[] bytes) : this(new MemoryStream(bytes))
        {
        }

        protected BinaryReader BinaryReader
        {
            get => m_binaryReader ?? (BinaryReader = new BinaryReader(BaseStream));
            set => m_binaryReader = value;
        }

        #endregion

        #region Stream positioning

        /// <summary>
        /// Check if the stream position is at the end of the stream
        /// </summary>
        public override bool IsEof
        {
            get { return BaseStream.Position >= BaseStream.Length && BitsLeft == 0; }
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
        public override long Pos
        {
            get { return BaseStream.Position; }
        }

        /// <summary>
        /// Get the total length of the stream (ie. file size)
        /// </summary>
        public override long Size
        {
            get { return BaseStream.Length; }
        }

        #endregion

        #region Integer types

        #region Signed

        /// <summary>
        /// Read a signed byte from the stream
        /// </summary>
        /// <returns></returns>
        public sbyte ReadS1()
        {
            return BinaryReader.ReadSByte();
        }

        #region Big-endian

        /// <summary>
        /// Read a signed short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public short ReadS2be()
        {
            return BitConverter.ToInt16(ReadBytesNormalisedBigEndian(2), 0);
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
        /// Read a signed long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public long ReadS8be()
        {
            return BitConverter.ToInt64(ReadBytesNormalisedBigEndian(8), 0);
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Read a signed short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public short ReadS2le()
        {
            return BitConverter.ToInt16(ReadBytesNormalisedLittleEndian(2), 0);
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
        /// Read a signed long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public long ReadS8le()
        {
            return BitConverter.ToInt64(ReadBytesNormalisedLittleEndian(8), 0);
        }

        #endregion

        #endregion

        #region Unsigned

        /// <summary>
        /// Read an unsigned byte from the stream
        /// </summary>
        /// <returns></returns>
        public byte ReadU1()
        {
            return BinaryReader.ReadByte();
        }

        #region Big-endian

        /// <summary>
        /// Read an unsigned short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public ushort ReadU2be()
        {
            return BitConverter.ToUInt16(ReadBytesNormalisedBigEndian(2), 0);
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
        /// Read an unsigned long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public ulong ReadU8be()
        {
            return BitConverter.ToUInt64(ReadBytesNormalisedBigEndian(8), 0);
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Read an unsigned short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public ushort ReadU2le()
        {
            return BitConverter.ToUInt16(ReadBytesNormalisedLittleEndian(2), 0);
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
        /// Read an unsigned long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
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

        #endregion

        #region Little-endian

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

        #endregion

        #region Unaligned bit values

        public override void AlignToByte()
        {
            Bits = 0;
            BitsLeft = 0;
        }

        public ulong ReadBitsInt(int n)
        {
            int bitsNeeded = n - BitsLeft;
            if (bitsNeeded > 0)
            {
                // 1 bit  => 1 byte
                // 8 bits => 1 byte
                // 9 bits => 2 bytes
                int bytesNeeded = (bitsNeeded - 1) / 8 + 1;
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

        //Method ported from algorithm specified @ issue#155
        public ulong ReadBitsIntLe(int n)
        {
            int bitsNeeded = n - BitsLeft;

            if (bitsNeeded > 0)
            {
                // 1 bit  => 1 byte
                // 8 bits => 1 byte
                // 9 bits => 2 bytes
                int bytesNeeded = (bitsNeeded - 1) / 8 + 1;
                byte[] buf = ReadBytes(bytesNeeded);
                for (int i = 0; i < buf.Length; i++)
                {
                    ulong v = (ulong)buf[i] << BitsLeft;
                    Bits |= v;
                    BitsLeft += 8;
                }
            }

            // raw mask with required number of 1s, starting from lowest bit
            ulong mask = GetMaskOnes(n);

            // derive reading result
            ulong res = Bits & mask;

            // remove bottom bits that we've just read by shifting
            Bits >>= n;
            BitsLeft -= n;

            return res;
        }

        #endregion

        #region Byte arrays

        /// <summary>
        /// Read a fixed number of bytes from the stream
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        public byte[] ReadBytes(long count)
        {
            if (count < 0 || count > Int32.MaxValue)
                throw new ArgumentOutOfRangeException($"requested {count} bytes, while only non-negative int32 amount of bytes possible");
            byte[] bytes = BinaryReader.ReadBytes((int)count);
            if (bytes.Length < count)
                throw new EndOfStreamException($"requested {count} bytes, but got only {bytes.Length} bytes");
            return bytes;
        }

        /// <summary>
        /// Read a fixed number of bytes from the stream
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        public byte[] ReadBytes(ulong count)
        {
            if (count > Int32.MaxValue)
                throw new ArgumentOutOfRangeException($"requested {count} bytes, while only non-negative int32 amount of bytes possible");
            byte[] bytes = BinaryReader.ReadBytes((int)count);
            if (bytes.Length < (int)count)
                throw new EndOfStreamException($"requested {count} bytes, but got only {bytes.Length} bytes");
            return bytes;
        }

        /// <summary>
        /// Read bytes from the stream in little endian format and convert them to the endianness of the current platform
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>An array of bytes that matches the endianness of the current platform</returns>
        protected byte[] ReadBytesNormalisedLittleEndian(int count)
        {
            byte[] bytes = ReadBytes(count);
            if (!IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Read bytes from the stream in big endian format and convert them to the endianness of the current platform
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>An array of bytes that matches the endianness of the current platform</returns>
        protected byte[] ReadBytesNormalisedBigEndian(int count)
        {
            byte[] bytes = ReadBytes(count);
            if (IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Read all the remaining bytes from the stream until the end is reached
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytesFull()
        {
            return ReadBytes(BaseStream.Length - BaseStream.Position);
        }

        /// <summary>
        /// Read a terminated string from the stream
        /// </summary>
        /// <param name="terminator">The string terminator value</param>
        /// <param name="includeTerminator">True to include the terminator in the returned string</param>
        /// <param name="consumeTerminator">True to consume the terminator byte before returning</param>
        /// <param name="eosError">True to throw an error when the EOS was reached before the terminator</param>
        /// <returns></returns>
        public byte[] ReadBytesTerm(byte terminator,
            bool includeTerminator,
            bool consumeTerminator,
            bool eosError)
        {
            List<byte> bytes = new List<byte>();
            while (true)
            {
                if (IsEof)
                {
                    if (eosError)
                        throw new EndOfStreamException($"End of stream reached, but no terminator `{terminator}` found");
                    break;
                }

                byte b = BinaryReader.ReadByte();
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
        /// Read a specific set of bytes and assert that they are the same as an expected result
        /// </summary>
        /// <param name="expected">The expected result</param>
        /// <returns></returns>
        public byte[] EnsureFixedContents(byte[] expected)
        {
            byte[] bytes = ReadBytes(expected.Length);

            if (bytes.Length != expected.Length)
            {
                throw new Exception($"Expected bytes: {Convert.ToBase64String(expected)} ({expected.Length} bytes), Instead got: {Convert.ToBase64String(bytes)} ({bytes.Length} bytes)");
            }

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != expected[i])
                {
                    throw new Exception($"Expected bytes: {Convert.ToBase64String(expected)} ({expected.Length} bytes), Instead got: {Convert.ToBase64String(bytes)} ({bytes.Length} bytes)");
                }
            }

            return bytes;
        }

        #endregion
    }
}
