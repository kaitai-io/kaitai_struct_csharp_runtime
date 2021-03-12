namespace Kaitai
{
    public class ValidationGreaterThanError : ValidationFailedError {
        public ValidationGreaterThanError(long max, long actual, KaitaiStream io, string srcPath)
            : base("not in range, max " + max + ", but got " + actual, io, srcPath)
        {
            this.max = max;
            this.actual = actual;
        }

        protected long max;
        protected long actual;
    }
}