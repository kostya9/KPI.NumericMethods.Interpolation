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

        public SecondNewton(IEnumerable<Node> values)
        {
            _values = values.ToArray();
            _cache = new Dictionary<string, double>();
        }

        private double GetDelta(int i, Node[] nodes)
        {
            var length = nodes.Length;

            if (length == 1)
                return _values[i].Y;

            var key = $"[{i}]: {string.Join(",", nodes.Select(n => n.X))}";

            if (_cache.TryGetValue(key, out var cached))
                return cached;

            var calculated = 
                (GetDelta(i + 1, nodes.Skip(1).ToArray()) - GetDelta(i, nodes.Take(length - 1).ToArray())) 
                    / (_values[i + length - 1].X - _values[i].X);
            _cache[key] = calculated;
            return calculated;
        }

        private void Interpolate(double xValue)
        {
            var prepared = _values
                    .Select((v, i) => (v, i + 1));

            Result = prepared
                .Reverse()
                .Aggregate<(Node v, int i), double>(0, (acc, el) => acc 
                    + GetDelta(0, _values.Take(el.i).ToArray()) 
                        * _values.Take(el.i - 1).Select(v => v.X).Aggregate<double, double>(1, (xAcc, x) => xAcc * (xValue - x)));
        }

        public static SecondNewton InterpolateFrom(IEnumerable<Node> nodes, double value)
        {
            var newton = new SecondNewton(nodes);
            newton.Interpolate(value);
            return newton;
        }
    }
}
