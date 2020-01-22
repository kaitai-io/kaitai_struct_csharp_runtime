using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Kaitai.Async
{
  internal class PipeReaderContext : IReaderContext
  {
    private readonly PipeReader _pipeReader;
    private ReadResult _readResult;

    public PipeReaderContext(PipeReader pipeReader)
    {
      _pipeReader = pipeReader;
    }

    private long RemainingBytesInReadResult => _readResult.Buffer.Length - Position;

    public long Position { get; private set; }

    public async ValueTask<long> GetSize()
    {
      await FillReadResultBufferToTheEnd();

      return _readResult.Buffer.Length;
    }

    public async ValueTask<bool> IsEof()
    {
      if (_readResult.Equals(default(ReadResult)) ||
          Position >= _readResult.Buffer.Length && !_readResult.IsCompleted)
      {
        _pipeReader.AdvanceTo(_readResult.Buffer.Start, _readResult.Buffer.GetPosition(Position));
        _readResult = await _pipeReader.ReadAsync();
      }

      return Position >= _readResult.Buffer.Length && _readResult.IsCompleted;
    }


    public async ValueTask SeekAsync(long position)
    {
      if (position <= Position)
      {
        Position = position;
      }
      else
      {
        while (_readResult.Buffer.Length < position && !_readResult.IsCompleted)
        {
          _pipeReader.AdvanceTo(_readResult.Buffer.Start, _readResult.Buffer.End);
          _readResult = await _pipeReader.ReadAsync();
        }

        if (_readResult.Buffer.Length <= position)
        {
          Position = position;
          return;
        }

        if (_readResult.IsCompleted)
        {
          throw new EndOfStreamException(
            $"requested {position} bytes, but got only {RemainingBytesInReadResult} bytes");
        }
      }
    }

    public async ValueTask<byte> ReadByteAsync()
    {
      var value = byte.MinValue;
      while (!TryReadByte(out value) && !_readResult.IsCompleted)
      {
        _pipeReader.AdvanceTo(_readResult.Buffer.Start, _readResult.Buffer.GetPosition(Position));
        _readResult = await _pipeReader.ReadAsync();
      }

      Position += 1;
      return value;

      bool TryReadByte(out byte readValue)
      {
        var sequenceReader = new SequenceReader<byte>(_readResult.Buffer);
        sequenceReader.Advance(Position);
        return sequenceReader.TryRead(out readValue);
      }
    }

    public async ValueTask<byte[]> ReadBytesAsync(long count)
    {
      if (count < 0 || count > int.MaxValue)
      {
        throw new ArgumentOutOfRangeException(
          $"requested {count} bytes, while only non-negative int32 amount of bytes possible");
      }

      byte[] value = null;

      while (!TryRead(out value, count))
      {
        if (_readResult.IsCompleted)
        {
          throw new EndOfStreamException(
            $"requested {count} bytes, but got only {RemainingBytesInReadResult} bytes");
        }

        _pipeReader.AdvanceTo(_readResult.Buffer.Start, _readResult.Buffer.GetPosition(Position));
        _readResult = await _pipeReader.ReadAsync();
      }

      Position += count;
      return value;

      bool TryRead(out byte[] readBytes, long readBytesCount)
      {
        if (RemainingBytesInReadResult < readBytesCount)
        {
          readBytes = null;
          return false;
        }

        readBytes = _readResult.Buffer.Slice(Position, readBytesCount).ToArray();
        return true;
      }
    }

    public virtual async ValueTask<byte[]> ReadBytesFullAsync()
    {
      await FillReadResultBufferToTheEnd();

      _pipeReader.AdvanceTo(_readResult.Buffer.Start, _readResult.Buffer.End);
      var value = _readResult.Buffer.Slice(Position, _readResult.Buffer.End).ToArray();
      Position += value.Length;
      return value;
    }

    private async Task FillReadResultBufferToTheEnd()
    {
      while (!_readResult.IsCompleted)
      {
        _pipeReader.AdvanceTo(_readResult.Buffer.Start, _readResult.Buffer.End);
        _readResult = await _pipeReader.ReadAsync();
      }
    }
  }
}