namespace Kaitai
{
    public abstract class KaitaiStruct
    {
        protected KaitaiStream m_io;

        protected KaitaiStruct(KaitaiStream io)
        {
            m_io = io;
        }

        public KaitaiStream M_Io
        {
            get
            {
                return m_io;
            }
        }
    }
}
