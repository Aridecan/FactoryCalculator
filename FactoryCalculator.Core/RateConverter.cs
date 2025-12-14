using System;

namespace FactoryCalculator.Core
{
    /// <summary>
    /// Utility for converting rates between configured <see cref="UnitRate"/> values.
    /// Internally conversions use <see cref="decimal"/> for deterministic arithmetic.
    /// </summary>
    public static class RateConverter
    {
        /// <summary>
        /// Converts a quantity expressed in <paramref name="from"/> to the equivalent quantity per second.
        /// </summary>
        /// <param name="quantity">Quantity expressed in <paramref name="from"/> (e.g. items per minute).</param>
        /// <param name="from">Source unit rate.</param>
        /// <returns>Quantity normalized to per-second units.</returns>
        public static decimal ToPerSecond(decimal quantity, UnitRate from)
        {
            var seconds = SecondsPerUnit(from);
            if (seconds <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(from), "UnitRate must map to a positive seconds-per-unit value.");
            }

            return quantity / seconds;
        }

        /// <summary>
        /// Converts a quantity expressed per second to the specified <paramref name="to"/> unit.
        /// </summary>
        /// <param name="perSecond">Quantity expressed per second.</param>
        /// <param name="to">Target unit rate.</param>
        /// <returns>Quantity expressed in the target unit (e.g. per minute).</returns>
        public static decimal FromPerSecond(decimal perSecond, UnitRate to)
        {
            var seconds = SecondsPerUnit(to);
            if (seconds <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(to), "UnitRate must map to a positive seconds-per-unit value.");
            }

            return perSecond * seconds;
        }

        /// <summary>
        /// Converts a quantity from one unit rate to another.
        /// </summary>
        /// <param name="quantity">Quantity expressed in <paramref name="from"/>.</param>
        /// <param name="from">Source unit rate.</param>
        /// <param name="to">Target unit rate.</param>
        /// <returns>Quantity expressed in <paramref name="to"/>.</returns>
        public static decimal Convert(decimal quantity, UnitRate from, UnitRate to)
        {
            if (from == to)
            {
                return quantity;
            }

            var perSecond = ToPerSecond(quantity, from);
            return FromPerSecond(perSecond, to);
        }

        private static decimal SecondsPerUnit(UnitRate rate)
        {
            return rate switch
            {
                UnitRate.PerSecond => 1m,
                UnitRate.PerMinute => 60m,
                UnitRate.PerHour => 3600m,
                _ => throw new ArgumentOutOfRangeException(nameof(rate), "Unsupported UnitRate value.")
            };
        }
    }
}
