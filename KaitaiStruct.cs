using System;

namespace Kaitai
{
    public abstract class KaitaiStruct
    {
        protected KaitaiStream m_io;

        public KaitaiStream M_Io
        {
            get
            {
                return m_io;
            }
        }

        public KaitaiStruct(KaitaiStream io)
        {
            m_io = io;
        }
    }
}