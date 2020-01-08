using System;
using System.Collections.Generic;
using System.Text;
using Kaitai.Async;
using Xunit;

namespace Kaitai.Struct.Runtime.Async.Tests
{
    
    public class KaitaiAsyncStructTests
    {
        [Fact]
        public void M_Io_IsSet()
        {
            var kaitaiAsyncStream = new KaitaiAsyncStream(new byte[0]);
            var kaitaiAsyncStructSUT = new FooKaitaiAsyncStruct(kaitaiAsyncStream);

            Assert.Equal(kaitaiAsyncStream, kaitaiAsyncStructSUT.M_Io);
        }

        private class FooKaitaiAsyncStruct : KaitaiAsyncStruct
        {
            public FooKaitaiAsyncStruct(KaitaiAsyncStream kaitaiStream) : base(kaitaiStream)
            {
            }
        }
    }
}
