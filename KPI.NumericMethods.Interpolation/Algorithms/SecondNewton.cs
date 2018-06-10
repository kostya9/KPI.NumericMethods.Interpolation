using KPI.NumericMethods.Interpolation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.NumericMethods.Interpolation.Algorithms
{
    public class SecondNewton
    {
        private readonly Node[] _values;
        private readonly Dictionary<string, double> _cache;

        public double Result { get; private set; }

        private SecondNewton(IEnumerable<Node> values)
        {
            _values = values.OrderByDescending(v => v).ToArray();
            _cache = new Dictionary<string, double>();
        }

        private void Interpolate(double point)
        {
            (_, int baseIndex) = CalculateBase(point);
            int stoppedAt = PopulateCache(baseIndex);

            var lengths = stoppedAt < 2 ? Enumerable.Empty<int>() : Enumerable.Range(2, stoppedAt - 2);

            Result = lengths
                .Aggregate<int, double>(_values[baseIndex].Y, (acc, length) => acc
                    + CachedDelta(baseIndex, length)
                        * Enumerable.Range(0, length - 1).Aggregate<int, double>(1, (xAcc, cur) => xAcc * (point - _values[cur + baseIndex].X)) / Factorial(length));
        }

        private (double baseValue, int baseIndex) CalculateBase(double point)
        {
            var baseValue = double.MaxValue;
            var baseIndex = -1;
            for (var i = 0; i < _values.Length; i++)
            {
                var value = _values[i].X;
                if (Math.Abs(point - value) < Math.Abs(point - baseValue))
                {
                    baseValue = value;
                    baseIndex = i;
                }
            }

            return (baseValue, baseIndex);
        }

        private double Factorial(int i)
        {
            double result = 1;
            for (; i != 1; i--)
                result = result * i;
            return result;
        }

        /// <summary>
        /// Populates cache and returns the length of latest populated entry
        /// </summary>
        /// <returns>The length of latest populated entry</returns>
        private int PopulateCache(int baseIndex)
        {
            var d = 100;
            var epsilon = 5 * Math.Pow(10, -d);

            for (int length = 1; length + baseIndex < _values.Length; length++)
            {
                bool isLargerThanEpsilon = false;

                for (int from = baseIndex; from + length < _values.Length; from++)
                {
                    var delta = CalculateDelta(from, length);
                    _cache[GetCacheKey(from, length)] = delta;
                    if (Math.Abs(delta) > epsilon)
                        isLargerThanEpsilon = true;
                }

                if (!isLargerThanEpsilon)
                    return length;
            }

            return _values.Length- baseIndex;
        }

        private double CalculateDelta(int from, int length)
        {
            if (length == 1)
                return _values[from].Y;

            return (CachedDelta(from + 1, length - 1) - CachedDelta(from, length - 1))
                    / (_values[from + length - 1].X - _values[from].X);
        }

        private string GetCacheKey(int from, int length)
            => $"{from}: {length}";

        private double CachedDelta(int from, int length)
            => _cache[GetCacheKey(from, length)];

        public static SecondNewton InterpolateFrom(IEnumerable<Node> nodes, double value)
        {
            var newton = new SecondNewton(nodes);
            newton.Interpolate(value);
            return newton;
        }
    }
}
