using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Kaitai.Async
{
  public class PipeReaderContext : IReaderContext
  {
    protected readonly PipeReader PipeReader;
    protected ReadResult ReadResult;

    public PipeReaderContext(PipeReader pipeReader)
    {
      PipeReader = pipeReader;
    }

    protected long RemainingBytesInReadResult => ReadResult.Buffer.Length - Position;

    public long Position { get; protected set; }

    public virtual async ValueTask<long> GetSizeAsync()
    {
      await FillReadResultBufferToTheEnd();

      return ReadResult.Buffer.Length;
    }

    public virtual async ValueTask<bool> IsEofAsync()
    {
      await EnsureReadResultIsNotDefault();

      if (Position >= ReadResult.Buffer.Length && !ReadResult.IsCompleted)
      {
        PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.GetPosition(Position));
        ReadResult = await PipeReader.ReadAsync();
      }

      return Position >= ReadResult.Buffer.Length && ReadResult.IsCompleted;
    }


    public virtual async ValueTask SeekAsync(long position)
    {
      if (position <= Position)
      {
        Position = position;
      }
      else
      {
        await EnsureReadResultIsNotDefault();

        while (ReadResult.Buffer.Length < position && !ReadResult.IsCompleted)
        {
          PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.End);
          ReadResult = await PipeReader.ReadAsync();
        }

        if (ReadResult.Buffer.Length <= position)
        {
          Position = position;
          return;
        }

        if (ReadResult.IsCompleted)
        {
          throw new EndOfStreamException(
            $"requested {position} bytes, but got only {RemainingBytesInReadResult} bytes");
        }
      }
    }

    public virtual async ValueTask<byte> ReadByteAsync()
    {
      await EnsureReadResultIsNotDefault();

      var value = byte.MinValue;
      while (!TryReadByte(out value) && !ReadResult.IsCompleted)
      {
        PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.GetPosition(Position));
        ReadResult = await PipeReader.ReadAsync();
      }

      Position += 1;
      return value;

      bool TryReadByte(out byte readValue)
      {
        var sequenceReader = new SequenceReader<byte>(ReadResult.Buffer);
        sequenceReader.Advance(Position);
        return sequenceReader.TryRead(out readValue);
      }
    }

    public virtual async ValueTask<byte[]> ReadBytesAsync(long count)
    {
      if (count < 0 || count > int.MaxValue)
      {
        throw new ArgumentOutOfRangeException(
          $"requested {count} bytes, while only non-negative int32 amount of bytes possible");
      }

      await EnsureReadResultIsNotDefault();

      byte[] value = null;

      while (!TryRead(out value, count))
      {
        if (ReadResult.IsCompleted)
        {
          throw new EndOfStreamException(
            $"requested {count} bytes, but got only {RemainingBytesInReadResult} bytes");
        }

        PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.GetPosition(Position));
        ReadResult = await PipeReader.ReadAsync();
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

        readBytes = ReadResult.Buffer.Slice(Position, readBytesCount).ToArray();
        return true;
      }
    }

    public virtual async ValueTask<byte[]> ReadBytesFullAsync()
    {
      await FillReadResultBufferToTheEnd();

      PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.End);
      var value = ReadResult.Buffer.Slice(Position, ReadResult.Buffer.End).ToArray();
      Position += value.Length;
      return value;
    }

    private async ValueTask FillReadResultBufferToTheEnd()
    {
      await EnsureReadResultIsNotDefault();

      while (!ReadResult.IsCompleted)
      {
        PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.End);
        ReadResult = await PipeReader.ReadAsync();
      }
    }

    private async ValueTask EnsureReadResultIsNotDefault()
    {
      if (ReadResult.Equals(default(ReadResult)))
      {
        ReadResult = await PipeReader.ReadAsync();
      }
    }
  }
}