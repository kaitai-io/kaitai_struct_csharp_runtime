using System;
using System.IO;

namespace Kaitai
{
    ///<summary>
    /// KaitaiStream is a special stream wrapping a specific set of functionality
    /// Specifically, it allows (unlike normal Streams) picking the endianness of reads,
    /// as well as signed/unsigned-ness of read values.
    ///</summary>
    public class KaitaiStream 
    {

        private Stream mStream;

#region Constructors
        ///<summary>
        /// Creates a KaitaiStream backed by a file (RO)
        ///</summary>
        public KaitaiStream(String filename)
        {
            mStream = (Stream)(new FileStream(filename, FileMode.Open));
        }

        ///<summary>
        ///Creates a KaitaiStream backed by a byte buffer
        ///</summary>
        public KaitaiStream(byte[] buffer)
        {
            mStream = new MemoryStream(buffer);
        }

#endregion 


#region Raw stream passthru
        ///<summary>Get the position within the stream</summary>
        public long pos()
        {
            return mStream.Position;
        }

        ///<summary>Seek to a specific position from the beginning of the stream</summary>
        public void seek(long position)
        {
            mStream.Seek(position, SeekOrigin.Begin);
        }

        ///<summary>Return if the current stream is at the end of the stream.</summary>
        public bool isEof()
        {
            return mStream.Position == mStream.Length;
        }

        ///<summary>Read N bytes from the stream.</summary>
        public byte[] readBytes(int n)
        {

            if (n > Int32.MaxValue)
            {
                throw new ArgumentException(String.Format("Cannot allocate {0} bytes", n));
            }

            try
            {
                byte[] tmp = new byte[n];
                int readCount = mStream.Read(tmp, 0, n);
                if (readCount < n)
                    throw new IOException("Requested to read " + n + " bytes, but got only " + readCount);
                return tmp;
            }
            catch(Exception e)
            {
                throw new IOException("Failed to read "+n+" bytes from stream", e);
            }
        }

#endregion

#region Read Byte/SByte

        ///<summary> Read 1 signed byte from the stream</summary>
        public SByte readS1()
        {
            int val = mStream.ReadByte();
            if(val == -1)
                throw new Exception("At EOF!");
            else
                return (SByte)val;
        }

        ///<summary>Read 1 unsigned byte fro mthe stream.</summary>
        public byte readU1() 
        { 
            int val = mStream.ReadByte();
            if(val == -1) 
                throw new Exception("At EOF!");
            else
                return (byte)(val);
        }

#endregion

#region 2-byte integer values

#region 16-bit integers (unsigned)

        ///<summary>Read an unsigned 16-bit integer, little endian from the stream.</summary>
        public UInt16 readU2le() {
            byte[] tmp = new byte[2];
            mStream.Read(tmp, 0, 2);
            return (UInt16)(
                (tmp[0] << 8) +
                (tmp[1] << 0) );
        }
        ///<summary>Read an unsigned 16-bit integer, big endian, from the stream.</summary>
        public UInt16 readU2be() {
            byte[] tmp = new byte[2];
            mStream.Read(tmp, 0, 2);
            return (UInt16)(
                (tmp[1] << 8) +
                (tmp[0] << 0) );
        }

#endregion

#region 16-bit integers (signed)
        ///<summary> Read a signed 16-bit integer, little endian, from the stream.</summary>
        public Int16 readS2le() {
            byte[] tmp = new byte[2];
            mStream.Read(tmp, 0, 2);
            return (Int16)(
                (tmp[0] << 8) +
                (tmp[1] << 0) );
        }

