using System;
using System.IO;

namespace Kaitai
{
    /// <summary>
    /// The base Kaitai writer which exposes an API for the Kaitai Struct framework.
    /// It's based off a <code>BinaryWriter</code>, which is a little-endian writer.
    /// </summary>
    public partial class KaitaiWriter : BinaryWriter
    {
        #region Constructors

        /// <summary>
        /// Creates a new KaitaiWriter backed by a stream
        /// </summary>
        public KaitaiWriter(Stream stream) : base(stream)
        {
        }

        ///<summary>
        /// Creates a KaitaiWriter backed by a file
        ///</summary>
        public KaitaiWriter(string file) : base(File.Open(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
        {
        }

        static readonly bool IsLittleEndian = BitConverter.IsLittleEndian;

        #endregion

        #region Integer types

        #region Signed

        /// <summary>
        /// Write a signed byte to the stream
        /// </summary>
        /// <returns></returns>
        public void WriteS1(sbyte v)
        {
            Write(v);
        }

        #region Big-endian

        /// <summary>
        /// Write a signed short to the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public void WriteS2be(short v)
        {
            WriteBytesNormalisedBigEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write a signed int to the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public void WriteS4be(int v)
        {
            WriteBytesNormalisedBigEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write a signed long to the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public void WriteS8be(long v)
        {
            WriteBytesNormalisedBigEndian(BitConverter.GetBytes(v));
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Write a signed short to the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public void WriteS2le(short v)
        {
            WriteBytesNormalisedLittleEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write a signed int to the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public void WriteS4le(int v)
        {
            WriteBytesNormalisedLittleEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write a signed long to the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public void WriteS8le(long v)
        {
            WriteBytesNormalisedLittleEndian(BitConverter.GetBytes(v));
        }

        #endregion

        #endregion

        #region Unsigned

        /// <summary>
        /// Write an unsigned byte to the stream
        /// </summary>
        /// <returns></returns>
        public void WriteU1(byte v)
        {
            Write(v);
        }

        #region Big-endian

        /// <summary>
        /// Write an unsigned short to the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public void WriteU2be(ushort v)
        {
            WriteBytesNormalisedBigEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write an unsigned int to the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public void WriteU4be(uint v)
        {
            WriteBytesNormalisedBigEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write an unsigned long to the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public void WriteU8be(ulong v)
        {
            WriteBytesNormalisedBigEndian(BitConverter.GetBytes(v));
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Write an unsigned short to the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public void WriteU2le(ushort v)
        {
            WriteBytesNormalisedLittleEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write an unsigned int to the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public void WriteU4le(uint v)
        {
            WriteBytesNormalisedLittleEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write an unsigned long to the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public void WriteU8le(ulong v)
        {
            WriteBytesNormalisedLittleEndian(BitConverter.GetBytes(v));
        }

        #endregion

        #endregion

        #endregion

        #region Floating point types

        #region Big-endian

        /// <summary>
        /// Write a single-precision floating point value to the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public void WriteF4be(float v)
        {
            WriteBytesNormalisedBigEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write a double-precision floating point value to the stream (big endian)
        /// </summary>
        /// <returns></returns>
        public void WriteF8be(double v)
        {
            WriteBytesNormalisedBigEndian(BitConverter.GetBytes(v));
        }

        #endregion

        #region Little-endian

        /// <summary>
        /// Write a single-precision floating point value to the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public void WriteF4le(float v)
        {
            WriteBytesNormalisedLittleEndian(BitConverter.GetBytes(v));
        }

        /// <summary>
        /// Write a double-precision floating point value to the stream (little endian)
        /// </summary>
        /// <returns></returns>
        public void WriteF8le(double v)
        {
            WriteBytesNormalisedLittleEndian(BitConverter.GetBytes(v));
        }

        #endregion

        #endregion

        #region Byte arrays

        /// <summary>
        /// Write a fixed number of bytes to the stream
        /// </summary>
        /// <param name="bytes">The byte array to write</param>
        /// <returns></returns>
        public void WriteBytes(byte[] bytes)
        {
            Write(bytes);
        }

        /// <summary>
        /// Write bytes to the stream in little endian format.
        /// </summary>
        /// <param name="bytes">The byte array to write.</param>
        protected void WriteBytesNormalisedLittleEndian(byte[] bytes)
        {
            if (!IsLittleEndian) Array.Reverse(bytes);
            Write(bytes);
        }

        /// <summary>
        /// Read bytes to the stream in big endian format.
        /// </summary>
        /// <param name="bytes">The byte array to write.</param>
        protected void WriteBytesNormalisedBigEndian(byte[] bytes)
        {
            if (IsLittleEndian) Array.Reverse(bytes);
            Write(bytes);
        }

        #endregion
    }
}
