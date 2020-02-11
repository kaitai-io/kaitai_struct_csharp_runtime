using System;
using System.Collections.Generic;
using System.Linq;

namespace Kaitai.Struct.Runtime.Async.Tests
{
    public class DecimalData
    {
        public static IEnumerable<object[]> Decimal4Data =>
            new List<(float expected, byte[] streamContent)>
            {
                (0f, BitConverter.GetBytes(0f)),
                (1f, BitConverter.GetBytes(1f)),
                (0.1f, BitConverter.GetBytes(0.1f)),
                (1.1f, BitConverter.GetBytes(1.1f)),
                (float.MinValue, BitConverter.GetBytes(float.MinValue)),
                (float.MaxValue, BitConverter.GetBytes(float.MaxValue))
            }.Select(t => new object[] {t.expected, t.streamContent});

        public static IEnumerable<object[]> Decimal8Data =>
            new List<(double expected, byte[] streamContent)>
            {
                (0d, BitConverter.GetBytes(0d)),
                (1d, BitConverter.GetBytes(1d)),
                (0.1d, BitConverter.GetBytes(0.1d)),
                (1.1d, BitConverter.GetBytes(1.1d)),
                (double.MinValue, BitConverter.GetBytes(double.MinValue)),
                (double.MaxValue, BitConverter.GetBytes(double.MaxValue))
            }.Select(t => new object[] {t.expected, t.streamContent});
    }
}