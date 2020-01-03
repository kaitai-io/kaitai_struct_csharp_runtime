using System;
using System.IO;
using System.IO.Compression;

namespace Kaitai
{
    public class Utilities
    {
        /// <summary>
        /// Performs XOR processing with given data, XORing every byte of the input with a single value.
        /// </summary>
        /// <param name="value">The data toe process</param>
        /// <param name="key">The key value to XOR with</param>
        /// <returns>Processed data</returns>
        public static byte[] ProcessXor(byte[] value, int key)
        {
            byte[] result = new byte[value.Length];
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
        public static byte[] ProcessXor(byte[] value, byte[] key)
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
        /// Performs a circular left rotation shift for a given buffer by a given amount of bits.
        /// Pass a negative amount to rotate right.
        /// </summary>
        /// <param name="data">The data to rotate</param>
        /// <param name="amount">The number of bytes to rotate by</param>
        /// <param name="groupSize"></param>
        /// <returns></returns>
        public static byte[] ProcessRotateLeft(byte[] data, int amount, int groupSize)
        {
            if (amount > 7 || amount < -7)
                throw new ArgumentException("Rotation of more than 7 cannot be performed.", "amount");
            if (amount < 0) amount += 8; // Rotation of -2 is the same as rotation of +6

            byte[] r = new byte[data.Length];
            switch (groupSize)
            {
                case 1:
                    for (int i = 0; i < data.Length; i++)
                    {
                        byte bits = data[i];
                        // http://stackoverflow.com/a/812039
                        r[i] = (byte)((bits << amount) | (bits >> (8 - amount)));
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
        public static byte[] ProcessZlib(byte[] data)
        {
            // See RFC 1950 (https://tools.ietf.org/html/rfc1950)
            // zlib adds a header to DEFLATE streams - usually 2 bytes,
            // but can be 6 bytes if FDICT is set.
            // There's also 4 checksum bytes at the end of the stream.

            byte zlibCmf = data[0];
            if ((zlibCmf & 0x0F) != 0x08)
                throw new NotSupportedException("Only the DEFLATE algorithm is supported for zlib data.");

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

        public static byte[] BytesStripRight(byte[] src, byte padByte)
        {
            int newLen = src.Length;
            while (newLen > 0 && src[newLen - 1] == padByte)
                newLen--;

            byte[] dst = new byte[newLen];
            Array.Copy(src, dst, newLen);
            return dst;
        }

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

        /// <summary>
        /// Performs modulo operation between two integers.
        /// </summary>
        /// <remarks>
        /// This method is required because C# lacks a "true" modulo
        /// operator, the % operator rather being the "remainder"
        /// operator. We want mod operations to always be positive.
        /// </remarks>
        /// <param name="a">The value to be divided</param>
        /// <param name="b">The value to divide by. Must be greater than zero.</param>
        /// <returns>The result of the modulo opertion. Will always be positive.</returns>
        public static int Mod(int a, int b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            int r = a % b;
            if (r < 0) r += b;
            return r;
        }

        /// <summary>
        /// Performs modulo operation between two integers.
        /// </summary>
        /// <remarks>
        /// This method is required because C# lacks a "true" modulo
        /// operator, the % operator rather being the "remainder"
        /// operator. We want mod operations to always be positive.
        /// </remarks>
        /// <param name="a">The value to be divided</param>
        /// <param name="b">The value to divide by. Must be greater than zero.</param>
        /// <returns>The result of the modulo opertion. Will always be positive.</returns>
        public static long Mod(long a, long b)
        {
            if (b <= 0) throw new ArgumentException("Divisor of mod operation must be greater than zero.", "b");
            long r = a % b;
            if (r < 0) r += b;
            return r;
        }

        /// <summary>
        /// Compares two byte arrays in lexicographical order.
        /// </summary>
        /// <returns>negative number if a is less than b, <c>0</c> if a is equal to b, positive number if a is greater than b.</returns>
        /// <param name="a">First byte array to compare</param>
        /// <param name="b">Second byte array to compare.</param>
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
            if (al == bl)
            {
                return 0;
            }

            return al - bl;
        }
    }
}
