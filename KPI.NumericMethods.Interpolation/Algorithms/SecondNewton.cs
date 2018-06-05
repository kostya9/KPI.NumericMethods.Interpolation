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
            int stoppedAt = PopulateCache();

            var lengths = Enumerable.Range(1, stoppedAt - 1);

            Result = lengths
                .Aggregate<int, double>(0, (acc, length) => acc
                    + GetCachedDelta(0, length)
                        * _values.Take(length - 1).Select(v => v.X).Aggregate<double, double>(1, (xAcc, x) => xAcc * (point - x)));
        }

        /// <summary>
        /// Populates cache and returns the length of latest populated entry
        /// </summary>
        /// <returns>The length of latest populated entry</returns>
        private int PopulateCache()
        {
            var d = 2;
            var epsilon = 5 * Math.Pow(10, -d);

            for (int length = 1; length <= _values.Length; length++)
            {
                bool isLargerThanEpsilon = false;

                for (int from = 0; from + length <= _values.Length; from++)
                {
                    var delta = CalculateDelta(from, length);
                    _cache[GetCacheKey(from, length)] = delta;
                    if (delta > epsilon)
                        isLargerThanEpsilon = true;
                }

                if (!isLargerThanEpsilon)
                    return length;
            }

            return _values.Length;
        }

        private double CalculateDelta(int from, int length)
        {
            if (length == 1)
                return _values[from].Y;

            return (GetCachedDelta(from + 1, length - 1) - GetCachedDelta(from, length - 1))
                    / (_values[from + length - 1].X - _values[from].X);
        }

        private string GetCacheKey(int from, int length)
            => $"{from}: {length}";

        private double GetCachedDelta(int from, int length)
            => _cache[GetCacheKey(from, length)];

        public static SecondNewton InterpolateFrom(IEnumerable<Node> nodes, double value)
        {
            var newton = new SecondNewton(nodes);
            newton.Interpolate(value);
            return newton;
        }
    }
}