        ///<summary> Read a signed 16-bit integer, big endian, from the stream.</summary>
        public Int16 readS2be() {
            byte[] tmp = new byte[2];
            mStream.Read(tmp, 0, 2);
            return (Int16)(
                (tmp[1] << 8) +
                (tmp[0] << 0) );
        }
#endregion

#endregion


#region 4-byte integer reads
#region 32-bit integers (Unsigned)
        ///<summary>Read a 32-bit little-endian unsigned integer</summary>
        public UInt32 readU4le()
        {
            byte[] buffer = readBytes(4);
            return (UInt32)(
                ( buffer[3] << 24 ) + 
                ( buffer[2] << 16 ) + 
                ( buffer[1] << 8  ) + 
                ( buffer[0] << 0  ) );

        }
        ///<summary>Read a 32-bit big-endian unsigned integer</summary>
        public UInt32 readU4be()
        {
            byte[] buffer = readBytes(4);
            return (UInt32)(
                ( buffer[0] << 24 ) +
                ( buffer[1] << 16 ) + 
                ( buffer[2] << 8  ) + 
                ( buffer[3] << 0  ) );
        }
#endregion
#region 32-bit integers (signed)
        ///<summary>Read a 32-bit little-endian signed integer</summary>
        public Int32 readS4le()
        {
            byte[] buffer = readBytes(4);
            return (Int32)(
                ( buffer[3] << 24 ) +
                ( buffer[2] << 16 ) +
                ( buffer[1] << 8  ) +
                ( buffer[0] << 0  ) );
        }
        ///<summary>Read a 32-bit big-endian signed integer</summary>
        public Int32 readS4be() 
        {
            byte[] buffer = readBytes(4);
            return (Int32)(
                ( buffer[0] << 24 ) +
                ( buffer[1] << 16 ) +
                ( buffer[2] << 8  ) +
                ( buffer[3] << 0  ) );
        }
#endregion
#endregion // 4-byte integer reads


#region 8-byte integer reads
#region 64-bit integers (unsigned)
        ///<summary>Read a 64-bit little-endian unsigned integer</summary>
        public UInt64 readU8le() { throw new NotImplementedException(); }
        ///<summary>Read a 64-bit big-endian unsigned integer</summary>
         public UInt64 readU8be() { throw new NotImplementedException(); }
#endregion
#region 64-bit integers (signed)
        ///<summary>Read a 64-bit little-endian signed integer</summary>
        public UInt64 readS8le() { throw new NotImplementedException(); }
        ///<summary>Read a 64-bit big-endian signed integer</summary>
        public UInt64 readS8be() { throw new NotImplementedException(); }
#endregion
#endregion // 8-byte integer reads 

        ///<summary>Read all remaining bytes from the stream.</summary>
        public byte[] readBytesFull()
        {
            long countLong = mStream.Length - mStream.Position;
            // TODO: check if the conversion is safe, throw exception otherwise?
            int count = (int) countLong;
            byte[] buffer = new byte[count];
            mStream.Read(buffer, 0, count);
            return buffer;
        }

        ///<summary>Read a specific set of bytes, expecting a specific result.</summary>
        public byte[] ensureFixedContents(int len, byte[] expected)
        {
            byte[] buff = readBytes(len);

            for (int idx = 0; idx < len; idx++)
                if (buff[idx] != expected[idx])
                {
                    String msg = "Expected bytes: "+Convert.ToBase64String(expected)+Environment.NewLine;
                    msg += "Got bytes: "+Convert.ToBase64String(buff);
                    throw new IOException(msg);
                }
            return buff;

        }

        ///<summary>Read a string until the end of the stream.</summary>
        public String readStrEos(String encoding)
        {
            System.Text.Encoding _encoding = System.Text.Encoding.GetEncoding(encoding);
            return _encoding.GetString(readBytesFull());
        }

        ///<summary>Read a string from len characters in the specified encoding.</summary>
        public String readStrByteLimit(int len, String encoding)
        {
            throw new NotImplementedException();
        }
        
        ///<summary>Read a string in a specific encoding, with a specific terminator, etc.</summary>
        public String readStrz(String encoding, byte term, bool includeTerm, bool consumeTerm, bool eosError)
        {
            throw new NotImplementedException();
        }
        
        ///<summary>Inflate a Zlib block</summary>
        public byte[] processZlib(byte[] data)
        {
            throw new NotImplementedException();
        }
        
        ///<summary>Barrel-rotate</summary>
        public byte[] processRotateLeft(byte[] data, int amount, int groupSize)
        {
            throw new NotImplementedException();
        }
    }
}