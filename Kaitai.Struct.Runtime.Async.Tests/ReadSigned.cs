using System.Linq;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
    public class ReadSigned
    {
        [Theory]
        [MemberData(nameof(IntegralData.Integral1Data), MemberType = typeof(IntegralData))]
        public async Task ReadS1Async_Test(sbyte expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(expected, await kaitaiStreamSUT.ReadS1Async());
        }

        [Theory]
        [MemberData(nameof(IntegralData.Integral2Data), MemberType = typeof(IntegralData))]
        public async Task ReadS2beAsync_Test(short expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(expected, await kaitaiStreamSUT.ReadS2beAsync());
        }

        [Theory]
        [MemberData(nameof(IntegralData.Integral4Data), MemberType = typeof(IntegralData))]
        public async Task ReadS4beAsync_Test(int expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(expected, await kaitaiStreamSUT.ReadS4beAsync());
        }

        [Theory]
        [MemberData(nameof(IntegralData.Integral8Data), MemberType = typeof(IntegralData))]
        public async Task ReadS8beAsync_Test(long expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(expected, await kaitaiStreamSUT.ReadS8beAsync());
        }

        [Theory]
        [MemberData(nameof(IntegralData.Integral2Data), MemberType = typeof(IntegralData))]
        public async Task ReadS2leAsync_Test(short expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent.Reverse().ToArray());

            Assert.Equal(expected, await kaitaiStreamSUT.ReadS2leAsync());
        }

        [Theory]
        [MemberData(nameof(IntegralData.Integral4Data), MemberType = typeof(IntegralData))]
        public async Task ReadS4leAsync_Test(int expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent.Reverse().ToArray());

            Assert.Equal(expected, await kaitaiStreamSUT.ReadS4leAsync());
        }

        [Theory]
        [MemberData(nameof(IntegralData.Integral8Data), MemberType = typeof(IntegralData))]
        public async Task ReadS8leAsync_Test(long expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent.Reverse().ToArray());

            Assert.Equal(expected, await kaitaiStreamSUT.ReadS8leAsync());
        }
    }
}