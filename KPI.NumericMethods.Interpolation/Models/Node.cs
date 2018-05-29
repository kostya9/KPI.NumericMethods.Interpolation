using System;
using System.Linq;

namespace KPI.NumericMethods.Interpolation.Models
{
    public class Node : IComparable
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public static Node From((double, double) dataUnit)
        {
            var (point, value) = dataUnit;
            return new Node
            {
                X = point,
                Y = value
            };
        }

        public static Node[] From(params (double, double)[] values)
        {
            return values.Select(From).ToArray();
        }

        public int CompareTo(object obj)
        {
            if(obj is Node n)
            {
                return X.CompareTo(n.X);
            }

            throw new ArgumentException();
        }
    }
}
