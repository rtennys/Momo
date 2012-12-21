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

        public static string GetName<T>(Expression<Func<T, object>> propertyExpression)
        {
            return propertyExpression.Body.As<MemberExpression>().Member.Name;
        }

        public static string GetName<T>(this T _, Expression<Func<T, object>> propertyExpression)
        {
            return GetName(propertyExpression);
        }
    }
}