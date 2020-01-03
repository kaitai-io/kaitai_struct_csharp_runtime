using System;

namespace Kaitai
{
    /// <summary>
    /// Common ancestor for all error originating from Kaitai Struct usage.
    /// Stores KSY source path, pointing to an element supposedly guilty of
    /// an error.
    /// </summary>
    public class KaitaiStructError : Exception
    {
        protected string srcPath;

        public KaitaiStructError(string msg, string srcPath)
            : base($"srcPath: {msg}")
        {
            this.srcPath = srcPath;
        }
    }
}
