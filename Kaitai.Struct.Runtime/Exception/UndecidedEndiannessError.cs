using System;

namespace Kaitai
{
    /// <summary>
    /// Error that occurs when default endianness should be decided with a
    /// switch, but nothing matches (although using endianness expression
    /// implies that there should be some positive result).
    /// </summary>
    public class UndecidedEndiannessError : Exception {
        public UndecidedEndiannessError()
            : base("Unable to decide on endianness")
        {
        }
        public UndecidedEndiannessError(string msg)
            : base(msg)
        {
        }
        public UndecidedEndiannessError(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}
