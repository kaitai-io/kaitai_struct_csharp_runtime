namespace Kaitai.Async
{
  public abstract class KaitaiAsyncStruct
  {
    protected readonly KaitaiAsyncStream m_io;

    protected KaitaiAsyncStruct(KaitaiAsyncStream kaitaiStream)
    {
      m_io = kaitaiStream;
    }

    // ReSharper disable once InconsistentNaming
    public KaitaiAsyncStream M_Io => m_io;
  }
}