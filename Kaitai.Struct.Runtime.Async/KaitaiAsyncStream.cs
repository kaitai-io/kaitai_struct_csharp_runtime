using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Kaitai.Async
{
  public class KaitaiAsyncStream : KaitaiStreamBase, IKaitaiAsyncStream
  {
    protected readonly IReaderContext ReaderContext;
    private ulong _bits;
    private int _bitsLeft;

    #region Constructors

    public KaitaiAsyncStream(IReaderContext readerContext)
    {
      ReaderContext = readerContext;
    }

    public KaitaiAsyncStream(PipeReader pipeReader)
    {
      ReaderContext = new PipeReaderContext(pipeReader);
    }

    public KaitaiAsyncStream(Stream stream)
    {
      ReaderContext = new StreamReaderContext(stream);
    }

    /// <summary>
    /// Creates a IKaitaiAsyncStream backed by a file (RO)
    /// </summary>
    public KaitaiAsyncStream(string file) : this(File.Open(file,
      FileMode.Open,
      FileAccess.Read,
      FileShare.Read))
    {
    }

    ///<summary>
    ///Creates a IKaitaiAsyncStream backed by a byte buffer
    ///</summary>
    public KaitaiAsyncStream(byte[] bytes) : this(new MemoryStream(bytes))
    {
    }

    #endregion

    #region Stream positioning

    public override bool IsEof =>
      ReaderContext.IsEofAsync().GetAwaiter().GetResult() && _bitsLeft == 0;

    public async ValueTask<bool> IsEofAsync(CancellationToken cancellationToken = default) => await ReaderContext.IsEofAsync(cancellationToken) && _bitsLeft == 0;

    public ValueTask<long> GetSizeAsync(CancellationToken cancellationToken = default) => ReaderContext.GetSizeAsync(cancellationToken);

    public virtual async Task SeekAsync(long position, CancellationToken cancellationToken = default) => await ReaderContext.SeekAsync(position, cancellationToken);
    public virtual async Task SeekAsync(ulong position, CancellationToken cancellationToken = default) => await SeekAsync((long)position, cancellationToken);

    public override long Pos => ReaderContext.Position;

    public override long Size => ReaderContext.GetSizeAsync().GetAwaiter().GetResult();

    #endregion

    #region Integer types

    #region Signed

    public async Task<sbyte> ReadS1Async(CancellationToken cancellationToken = default) => (sbyte) await ReadU1Async(cancellationToken);

    #region Big-endian

    public async Task<short> ReadS2beAsync(CancellationToken cancellationToken = default) => BitConverter.ToInt16(await ReadBytesNormalisedBigEndianAsync(2, cancellationToken), 0);

    public async Task<int> ReadS4beAsync(CancellationToken cancellationToken = default) => BitConverter.ToInt32(await ReadBytesNormalisedBigEndianAsync(4, cancellationToken), 0);

    public async Task<long> ReadS8beAsync(CancellationToken cancellationToken = default) => BitConverter.ToInt64(await ReadBytesNormalisedBigEndianAsync(8, cancellationToken), 0);

    #endregion

    #region Little-endian

    public async Task<short> ReadS2leAsync(CancellationToken cancellationToken = default) => BitConverter.ToInt16(await ReadBytesNormalisedLittleEndianAsync(2, cancellationToken), 0);

    public async Task<int> ReadS4leAsync(CancellationToken cancellationToken = default) => BitConverter.ToInt32(await ReadBytesNormalisedLittleEndianAsync(4, cancellationToken), 0);

    public async Task<long> ReadS8leAsync(CancellationToken cancellationToken = default) => BitConverter.ToInt64(await ReadBytesNormalisedLittleEndianAsync(8, cancellationToken), 0);

    #endregion

    #endregion

    #region Unsigned

    public async Task<byte> ReadU1Async(CancellationToken cancellationToken = default) => await ReaderContext.ReadByteAsync(cancellationToken);

    #region Big-endian

    public async Task<ushort> ReadU2beAsync(CancellationToken cancellationToken = default) => BitConverter.ToUInt16(await ReadBytesNormalisedBigEndianAsync(2, cancellationToken), 0);

    public async Task<uint> ReadU4beAsync(CancellationToken cancellationToken = default) => BitConverter.ToUInt32(await ReadBytesNormalisedBigEndianAsync(4, cancellationToken), 0);

    public async Task<ulong> ReadU8beAsync(CancellationToken cancellationToken = default) => BitConverter.ToUInt64(await ReadBytesNormalisedBigEndianAsync(8, cancellationToken), 0);

    #endregion

    #region Little-endian

    public async Task<ushort> ReadU2leAsync(CancellationToken cancellationToken = default) =>
      BitConverter.ToUInt16(await ReadBytesNormalisedLittleEndianAsync(2, cancellationToken), 0);

    public async Task<uint> ReadU4leAsync(CancellationToken cancellationToken = default) => BitConverter.ToUInt32(await ReadBytesNormalisedLittleEndianAsync(4, cancellationToken), 0);

    public async Task<ulong> ReadU8leAsync(CancellationToken cancellationToken = default) => BitConverter.ToUInt64(await ReadBytesNormalisedLittleEndianAsync(8, cancellationToken), 0);

    #endregion

    #endregion

    #endregion

    #region Floating point types

    #region Big-endian

    public async Task<float> ReadF4beAsync(CancellationToken cancellationToken = default) => BitConverter.ToSingle(await ReadBytesNormalisedBigEndianAsync(4, cancellationToken), 0);

    public async Task<double> ReadF8beAsync(CancellationToken cancellationToken = default) => BitConverter.ToDouble(await ReadBytesNormalisedBigEndianAsync(8, cancellationToken), 0);

    #endregion

    #region Little-endian

    public async Task<float> ReadF4leAsync(CancellationToken cancellationToken = default) => BitConverter.ToSingle(await ReadBytesNormalisedLittleEndianAsync(4, cancellationToken), 0);

    public async Task<double> ReadF8leAsync(CancellationToken cancellationToken = default) =>
      BitConverter.ToDouble(await ReadBytesNormalisedLittleEndianAsync(8, cancellationToken), 0);

    #endregion

    #endregion

    #region Unaligned bit values

    public override void AlignToByte()
    {
      _bits = 0;
      _bitsLeft = 0;
    }

    public async Task<ulong> ReadBitsIntBeAsync(int n, CancellationToken cancellationToken = default)
    {
      int bitsNeeded = n - _bitsLeft;
      if (bitsNeeded > 0)
      {
        // 1 bit  => 1 byte
        // 8 bits => 1 byte
        // 9 bits => 2 bytes
        int bytesNeeded = (bitsNeeded - 1) / 8 + 1;
        var buf = await ReadBytesAsync(bytesNeeded, cancellationToken);
        for (var i = 0; i < buf.Length; i++)
        {
          _bits <<= 8;
          _bits |= buf[i];
          _bitsLeft += 8;
        }
      }

      // raw mask with required number of 1s, starting from lowest bit
      ulong mask = GetMaskOnes(n);
      // shift mask to align with highest bits available in "bits"
      int shiftBits = _bitsLeft - n;
      mask = mask << shiftBits;
      // derive reading result
      ulong res = (_bits & mask) >> shiftBits;
      // clear top bits that we've just read => AND with 1s
      _bitsLeft -= n;
      mask = GetMaskOnes(_bitsLeft);
      _bits &= mask;

      return res;
    }

    public Task<ulong> ReadBitsIntAsync(int n, CancellationToken cancellationToken = default) =>
      ReadBitsIntBeAsync(n, cancellationToken);


    //Method ported from algorithm specified @ issue#155
    public async Task<ulong> ReadBitsIntLeAsync(int n, CancellationToken cancellationToken = default)
    {
      int bitsNeeded = n - _bitsLeft;

      if (bitsNeeded > 0)
      {
        // 1 bit  => 1 byte
        // 8 bits => 1 byte
        // 9 bits => 2 bytes
        int bytesNeeded = (bitsNeeded - 1) / 8 + 1;
        var buf = await ReadBytesAsync(bytesNeeded, cancellationToken);
        for (var i = 0; i < buf.Length; i++)
        {
          ulong v = (ulong) buf[i] << _bitsLeft;
          _bits |= v;
          _bitsLeft += 8;
        }
      }

      // raw mask with required number of 1s, starting from lowest bit
      ulong mask = GetMaskOnes(n);

      // derive reading result
      ulong res = _bits & mask;

      // remove bottom bits that we've just read by shifting
      _bits >>= n;
      _bitsLeft -= n;

      return res;
    }

    #endregion

    #region Byte arrays

    public async Task<byte[]> ReadBytesAsync(long count, CancellationToken cancellationToken = default) => await ReaderContext.ReadBytesAsync(count, cancellationToken);

    public async Task<byte[]> ReadBytesAsync(ulong count, CancellationToken cancellationToken = default)
    {
      if (count > int.MaxValue)
      {
        throw new ArgumentOutOfRangeException(
          $"requested {count} bytes, while only non-negative int32 amount of bytes possible");
      }

      return await ReadBytesAsync((long) count, cancellationToken);
    }

    /// <summary>
    /// Read bytes from the stream in little endian format and convert them to the endianness of the current platform
    /// </summary>
    /// <param name="count">The number of bytes to read</param>
    /// <returns>An array of bytes that matches the endianness of the current platform</returns>
    protected async Task<byte[]> ReadBytesNormalisedLittleEndianAsync(int count, CancellationToken cancellationToken = default)
    {
      var bytes = await ReadBytesAsync(count, cancellationToken);
      if (!IsLittleEndian)
      {
        Array.Reverse(bytes);
      }

      return bytes;
    }

    /// <summary>
    /// Read bytes from the stream in big endian format and convert them to the endianness of the current platform
    /// </summary>
    /// <param name="count">The number of bytes to read</param>
    /// <returns>An array of bytes that matches the endianness of the current platform</returns>
    protected async Task<byte[]> ReadBytesNormalisedBigEndianAsync(int count, CancellationToken cancellationToken = default)
    {
      var bytes = await ReadBytesAsync(count, cancellationToken);
      if (IsLittleEndian)
      {
        Array.Reverse(bytes);
      }

      return bytes;
    }

    /// <summary>
    /// Read all the remaining bytes from the stream until the end is reached
    /// </summary>
    /// <returns></returns>
    public virtual async Task<byte[]> ReadBytesFullAsync(CancellationToken cancellationToken = default) => await ReaderContext.ReadBytesFullAsync(cancellationToken);

    /// <summary>
    /// Read a terminated string from the stream
    /// </summary>
    /// <param name="terminator">The string terminator value</param>
    /// <param name="includeTerminator">True to include the terminator in the returned string</param>
    /// <param name="consumeTerminator">True to consume the terminator byte before returning</param>
    /// <param name="eosError">True to throw an error when the EOS was reached before the terminator</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<byte[]> ReadBytesTermAsync(byte terminator,
      bool includeTerminator,
      bool consumeTerminator,
      bool eosError,
      CancellationToken cancellationToken = default)
    {
      var bytes = new List<byte>();
      while (true)
      {
        if (await IsEofAsync(cancellationToken))
        {
          if (eosError)
          {
            throw new EndOfStreamException(
              $"End of stream reached, but no terminator `{terminator}` found");
          }

          break;
        }

        byte b = await ReadU1Async(cancellationToken);
        if (b == terminator)
        {
          if (includeTerminator)
          {
            bytes.Add(b);
          }

          if (!consumeTerminator)
          {
            await SeekAsync(Pos - 1, cancellationToken);
          }

          break;
        }

        bytes.Add(b);
      }

      return bytes.ToArray();
    }

    /// <summary>
    /// Read a specific set of bytes and assert that they are the same as an expected result
    /// </summary>
    /// <param name="expected">The expected result</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<byte[]> EnsureFixedContentsAsync(byte[] expected, CancellationToken cancellationToken = default)
    {
      var bytes = await ReadBytesAsync(expected.Length, cancellationToken);

      if (bytes.Length != expected.Length) //TODO Is this necessary?
      {
        throw new Exception(
          $"Expected bytes: {Convert.ToBase64String(expected)} ({expected.Length} bytes), Instead got: {Convert.ToBase64String(bytes)} ({bytes.Length} bytes)");
      }

      for (var i = 0; i < bytes.Length; i++)
      {
        if (bytes[i] != expected[i])
        {
          throw new Exception(
            $"Expected bytes: {Convert.ToBase64String(expected)} ({expected.Length} bytes), Instead got: {Convert.ToBase64String(bytes)} ({bytes.Length} bytes)");
        }
      }

      return bytes;
    }

    #endregion
  }
}