using System;

namespace Kaitai
{
    public class KaitaiStruct
    {
        protected KaitaiStream m_io;

        public KaitaiStruct(KaitaiStream _io)
        {
            m_io = _io;
        }

        public KaitaiStream _io() { return m_io; }
    }
}
