using System;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissorsLizardSpock
{
    static class Extensions
    {
        /// <summary>
        ///     Returns a collection of integers containing the indexes at which the elements of the source collection match
        ///     the given predicate.</summary>
        /// <typeparam name="T">
        ///     The type of elements in the collection.</typeparam>
        /// <param name="source">
        ///     The source collection whose elements are tested using <paramref name="predicate"/>.</param>
        /// <param name="predicate">
        ///     The predicate against which the elements of <paramref name="source"/> are tested.</param>
        /// <returns>
        ///     A collection containing the zero-based indexes of all the matching elements, in increasing order.</returns>
        public static IEnumerable<int> SelectIndexWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            return selectIndexWhereIterator(source, predicate);
        }

        private static IEnumerable<int> selectIndexWhereIterator<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            int i = 0;
            using (var e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (predicate(e.Current))
                        yield return i;
                    i++;
                }
            }
        }

        /// <summary>
        ///     Executes the specified function with the specified argument.</summary>
        /// <typeparam name="TSource">
        ///     Type of the argument to the function.</typeparam>
        /// <typeparam name="TResult">
        ///     Type of the result of the function.</typeparam>
        /// <param name="source">
        ///     The argument to the function.</param>
        /// <param name="func">
        ///     The function to execute.</param>
        /// <returns>
        ///     The result of the function.</returns>
        public static TResult Apply<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");
            return func(source);
        }

        public static void LocalTranslate(this Transform transform, Vector3 by)
        {
            transform.localPosition = transform.localPosition - by;
        }
    }
}
