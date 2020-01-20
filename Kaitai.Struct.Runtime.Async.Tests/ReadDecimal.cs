using System.Linq;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
    public class ReadDecimal
    {
        [Theory]
        [MemberData(nameof(DecimalData.Decimal4Data), MemberType = typeof(DecimalData))]
        public async Task ReadF4beAsync_Test(float expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent.Reverse().ToArray());

            Assert.Equal(expected, await kaitaiStreamSUT.ReadF4beAsync());
        }

        [Theory]
        [MemberData(nameof(DecimalData.Decimal4Data), MemberType = typeof(DecimalData))]
        public async Task ReadF4leAsync_Test(float expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(expected, await kaitaiStreamSUT.ReadF4leAsync());
        }

        [Theory]
        [MemberData(nameof(DecimalData.Decimal8Data), MemberType = typeof(DecimalData))]
        public async Task ReadF8beAsync_Test(double expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent.Reverse().ToArray());

            Assert.Equal(expected, await kaitaiStreamSUT.ReadF8beAsync());
        }

        [Theory]
        [MemberData(nameof(DecimalData.Decimal8Data), MemberType = typeof(DecimalData))]
        public async Task ReadF8leAsync_Test(double expected, byte[] streamContent)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(expected, await kaitaiStreamSUT.ReadF8leAsync());
        }
    }
}