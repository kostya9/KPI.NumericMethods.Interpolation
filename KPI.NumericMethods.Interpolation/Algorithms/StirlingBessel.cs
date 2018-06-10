using KPI.NumericMethods.Interpolation.Models;
using System.Collections.Generic;
using System.Linq;

namespace KPI.NumericMethods.Interpolation.Algorithms
{
    class StirlingBessel
    {
        public double Result { get; }

        public StirlingBessel(double result)
        {
            Result = result;
        }

        public static StirlingBessel InterpolateFrom(IEnumerable<Node> nodes, double value)
        {
            var q = Stirling.Q(nodes.ToArray(), value);

            double result;
            if(q <= 0.25)
            {
                result = Bessel.InterpolateFrom(nodes, value).Result;
            }
            else
            {
                result = Stirling.InterpolateFrom(nodes, value).Result;
            }

            return new StirlingBessel(result);
        }
    }
}
