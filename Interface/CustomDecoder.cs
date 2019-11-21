namespace Kaitai
{
    /// <summary>
    ///     A custom decoder interface. Implementing classes can be called from
    ///     inside a .ksy file using `process: XXX` syntax.
    /// </summary>
    public interface CustomDecoder
    {
        /// <summary>
        ///     Decodes a given byte array, according to some custom algorithm
        ///     (specific to implementing class) and parameters given in the
        ///     constructor, returning another byte array.
        /// </summary>
        /// <param name="src">Source byte array.</param>
        byte[] Decode(byte[] src);
    }
}
