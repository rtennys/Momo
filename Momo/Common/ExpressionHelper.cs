using System;
using System.Linq.Expressions;

namespace Momo.Common
{
    public static class ExpressionHelper
    {
        public static Func<T, object> GetSelector<T>(string property)
        {
            var param = Expression.Parameter(typeof(T), "item");
            var prop = Expression.Property(param, property);
            var expression = Expression.Convert(prop, typeof(object));

            return Expression.Lambda<Func<T, object>>(expression, param).Compile();
        }
    }
}