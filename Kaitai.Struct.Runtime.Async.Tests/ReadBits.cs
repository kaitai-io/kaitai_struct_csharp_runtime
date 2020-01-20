using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
    public class ReadBits
    {
        [Theory]
        [MemberData(nameof(BitsData.BitsBeData), MemberType = typeof(BitsData))]
        public async Task ReadBitsIntAsync_Test(ulong expected, byte[] streamContent, int bitsCount)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(expected, await kaitaiStreamSUT.ReadBitsIntAsync(bitsCount));
        }

        [Theory]
        [MemberData(nameof(BitsData.BitsLeData), MemberType = typeof(BitsData))]
        public async Task ReadBitsIntLeAsync_Test(ulong expected, byte[] streamContent, int bitsCount)
        {
            var kaitaiStreamSUT = new KaitaiAsyncStream(streamContent);

            Assert.Equal(expected, await kaitaiStreamSUT.ReadBitsIntLeAsync(bitsCount));
        }
    }
}