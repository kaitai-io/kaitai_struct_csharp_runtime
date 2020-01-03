namespace Kaitai
{
    public interface IKaitaiStreamBase
    {
        /// <summary>
        /// Check if the stream position is at the end of the stream
        /// </summary>
        bool IsEof { get; }

        /// <summary>
        /// Get the current position in the stream
        /// </summary>
        long Pos { get; }

        /// <summary>
        /// Get the total length of the stream (ie. file size)
        /// </summary>
        long Size { get; }

        void AlignToByte();

        /// <summary>
        /// Performs XOR processing with given data, XORing every byte of the input with a single value.
        /// </summary>
        /// <param name="value">The data toe process</param>
        /// <param name="key">The key value to XOR with</param>
        /// <returns>Processed data</returns>
        byte[] ProcessXor(byte[] value, int key);

        /// <summary>
        /// Performs XOR processing with given data, XORing every byte of the input with a key
        /// array, repeating from the beginning of the key array if necessary
        /// </summary>
        /// <param name="value">The data toe process</param>
        /// <param name="key">The key array to XOR with</param>
        /// <returns>Processed data</returns>
        byte[] ProcessXor(byte[] value, byte[] key);

        /// <summary>
        /// Performs a circular left rotation shift for a given buffer by a given amount of bits.
        /// Pass a negative amount to rotate right.
        /// </summary>
        /// <param name="data">The data to rotate</param>
        /// <param name="amount">The number of bytes to rotate by</param>
        /// <param name="groupSize"></param>
        /// <returns></returns>
        byte[] ProcessRotateLeft(byte[] data, int amount, int groupSize);

        /// <summary>
        /// Inflates a deflated zlib byte stream
        /// </summary>
        /// <param name="data">The data to deflate</param>
        /// <returns>The deflated result</returns>
        byte[] ProcessZlib(byte[] data);
    }
}
