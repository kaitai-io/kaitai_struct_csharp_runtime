namespace Kaitai
{
    public class ValidationLessThanError : ValidationFailedError {
        public ValidationLessThanError(long min, long actual, KaitaiStream io, string srcPath)
            : base("not in range, min " + min + ", but got " + actual, io, srcPath)
        {
            this.min = min;
            this.actual = actual;
        }

        protected long min;
        protected long actual;
    }
}