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
            int stopped = PopulateCache();

            var prepared = _values
                    .Take(stopped)
                    .Select((v, i) => (v, i + 1));

            Result = prepared
                .Aggregate<(Node v, int i), double>(0, (acc, el) => acc
                    + GetCachedDelta(0, el.i)
                        * _values.Take(el.i - 1).Select(v => v.X).Aggregate<double, double>(1, (xAcc, x) => xAcc * (point - x)));
        }

        private int PopulateCache()
        {
            var d = 2;
            var epsilon = 5 * Math.Pow(10, -d);

            for (int i = 1; i <= _values.Length; i++)
            {
                bool isLargerThanEpsilon = false;
                for (int j = 0; j + i <= _values.Length; j++)
                {
                    var delta = CalculateDelta(j, i);
                    _cache[GetCacheKey(j, i)] = delta;
                    if (delta > epsilon)
                        isLargerThanEpsilon = true;
                }

                if (!isLargerThanEpsilon)
                {
                    return i;
                }
            }

            return _values.Length;
        }

        private double CalculateDelta(int i, int length)
        {
            if (length == 1)
                return _values[i].Y;

            return (GetCachedDelta(i + 1, length - 1) - GetCachedDelta(i, length - 1))
                    / (_values[i + length - 1].X - _values[i].X);
        }

        private string GetCacheKey(int i, int length)
            => $"{i}: {length}";

        private double GetCachedDelta(int i, int length)
            => _cache[GetCacheKey(i, length)];

        public static SecondNewton InterpolateFrom(IEnumerable<Node> nodes, double value)
        {
            var newton = new SecondNewton(nodes);
            newton.Interpolate(value);
            return newton;
        }
    }
}
