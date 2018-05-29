using System;
using System.Collections.ObjectModel;

namespace KPI.NumericMethods.Interpolation.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void InsertSorted<TSource>(this ObservableCollection<TSource> source, TSource value)
            where TSource : IComparable
        {
            int i;
            for (i = 0; i < source.Count; i++)
            {
                if (source[i].CompareTo(value) > 0)
                    break;
            }

            source.Insert(i, value);
        }
    }
}
