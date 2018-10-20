using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipApp.ExtensionClasses
{
    static class LinqExtensions
    {
        static IEnumerable<T> Zip<T1, T2, T>(this IEnumerable<T1> first,IEnumerable<T2> second, Func<T1, T2, T> operation)
        {
            using (var firstIteration = first.GetEnumerator())
            using (var secondIteration = second.GetEnumerator())
            {
                while (firstIteration.MoveNext())
                {
                    if (secondIteration.MoveNext())
                    {
                        yield return operation(firstIteration.Current, secondIteration.Current);
                    }
                    else
                    {
                        yield return operation(firstIteration.Current,default(T2));
                    }
                }
                while (secondIteration.MoveNext())
                {
                    yield return operation(default(T1), secondIteration.Current);
                }
            }
        }

        public static IEnumerable<T> Interleave<T>(
    this IEnumerable<T> first, IEnumerable<T> second)
        {
            using (var enumerator1 = first.GetEnumerator())
            using (var enumerator2 = second.GetEnumerator())
            {
                bool firstHasMore;
                bool secondHasMore;

                while ((firstHasMore = enumerator1.MoveNext())
                     | (secondHasMore = enumerator2.MoveNext()))
                {
                    if (firstHasMore)
                        yield return enumerator1.Current;

                    if (secondHasMore)
                        yield return enumerator2.Current;
                }
            }
        }
    }
}
