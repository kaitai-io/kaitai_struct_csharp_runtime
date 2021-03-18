using System;

namespace Kaitai
{
    public class ValidationGreaterThanError : ValidationFailedError {
        public ValidationGreaterThanError(byte[] max, byte[] actual, IKaitaiStreamBase io, string srcPath)
            : base("not in range, max " + ByteArrayToHex(max) + ", but got " + ByteArrayToHex(actual), io, srcPath)
        {
            this.max = max;
            this.actual = actual;
        }
        
        public ValidationGreaterThanError(Object max, Object actual, IKaitaiStreamBase io, string srcPath)
            : base("not in range, max " + max + ", but got " + actual, io, srcPath)
        {
            this.max = max;
            this.actual = actual;
        }

        protected Object max;
        protected Object actual;
    }
}
