using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
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

    public virtual async ValueTask<long> GetSizeAsync(CancellationToken cancellationToken = default)
    {
      await FillReadResultBufferToTheEndAsync(cancellationToken);

      return ReadResult.Buffer.Length;
    }

    public virtual async ValueTask<bool> IsEofAsync(CancellationToken cancellationToken = default)
    {
      await EnsureReadResultIsNotDefaultAsync(cancellationToken);

      if (Position >= ReadResult.Buffer.Length && !ReadResult.IsCompleted)
      {
        PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.GetPosition(Position));
        ReadResult = await PipeReader.ReadAsync(cancellationToken);
      }

      return Position >= ReadResult.Buffer.Length && ReadResult.IsCompleted;
    }


    public virtual async ValueTask SeekAsync(long position, CancellationToken cancellationToken = default)
    {
      if (position <= Position)
      {
        Position = position;
      }
      else
      {
        await EnsureReadResultIsNotDefaultAsync(cancellationToken);

        while (ReadResult.Buffer.Length < position && !ReadResult.IsCompleted)
        {
          PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.End);
          ReadResult = await PipeReader.ReadAsync(cancellationToken);
        }

        if (ReadResult.Buffer.Length >= position)
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

    public virtual async ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default)
    {
      await EnsureReadResultIsNotDefaultAsync(cancellationToken);

      var value = byte.MinValue;
      while (!TryReadByte(out value) && !ReadResult.IsCompleted)
      {
        PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.GetPosition(Position));
        ReadResult = await PipeReader.ReadAsync(cancellationToken);
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

    public virtual async ValueTask<byte[]> ReadBytesAsync(long count, CancellationToken cancellationToken = default)
    {
      if (count < 0 || count > int.MaxValue)
      {
        throw new ArgumentOutOfRangeException(
          $"requested {count} bytes, while only non-negative int32 amount of bytes possible");
      }

      await EnsureReadResultIsNotDefaultAsync(cancellationToken);

      byte[] value = null;

      while (!TryRead(out value, count))
      {
        if (ReadResult.IsCompleted)
        {
          throw new EndOfStreamException(
            $"requested {count} bytes, but got only {RemainingBytesInReadResult} bytes");
        }

        PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.GetPosition(Position));
        ReadResult = await PipeReader.ReadAsync(cancellationToken);
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

    public virtual async ValueTask<byte[]> ReadBytesFullAsync(CancellationToken cancellationToken = default)
    {
      await FillReadResultBufferToTheEndAsync(cancellationToken);

      PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.End);
      var value = ReadResult.Buffer.Slice(Position, ReadResult.Buffer.End).ToArray();
      Position += value.Length;
      return value;
    }

    private async ValueTask FillReadResultBufferToTheEndAsync(CancellationToken cancellationToken = default)
    {
      await EnsureReadResultIsNotDefaultAsync(cancellationToken);

      while (!ReadResult.IsCompleted)
      {
        PipeReader.AdvanceTo(ReadResult.Buffer.Start, ReadResult.Buffer.End);
        ReadResult = await PipeReader.ReadAsync(cancellationToken);
      }
    }

    private async ValueTask EnsureReadResultIsNotDefaultAsync(CancellationToken cancellationToken = default)
    {
      if (ReadResult.Equals(default(ReadResult)))
      {
        ReadResult = await PipeReader.ReadAsync(cancellationToken);
      }
    }
  }
}