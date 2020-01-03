using System.Text;

namespace Kaitai
{
    /// <summary>
    /// Common ancestor for all validation failures. Stores pointer to
    /// KaitaiStream IO object which was involved in an error.
    /// </summary>
    public class ValidationFailedError : KaitaiStructError
    {
        protected IKaitaiStreamBase io;

        public ValidationFailedError(string msg, IKaitaiStreamBase io, string srcPath)
            : base($"at pos {io.Pos}: validation failed: {msg}", srcPath)
        {
            this.io = io;
        }

        protected static string ByteArrayToHex(byte[] arr)
        {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(' ');
                }

                sb.Append($"{arr[i]:X2}");
            }

            sb.Append(']');
            return sb.ToString();
        }
    }
}
