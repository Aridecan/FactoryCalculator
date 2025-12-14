using System;
using FactoryCalculator.Core;
using Xunit;

namespace FactoryCalculator.Core.Tests
{
    public class RateConverterTests
    {
        [Theory]
        [InlineData(60, "PerMinute", 1)]    // 60 per minute == 1 per second
        [InlineData(3600, "PerHour", 1)]    // 3600 per hour == 1 per second
        [InlineData(1, "PerSecond", 1)]     // 1 per second == 1 per second
        public void ToPerSecond_ConvertsCorrectly(double input, string fromName, double expectedPerSecond)
        {
            var from = Enum.Parse<UnitRate>(fromName);
            var actual = RateConverter.ToPerSecond((decimal)input, from);
            Assert.Equal((decimal)expectedPerSecond, actual);
        }

        [Theory]
        [InlineData(1, "PerMinute", 60)]    // 1/s -> 60/min
        [InlineData(1, "PerHour", 3600)]    // 1/s -> 3600/hr
        [InlineData(2.5, "PerMinute", 150)] // 2.5/s -> 150/min
        public void FromPerSecond_ConvertsCorrectly(double perSecond, string toName, double expected)
        {
            var to = Enum.Parse<UnitRate>(toName);
            var actual = RateConverter.FromPerSecond((decimal)perSecond, to);
            Assert.Equal((decimal)expected, actual);
        }

        [Theory]
        [InlineData(120, "PerMinute", "PerSecond", 2)]   // 120/min == 2/s
        [InlineData(2, "PerSecond", "PerMinute", 120)]   // 2/s == 120/min
        [InlineData(1, "PerHour", "PerMinute", 0.016666666666666666)]  // 1/hr == 1/60 per minute
        public void Convert_BetweenUnits_ReturnsExpected(double input, string fromName, string toName, double expected)
        {
            var from = Enum.Parse<UnitRate>(fromName);
            var to = Enum.Parse<UnitRate>(toName);
            var actual = RateConverter.Convert((decimal)input, from, to);
            var expectedDecimal = (decimal)expected;

            // Allow small floating/decimal conversion rounding differences.
            const decimal tolerance = 1e-10m;
            Assert.InRange(actual, expectedDecimal - tolerance, expectedDecimal + tolerance);
        }

        [Fact]
        public void Convert_SameUnit_ReturnsInput()
        {
            const decimal value = 42m;
            var actual = RateConverter.Convert(value, UnitRate.PerMinute, UnitRate.PerMinute);
            Assert.Equal(value, actual);
        }

        [Fact]
        public void ToPerSecond_InvalidRate_Throws()
        {
            var invalid = (UnitRate)999;
            Assert.Throws<ArgumentOutOfRangeException>(() => RateConverter.ToPerSecond(1m, invalid));
        }
    }
}

