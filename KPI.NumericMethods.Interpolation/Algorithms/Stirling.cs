﻿using KPI.NumericMethods.Interpolation.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KPI.NumericMethods.Interpolation.Algorithms
{
    class Stirling
    {
        private readonly Node[] _values;
        private readonly Dictionary<string, double> _cache;

        public double Result { get; private set; }

        private Stirling(IEnumerable<Node> values)
        {
            _values = values.ToArray();
            _cache = new Dictionary<string, double>();
        }

        public static double Q(Node[] nodes, double point)
        {
            (var baseValue, _) = CalculateBase(nodes, point);
            var step = nodes[1].X - nodes[0].X;
            return (point - baseValue) / step;
        }

        private void Interpolate(double point)
        {
            // Calculating the base of the interpolation
            (var baseValue, var baseIndex) = CalculateBase(_values, point);

            // Elements from baseIndex to top and bottom that are used fot interpolating
            var elements = baseIndex > (_values.Length - baseIndex - 1) ? (_values.Length - baseIndex - 1) : baseIndex;

            int stoppedAt = PopulateCache(baseIndex, elements);

            var powers = Enumerable.Range(1, stoppedAt);
            var step = _values[1].X - _values[0].X;
            var q = (point - baseValue) / step;
            var qSquare = q * q;

            Result = powers
                .Aggregate(_values[baseIndex].Y, (acc, power) => acc
                    + ((power % 2 == 1 ? q * (CachedDelta(-power / 2 - 1, power) + CachedDelta(-power / 2, power)) / 2 : qSquare * CachedDelta(-power / 2, power))
                    * Multiply(qSquare, (power - 1) / 2))
                    / Factorial(power));
        }

        private static (double baseValue, int baseIndex) CalculateBase(Node[] nodes, double point)
        {
            var baseValue = double.MaxValue;
            var baseIndex = -1;
            for (var i = 0; i < nodes.Length; i++)
            {
                var value = nodes[i].X;
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

        // (q^2 - 1) * (q^2 - 2) ...
        private double Multiply(double qSquare, int times)
            => Enumerable.Range(1, times).Aggregate<int, double>(1, (acc, cur) => acc * (qSquare - cur * cur));

        /// <summary>
        /// Populates cache and returns the length of latest populated entry
        /// </summary>
        /// <returns>The length of latest populated entry</returns>
        private int PopulateCache(int baseIndex, int elements)
        {
            var d = 3;
            var epsilon = 5 * Math.Pow(10, -d);

            for (int length = 0; length < 2 * elements + 1; length++)
            {
                bool isLargerThanEpsilon = false;

                for (int from = -elements; from + length <= elements; from++)
                {
                    var delta = CalculateDelta(baseIndex, from, length);
                    _cache[GetCacheKey(from, length)] = delta;
                    if (Math.Abs(delta) > epsilon)
                        isLargerThanEpsilon = true;
                }

                if (!isLargerThanEpsilon)
                    return length;
            }

            return 2 * elements;
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

        public static Stirling InterpolateFrom(IEnumerable<Node> nodes, double value)
        {
            var stirling = new Stirling(nodes);
            stirling.Interpolate(value);
            return stirling;
        }
    }
}
