namespace Kaitai.Async
{
  public abstract class KaitaiAsyncStruct
  {
    protected readonly KaitaiAsyncStream m_io;

    protected KaitaiAsyncStruct(KaitaiAsyncStream kaitaiStream)
    {
      m_io = kaitaiStream;
    }

    public KaitaiAsyncStream M_Io => m_io;
  }
}