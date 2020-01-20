using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
    public class KaitaiAsyncStreamBaseTests
    {
        [Fact]
        public async Task AlignToByte_Test()
        {
            //Arrange
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[]{0b_1000_0000});

            var read = await kaitaiStreamSUT.ReadBitsIntAsync(1);
            Assert.Equal(1u,  read);

            //Act
            kaitaiStreamSUT.AlignToByte();
            //Assert
            Assert.Equal(1, kaitaiStreamSUT.Pos);
        }

        [Theory]
        [InlineData(true, 0, 0)]
        [InlineData(false, 1, 0)]
        [InlineData(false, 1, 1)]
        [InlineData(false, 1, 2)]
        [InlineData(false, 1, 3)]
        [InlineData(false, 1, 4)]
        [InlineData(false, 1, 5)]
        [InlineData(false, 1, 6)]
        [InlineData(false, 1, 7)]
        [InlineData(true, 1, 8)]
        public async Task Eof_Test(bool shouldBeEof, int streamSize, int readBitsAmount)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[streamSize]);

            await kaitaiStreamSUT.ReadBitsIntAsync(readBitsAmount);

            if (shouldBeEof)
                Assert.True(kaitaiStreamSUT.IsEof);
            else
                Assert.False(kaitaiStreamSUT.IsEof);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        public async Task Pos_ByRead_Test(int expectedPos, int readBitsAmount)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[1]);

            await kaitaiStreamSUT.ReadBytesAsync(readBitsAmount);

            Assert.Equal(expectedPos, kaitaiStreamSUT.Pos);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        public async Task Pos_BySeek_Test(int expectedPos, int position)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[1]);

            await kaitaiStreamSUT.SeekAsync(position);

            Assert.Equal(expectedPos, kaitaiStreamSUT.Pos);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Size_Test(int streamSize)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[streamSize]);

            Assert.Equal(streamSize, kaitaiStreamSUT.Size);
        }

        [Fact]
        public void EmptyStream_NoRead_NoSeek_IsEof_ShouldBe_True()
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[0]);

            Assert.True(kaitaiStreamSUT.IsEof);
        }

        [Fact]
        public void EmptyStream_NoRead_NoSeek_Pos_ShouldBe_0()
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(new byte[0]);

            Assert.Equal(0, kaitaiStreamSUT.Pos);
        }
    }
}