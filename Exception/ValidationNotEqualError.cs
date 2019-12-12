using System;

namespace Kaitai
{
    /// <summary>
    /// Signals validation failure: we required "actual" value to be equal to
    /// "expected", but it turned out that it's not.
    /// </summary>
    public class ValidationNotEqualError : ValidationFailedError
    {
        protected Object actual;

        protected Object expected;

        public ValidationNotEqualError(byte[] expected, byte[] actual, KaitaiStream io, string srcPath)
            : base("not equal, expected " + ByteArrayToHex(expected) + ", but got " + ByteArrayToHex(actual), io,
                srcPath)
        {
            this.expected = expected;
            this.actual = actual;
        }

        public ValidationNotEqualError(Object expected, Object actual, KaitaiStream io, string srcPath)
            : base("not equal, expected " + expected + ", but got " + actual, io, srcPath)
        {
            this.expected = expected;
            this.actual = actual;
        }
    }
}
