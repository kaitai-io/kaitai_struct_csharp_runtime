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

        public override bool IsEof => BaseStream.Position >= BaseStream.Length && BitsLeft == 0;

        public void Seek(long position) => BaseStream.Seek(position, SeekOrigin.Begin);
        public void Seek(ulong position) => Seek((long)position);

        public override long Pos => BaseStream.Position;

        public override long Size => BaseStream.Length;

        #endregion

        #region Integer types

        #region Signed

        public sbyte ReadS1() => BinaryReader.ReadSByte();

        #region Big-endian

        public short ReadS2be() => BitConverter.ToInt16(ReadBytesNormalisedBigEndian(2), 0);

        public int ReadS4be() => BitConverter.ToInt32(ReadBytesNormalisedBigEndian(4), 0);

        public long ReadS8be() => BitConverter.ToInt64(ReadBytesNormalisedBigEndian(8), 0);

        #endregion

        #region Little-endian

        public short ReadS2le() => BitConverter.ToInt16(ReadBytesNormalisedLittleEndian(2), 0);

        public int ReadS4le() => BitConverter.ToInt32(ReadBytesNormalisedLittleEndian(4), 0);

        public long ReadS8le() => BitConverter.ToInt64(ReadBytesNormalisedLittleEndian(8), 0);

        #endregion

        #endregion

        #region Unsigned

        public byte ReadU1() => BinaryReader.ReadByte();

        #region Big-endian

        public ushort ReadU2be() => BitConverter.ToUInt16(ReadBytesNormalisedBigEndian(2), 0);

        public uint ReadU4be() => BitConverter.ToUInt32(ReadBytesNormalisedBigEndian(4), 0);

        public ulong ReadU8be() => BitConverter.ToUInt64(ReadBytesNormalisedBigEndian(8), 0);

        #endregion

        #region Little-endian

        public ushort ReadU2le() => BitConverter.ToUInt16(ReadBytesNormalisedLittleEndian(2), 0);

        public uint ReadU4le() => BitConverter.ToUInt32(ReadBytesNormalisedLittleEndian(4), 0);

        public ulong ReadU8le() => BitConverter.ToUInt64(ReadBytesNormalisedLittleEndian(8), 0);

        #endregion

        #endregion

        #endregion

        #region Floating point types

        #region Big-endian

        public float ReadF4be() => BitConverter.ToSingle(ReadBytesNormalisedBigEndian(4), 0);

        public double ReadF8be() => BitConverter.ToDouble(ReadBytesNormalisedBigEndian(8), 0);

        #endregion

        #region Little-endian

        public float ReadF4le() => BitConverter.ToSingle(ReadBytesNormalisedLittleEndian(4), 0);

        public double ReadF8le() => BitConverter.ToDouble(ReadBytesNormalisedLittleEndian(8), 0);

        #endregion

        #endregion

        #region Unaligned bit values

        public override void AlignToByte()
        {
            Bits = 0;
            BitsLeft = 0;
        }

        public ulong ReadBitsIntBe(int n)
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
            mask <<= shiftBits;
            // derive reading result
            ulong res = (Bits & mask) >> shiftBits;
            // clear top bits that we've just read => AND with 1s
            BitsLeft -= n;
            mask = GetMaskOnes(BitsLeft);
            Bits &= mask;

            return res;
        }

        public ulong ReadBitsInt(int n) => ReadBitsIntBe(n);


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

        public byte[] ReadBytes(long count)
        {
            if (count < 0 || count > Int32.MaxValue)
                throw new ArgumentOutOfRangeException($"requested {count} bytes, while only non-negative int32 amount of bytes possible");
            byte[] bytes = BinaryReader.ReadBytes((int)count);
            if (bytes.Length < count)
                throw new EndOfStreamException($"requested {count} bytes, but got only {bytes.Length} bytes");
            return bytes;
        }

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
