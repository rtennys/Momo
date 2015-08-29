using System;
using System.Collections.Generic;
using System.Linq;

namespace Momo.Common
{
    public class LambdaEqualityComparer<T> : EqualityComparer<T>
    {
        public LambdaEqualityComparer(Func<T, T, bool> equals, Func<T, int> hashCode)
        {
            _equals = equals;
            _hashCode = hashCode;
        }

        private readonly Func<T, T, bool> _equals;
        private readonly Func<T, int> _hashCode;

        public override bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return _hashCode(obj);
        }
    }

    public static class LambdaEqualityComparerExtensions
    {
        public static IEnumerable<T> Union<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> equals)
        {
            return first.Union(second, new LambdaEqualityComparer<T>(equals, x => x.GetHashCode()));
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> equals, Func<T, int> hashCode)
        {
            return first.Union(second, new LambdaEqualityComparer<T>(equals, hashCode));
        }
    }
}
