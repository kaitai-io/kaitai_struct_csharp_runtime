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

        /// <summary>
        /// Get the current position in the stream
        /// </summary>
        /// <returns></returns>
        public long Pos()
        {
            return BaseStream.Position;
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
        /// Check if the stream position is at the end of the stream
        /// </summary>
        /// <returns>True if the pointer is at the end of the stream</returns>
        public bool IsEof()
        {
            return BaseStream.Position >= BaseStream.Length;
        }

        /// <summary>
        /// Read a fixed number of bytes from the stream
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        public byte[] ReadBytes(long count)
        {
            return base.ReadBytes((int) count);
        }

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
            return ReadUInt16();
        }

        /// <summary>
        /// Read a signed short from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public short ReadS2le()
        {
            return ReadInt16();
        }

        /// <summary>
        /// Read an unsigned int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public uint ReadU4le()
        {
            return ReadUInt32();
        }

        /// <summary>
        /// Read a signed int from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public int ReadS4le()
        {
            return ReadInt32();
        }

        /// <summary>
        /// Read an unsigned long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public ulong ReadU8le()
        {
            return ReadUInt64();
        }

        /// <summary>
        /// Read a signed long from the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public long ReadS8le()
        {
            return ReadInt64();
        }

        /// <summary>
        /// Read an unsigned short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public ushort ReadU2be()
        {
            var b1 = ReadByte();
            var b2 = ReadByte();
            return (ushort) ((b1 << 8) + (b2 << 0));
        }

        /// <summary>
        /// Read a signed short from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public short ReadS2be()
        {
            var b1 = ReadByte();
            var b2 = ReadByte();
            return (short) ((b1 << 8) + (b2 << 0));
        }

        /// <summary>
        /// Read an unsigned int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public uint ReadU4be()
        {
            var b1 = ReadByte();
            var b2 = ReadByte();
            var b3 = ReadByte();
            var b4 = ReadByte();
            return (uint) ((b1 << 24) + (b2 << 16) + (b3 << 8) + (b4 << 0));
        }

        /// <summary>
        /// Read a signed int from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public int ReadS4be()
        {
            var b1 = ReadByte();
            var b2 = ReadByte();
            var b3 = ReadByte();
            var b4 = ReadByte();
            return (b1 << 24) + (b2 << 16) + (b3 << 8) + (b4 << 0);
        }

        /// <summary>
        /// Read an unsigned long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public ulong ReadU8be()
        {
            byte[] buffer = ReadBytes(8);
            return (ulong) (
                ( buffer[0] << 56) +  // high byte
                ( buffer[1] << 48) +
                ( buffer[2] << 40) +
                ( buffer[3] << 32) + // ^ high word
                ( buffer[4] << 24) + // v low word
                ( buffer[5] << 16) +
                ( buffer[6] << 8)  +
                ( buffer[7] << 0)  ); // low byte
        }

        /// <summary>
        /// Read a signed long from the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public long ReadS8be()
        {
            byte[] buffer = ReadBytes(8);
            return (Int64) (
                ( buffer[0] << 56) +  // high byte
                ( buffer[1] << 48) +
                ( buffer[2] << 40) +
                ( buffer[3] << 32) + // ^ high word
                ( buffer[4] << 24) + // v low word
                ( buffer[5] << 16) +
                ( buffer[6] << 8)  +
                ( buffer[7] << 0)  ); // low byte
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

        /// <summary>
        /// Deflates a stream of bytes
        /// </summary>
        /// <param name="data">The data to deflate</param>
        /// <returns>The deflated result</returns>
        public byte[] ProcessZlib(byte[] data)
        {
            using (var ms = new MemoryStream(data))
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

        public byte[] ProcessXorInt(byte[] value, int xorValue)
        {
            var result = new byte[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                result[i] = (byte) (value[i] ^ xorValue);
            }
            return result;
        }
    }
}