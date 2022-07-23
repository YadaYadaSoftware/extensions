using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Collections.Generic;

public static class EnumerableExtensions
{
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static IEnumerable<IEnumerable<T>> GroupWhile<T>(this IEnumerable<T> seq, Func<T, T, bool> condition)
    {
        if (seq == null || !seq.Any()) yield return new List<T>();
        if (seq.Count() == 1) yield return new List<T>{ seq.Single() };

        var prev = seq.First();
        List<T> list = new() { prev };

        foreach (var item in seq.Skip(1))
        {
            if (condition(prev, item) == false)
            {
                yield return list;
                list = new List<T>();
            }
            list.Add(item);
            prev = item;
        }

        yield return list;
    }

}