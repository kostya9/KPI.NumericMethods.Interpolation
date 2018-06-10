using KPI.NumericMethods.Interpolation.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KPI.NumericMethods.Interpolation.Algorithms
{
    class Bessel
    {
        private readonly Node[] _values;
        private readonly Dictionary<string, double> _cache;

        public double Result { get; private set; }

        private Bessel(IEnumerable<Node> values)
        {
            _values = values.ToArray();
            _cache = new Dictionary<string, double>();
        }

        private void Interpolate(double point)
        {
            var baseIndexL = -1;
            var baseIndexR = -1;
            var baseValue = double.MaxValue;
            for (var i = 1; i < _values.Length; i++)
            {
                var valueL = _values[i - 1].X;
                var valueR = _values[i].X;
                if ((valueL - point) * (valueR - point) <= 0)
                {
                    baseIndexL = i - 1;
                    baseIndexR = i;
                    baseValue = _values[baseIndexL].X;
                    break;
                }
            }

            // Elements from baseIndex to top and bottom that are used fot interpolating
            var elements = baseIndexL > (_values.Length - baseIndexR - 1) ? (_values.Length - baseIndexR - 1) : baseIndexL;

            int stoppedAt = PopulateCache(baseIndexL, elements);

            var powers = Enumerable.Range(1, stoppedAt);

            var step = _values[1].X - _values[0].X;
            var q = (point - baseValue) / step;
            var qSquare = q * q;

            var first = (_values[baseIndexL].Y + _values[baseIndexR].Y) / 2;

            Result = powers
                .Aggregate(first, (acc, power) => acc + NextSumMember(power, q, qSquare));
        }

        private double NextSumMember(int power, double q, double qSquare)
        {
            if (power == 1)
                return (q - 0.5) * CachedDelta(0, 1);

            return ((power % 2 == 1 
                    ? (q - 0.5) * CachedDelta(-power / 2, power) 
                    : (CachedDelta(-power / 2, power) + CachedDelta(-power / 2 + 1, power)) / 2)
                        * q * Multiply(qSquare, (power - 2) / 2) * (q - power / 2))
                        / Factorial(power);
        }

        private double Factorial(int i)
        {
            double result = 1;
            for (; i != 1; i--)
                result = result * i;
            return result;
        }

        // (q^2 - 1) * (q^2 - 2) ...
        private double Multiply(double qSquare, int times)
            => times <= 0 ? 1 : Enumerable.Range(1, times).Aggregate<int, double>(1, (acc, cur) => acc * (qSquare - cur * cur));

        /// <summary>
        /// Populates cache and returns the length of latest populated entry
        /// </summary>
        /// <returns>The length of latest populated entry</returns>
        private int PopulateCache(int baseIndex, int elements)
        {
            var d = 2;
            var epsilon = 5 * Math.Pow(10, -d);

            for (int length = 0; length < 2 * elements + 2; length++)
            {
                bool isLargerThanEpsilon = false;

                for (int from = -elements; from + length <= elements + 1; from++)
                {
                    var delta = CalculateDelta(baseIndex, from, length);
                    _cache[GetCacheKey(from, length)] = delta;
                    if (delta > epsilon)
                        isLargerThanEpsilon = true;
                }

                if (!isLargerThanEpsilon)
                    return length;
            }

            return 2 * elements + 1;
        }

        private double CalculateDelta(int baseIndex, int from, int power)
        {
            if (power == 0)
                return _values[baseIndex + from].Y;

            return (CachedDelta(from + 1, power - 1) - CachedDelta(from, power - 1));
        }

        private string GetCacheKey(int from, int power)
            => $"{from}: {power}";

        private double CachedDelta(int from, int power)
            => _cache[GetCacheKey(from, power)];

        public static Bessel InterpolateFrom(IEnumerable<Node> nodes, double value)
        {
            var bessel = new Bessel(nodes);
            bessel.Interpolate(value);
            return bessel;
        }
    }
}
