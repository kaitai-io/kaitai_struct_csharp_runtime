using System;
using System.IO;

namespace Kaitai
{
    public class KaitaiStream 
    {

        private Stream mStream;

#region Constructors
        public KaitaiStream(String filename)
        {
            mStream = (Stream)(new FileStream(filename, FileMode.Open));
        }

        public KaitaiStream(byte[] buffer)
        {
            mStream = new MemoryStream(buffer);
        }

#endregion 


#region Raw stream passthru
        public long pos()
        {
            return mStream.Position;
        }

        public void seek(long position)
        {
            mStream.Seek(position, SeekOrigin.Begin);
        }

        public bool isEof()
        {
            return mStream.Position == mStream.Length;
        }

        public byte[] readBytes(int n)
        {
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

        public SByte readS1()
        {
            int val = mStream.ReadByte();
            if(val == -1)
                throw new Exception("At EOF!");
            else
                return (SByte)val;
        }

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

        public UInt16 readU2le() {
            byte[] tmp = new byte[2];
            mStream.Read(tmp, 0, 2);
            return (UInt16)( (tmp[0] << 8)+(tmp[1] << 0) );
        }
        public UInt16 readU2be() {
            byte[] tmp = new byte[2];
            mStream.Read(tmp, 0, 2);
            return (UInt16)( (tmp[1] << 8)+(tmp[0] << 0) );
        }

#endregion

#region 16-bit integers (signed)
        public Int16 readS2le() {
            byte[] tmp = new byte[2];
            mStream.Read(tmp, 0, 2);
            return (Int16)( (tmp[0] << 8)+(tmp[1] << 0) );
        }

        public Int16 readS2be() {
            byte[] tmp = new byte[2];
            mStream.Read(tmp, 0, 2);
            return (Int16)( (tmp[1] << 8)+(tmp[0] << 0) );
        }
#endregion

#endregion


#region 4-byte integer reads
#region 32-bit integers (Unsigned)
        public UInt32 readU4le() { throw new NotImplementedException(); }
        public UInt32 readU4be() { throw new NotImplementedException(); }
#endregion
#region 32-bit integers (signed)
        public UInt32 readS4le() { throw new NotImplementedException(); }
        public UInt32 readS4be() { throw new NotImplementedException(); }
#endregion
#endregion // 4-byte integer reads


#region 8-byte integer reads
#region 64-bit integers (unsigned)
    public UInt64 readU8le() { throw new NotImplementedException(); }
    public UInt64 readU8be() { throw new NotImplementedException(); }
#endregion
#region 64-bit integers (signed)
    public UInt64 readS8le() { throw new NotImplementedException(); }
    public UInt64 readS8be() { throw new NotImplementedException(); }
#endregion
#endregion // 8-byte integer reads 


        public byte[] readBytesFull()
        {
            int count = mStream.Length - mStream.Position;
            byte[] buffer = new byte[count];
            mStream.Read(buffer, 0, count);
            return buffer;
        }

        public byte[] ensureFixedContents(int len, byte[] expected)
        {

            IOException e = new IOException();
            e.Message = "Expected bytes: "+Convert.ToBase64String(expected)+Environment.NewLine;
            byte[] buff = readBytes(len);
            e.message += "Got bytes: "+Convert.ToBase64String(buff);

            for(int idx =0; idx < len; idx++)
                if(buff[idx] != expected[idx]) throw e;
            return buff;

        }

        public String readStrEos(String encoding)
        {
            System.Text.Encoding _encoding = System.Text.Encoding.GetEncoding(encoding);
            return _encoding.GetString(readBytesFull());
        }
        public String readStrByteLimit(int len, String encoding)
        {
            throw new NotImplementedException();
        }
    }
}