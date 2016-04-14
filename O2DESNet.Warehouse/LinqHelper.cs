﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O2DESNet.Warehouse
{
    public static class LinqHelper
    {
        public static List<T> ExtractAll<T>(this List<T> source, Predicate<T> match)
        {
            List<T> extract = source.FindAll(match);
            source.RemoveAll(match);
            return extract;
        }

        public static List<T> ExtractRange<T>(this List<T> source, int index, int count)
        {
            List<T> extract = source.GetRange(index, count);
            source.RemoveRange(index, count);
            return extract;
        }

        public static TimeSpan Multiply(this TimeSpan timeSpan, int multiplier)
        {
            return TimeSpan.FromTicks(timeSpan.Ticks * multiplier);
        }

        public static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            List<TValue> values = Enumerable.ToList(dict.Values);
            int size = dict.Count;
            while (true)
            {
                yield return values[Simulator.RS.Next(size)];
            }
        }

        public static IEnumerable<TKey> RandomKeys<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            List<TKey> values = Enumerable.ToList(dict.Keys);
            int size = dict.Count;
            while (true)
            {
                yield return values[Simulator.RS.Next(size)];
            }
        }
    }
}