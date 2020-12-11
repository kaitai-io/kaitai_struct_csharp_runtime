using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
    public class PipeReaderContextTests
    {
        private readonly PipeReaderContext _pipeReaderContextSUT;
        private readonly PipeWriter _pipeWriter;

        public PipeReaderContextTests()
        {
            var pipe = new Pipe(new PipeOptions(minimumSegmentSize: 2));
            _pipeWriter = pipe.Writer;
            _pipeReaderContextSUT = new PipeReaderContext(pipe.Reader);
        }

        [Fact]
        public async Task ReadByteAsync_RequestingMoreData_ShouldAwaitOnRead()
        {
            //Arrange
            await _pipeWriter.WriteAsync(new byte[1]);

            await _pipeReaderContextSUT.ReadByteAsync(CancellationToken.None);
            using var cts = new CancellationTokenSource(100);
            
            //Act & assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                async () => await _pipeReaderContextSUT.ReadByteAsync(cts.Token));
        }

        [Fact]
        public async Task ReadBytesAsync_RequestingMoreData_ShouldAwaitOnRead()
        {
            //Arrange
            await _pipeWriter.WriteAsync(new byte[2]);

            await _pipeReaderContextSUT.ReadBytesAsync(1, CancellationToken.None);
            using var cts = new CancellationTokenSource(100);

            //Act & assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                async () => await _pipeReaderContextSUT.ReadBytesAsync(2, cts.Token));
        }

        [Fact]
        public async Task SeekAsync_PassEof_Completed_ShouldThrow()
        {
            //Arrange
            await _pipeWriter.WriteAsync(new byte[1]);
            await _pipeWriter.CompleteAsync();
            using var cts = new CancellationTokenSource(100);
            
            //Act & assert
            await Assert.ThrowsAsync<EndOfStreamException>(
                async () => await _pipeReaderContextSUT.SeekAsync(2, cts.Token));
        }

        [Fact]
        public async Task SeekAsync_PassEof_NotCompleted_ShouldThrow()
        {
            //Arrange
            await _pipeWriter.WriteAsync(new byte[1]);
            using var cts = new CancellationTokenSource(100);

            //Act & assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                async () => await _pipeReaderContextSUT.SeekAsync(2, cts.Token));
        }
    }
}