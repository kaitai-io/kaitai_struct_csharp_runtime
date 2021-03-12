using System;

namespace Kaitai
{
    public class ValidationExprError : ValidationFailedError {
        public ValidationExprError(Object actual, KaitaiStream io, string srcPath)
            : base("not matching the expression, got " + actual, io, srcPath)
        {
            this.actual = actual;
        }

        protected Object actual;
    }
}
