using System;

namespace Kaitai
{
    public abstract class KaitaiStruct
    {
        protected KaitaiStream _stream;

        public KaitaiStream GetKaitaiStream()
        {
            return _stream;
        }

        public KaitaiStruct(KaitaiStream stream)
        {
            _stream = stream;
        }
    }
}