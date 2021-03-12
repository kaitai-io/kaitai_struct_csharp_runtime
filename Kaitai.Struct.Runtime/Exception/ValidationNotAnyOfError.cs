using System;

namespace Kaitai
{
    public class ValidationNotAnyOfError : ValidationFailedError {
        public ValidationNotAnyOfError(Object actual, KaitaiStream io, string srcPath)
            : base("not any of the list, got " + actual, io, srcPath)
        {
            this.actual = actual;
        }

        protected Object actual;
    }
}
