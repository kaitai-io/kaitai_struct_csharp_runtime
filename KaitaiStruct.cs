using System;

namespace Kaitai
{
    public class KaitaiStruct
    {
        protected KaitaiStream m_io;
        protected KaitaiStruct m_parent;

        public KaitaiStruct(KaitaiStream _io, KaitaiStruct _parent = null)
        {
            m_io = _io;
            m_parent = _parent;
        }

        public KaitaiStream _io() { return m_io; }
        public KaitaiStruct _parent() { return m_parent; }
    }
}
