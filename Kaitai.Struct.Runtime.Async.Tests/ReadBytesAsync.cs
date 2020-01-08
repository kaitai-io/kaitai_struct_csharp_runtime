using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
    public class ReadBytesAsync
    {
        public static IEnumerable<object[]> BytesData =>
           new List<(byte[] streamContent, int bytesCount)>
           {
                (new byte[] {0b_1101_0101}, 0),
                (new byte[] {0b_1101_0101}, 1),
                (new byte[] {0b_1101_0101, 0b_1101_0101}, 1),
                (new byte[] {0b_1101_0101, 0b_1101_0101}, 2),
           }.Select(t => new object[] { t.streamContent, t.bytesCount });


        [Theory]
        [MemberData(nameof(BytesData))]
        public async Task ReadBytesAsync_long_Test(byte[] streamContent, long bytesCount)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(streamContent.Take((int)bytesCount), await kaitaiStreamSUT.ReadBytesAsync(bytesCount));
        }

        [Theory]
        [MemberData(nameof(BytesData))]
        public async Task ReadBytesAsync_ulong_Test(byte[] streamContent, ulong bytesCount)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(streamContent.Take((int)bytesCount), await kaitaiStreamSUT.ReadBytesAsync(bytesCount));
        }

        [Fact]
        public async Task ReadBytesAsyncLong_NegativeInvoke_ThrowsArgumentOutOfRangeException()
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[0]);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await kaitaiStreamSUT.ReadBytesAsync((long) -1));
        }

        [Fact]
        public async Task ReadBytesAsyncLong_LargerThanInt32Invoke_ThrowsArgumentOutOfRangeException()
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[0]);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await kaitaiStreamSUT.ReadBytesAsync((long)Int32.MaxValue+1));
        }

        [Fact]
        public async Task ReadBytesAsyncLong_LargerThanBufferInvoke_ThrowsArgumentOutOfRangeException()
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[0]);

            await Assert.ThrowsAsync<EndOfStreamException>(async () =>
                await kaitaiStreamSUT.ReadBytesAsync((long)1));
        }
        
        [Fact]
        public async Task ReadBytesAsyncULong_LargerThanInt32Invoke_ThrowsArgumentOutOfRangeException()
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[0]);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await kaitaiStreamSUT.ReadBytesAsync((ulong)Int32.MaxValue + 1));
        }

        [Fact]
        public async Task ReadBytesAsyncULong_LargerThanBufferInvoke_ThrowsArgumentOutOfRangeException()
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[0]);

            await Assert.ThrowsAsync<EndOfStreamException>(async () =>
                await kaitaiStreamSUT.ReadBytesAsync((ulong)1));
        }

        public static IEnumerable<object[]> StringData =>
            new List<string>
            {
                "",
                "ABC",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            }.Select(t => new []{ Encoding.ASCII.GetBytes(t)});


        [Theory]
        [MemberData(nameof(StringData))]
        public async Task ReadBytesFullAsync_Test(byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(streamContent, await kaitaiStreamSUT.ReadBytesFullAsync());
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public async Task EnsureFixedContentsAsync_Test(byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(streamContent, await kaitaiStreamSUT.EnsureFixedContentsAsync(streamContent));
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public async Task EnsureFixedContentsAsync_ThrowsIfByteIsChanged(byte[] streamContent)
        {
            if(streamContent.Length == 0) return;
            
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            var expected = streamContent.ToArray();
            expected[0] = (byte)~expected[0];

            await Assert.ThrowsAsync<Exception>(async () => await kaitaiStreamSUT.EnsureFixedContentsAsync(expected));
        }

        public static IEnumerable<object[]> StringWithTerminatorsData =>
            new List<(string streamContent, string expected, char terminator, bool isPresent, bool shouldInclude)>
            {
                ("",   "",  '\0', false, false),
                ("",   "",  '\0', false, true),

                ("ABC", "ABC", '\0', false, false),
                ("ABC", "ABC", '\0', false, true),

                ("ABC", "", 'A', true, false),
                ("ABC", "A", 'A', true, true),

                ("ABC", "A", 'B', true, false),
                ("ABC", "AB", 'B', true, true),

                ("ABC", "AB", 'C', true, false),
                ("ABC", "ABC", 'C', true, true),
            }.Select(t => new[] { Encoding.ASCII.GetBytes(t.streamContent), Encoding.ASCII.GetBytes(t.expected), (object)(byte)t.terminator, t.isPresent, t.shouldInclude});

        [Theory]
        [MemberData(nameof(StringWithTerminatorsData))]
        public async Task ReadBytesTermAsync(byte[] streamContent, byte[] expected, byte terminator, bool _, bool shouldInclude)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(expected, await kaitaiStreamSUT.ReadBytesTermAsync(terminator, shouldInclude, false, false));
        }

        [Theory]
        [MemberData(nameof(StringWithTerminatorsData))]
        public async Task ReadBytesTermAsync_ThrowsIsTerminatorNotPresent(byte[] streamContent, byte[] expected, byte terminator, bool terminatorIsPresent, bool shouldInclude)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            if(terminatorIsPresent) return;

            await Assert.ThrowsAsync<EndOfStreamException>(async()=> await kaitaiStreamSUT.ReadBytesTermAsync(terminator, shouldInclude, false, true));
        }

        [Theory]
        [MemberData(nameof(StringWithTerminatorsData))]
        public async Task ReadBytesTermAsync_ShouldNotConsumeTerminator(byte[] streamContent, byte[] expected, byte terminator, bool terminatorIsPresent, bool shouldInclude)
        {
            //Arrange
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);
            
            //Act
            await kaitaiStreamSUT.ReadBytesTermAsync(terminator, shouldInclude, false, false);

            //Assert
            var amountToConsume = expected.Length;
            if (expected.Length > 0 && shouldInclude && terminatorIsPresent)
            {
                amountToConsume--;
            }
            
            Assert.Equal(amountToConsume, kaitaiStreamSUT.Pos);
        }

        [Theory]
        [MemberData(nameof(StringWithTerminatorsData))]
        public async Task ReadBytesTermAsync_ShouldConsumeTerminator(byte[] streamContent, byte[] expected, byte terminator, bool terminatorIsPresent, bool shouldInclude)
        {
            //Arrange
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            //Act
            await kaitaiStreamSUT.ReadBytesTermAsync(terminator, shouldInclude, true, false);

            //Assert
            var amountToConsume = expected.Length;
            if (!shouldInclude && terminatorIsPresent)
            {
                amountToConsume++;
            }

            Assert.Equal(amountToConsume, kaitaiStreamSUT.Pos);
        }

    }
}