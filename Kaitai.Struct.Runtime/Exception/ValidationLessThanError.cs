using System;

namespace Kaitai
{
    public class ValidationLessThanError : ValidationFailedError {
        public ValidationLessThanError(byte[] min, byte[] actual, IKaitaiStreamBase io, string srcPath)
            : base("not in range, min " + ByteArrayToHex(min) + ", but got " + ByteArrayToHex(actual), io, srcPath)
        {
            this.min = min;
            this.actual = actual;
        }
        public ValidationLessThanError(Object min, Object actual, IKaitaiStreamBase io, string srcPath)  
            : base("not in range, min " + min + ", but got " + actual, io, srcPath)
        {
            this.min = min;
            this.actual = actual;
        }

        protected Object min;
        protected Object actual;
    }
}
