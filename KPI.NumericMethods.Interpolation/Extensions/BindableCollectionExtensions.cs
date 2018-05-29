using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace KPI.NumericMethods.Interpolation.Extensions
{
    public static class BindableCollectionExtensions
    {
        public static BindableCollection<T> ToSortedBindableCollection<T>(this IEnumerable<T> enumerable)
            where T: IComparable
        {
            var collection = new BindableCollection<T>();

            foreach(var v in enumerable)
            {
                collection.InsertSorted(v);
            }

            return collection;
        }
    }
}
