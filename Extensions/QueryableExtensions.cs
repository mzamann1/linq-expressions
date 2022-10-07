using System;
using System.Linq.Expressions;
using LinqExpressions.Constants;

namespace LinqExpressions.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> SimpleComparison<TSource>(this IQueryable<TSource> source, string key, int value)
        {
            try
            {
                var type = typeof(TSource);
                var pe = Expression.Parameter(type);

                var propertyReference = Expression.Property(pe, key);
                var constantReference = Expression.Constant(value);
                var binaryExpression = Expression.Equal(propertyReference, constantReference);
                Expression<Func<TSource, bool>> lambda = Expression.Lambda<Func<TSource, bool>>(binaryExpression, pe);
                return source.Where(lambda);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return source;
            }

        }

        public static IQueryable<TSource> WhereCustom<TSource>(this IQueryable<TSource> source, string key, string value, MethodType methodType = MethodType.Empty)
        {
            /*
            *   Check if Key is null or empty space, or
            *   if someone has provided only the key 
            */
            if (string.IsNullOrWhiteSpace(key) || (string.IsNullOrWhiteSpace(value) && methodType != MethodType.Empty))
            {
                return source;
            }

            /*
            * Checking the type of TSource Object , for further processing
            */

            Type sourceType = typeof(TSource);

            /*
            * Creating Parameter Expression  t = >  , parameter  t will be of type TSource
            */

            var parameterExp = Expression.Parameter(sourceType, "t");

            /*
            * Getting Type of the Key tName
            */

            var propertyType = GetPropertyType(sourceType, key);

            /*
            * Now we need to create expression for that property
            */

            var memberExp = sourceType.GetProperty(key) == null ? default : Expression.Property(parameterExp, key);

            /*
            * Now we need to convert the provided value's datatype to the property type from which it is going to be matched.
            */
            var convertedConstvalue = Expression.Convert(Expression.Constant(value), propertyType);

            Expression finalExp = default;

            switch (methodType)
            {

                case MethodType.Empty:
                    finalExp = Expression.Call(typeof(string), nameof(string.IsNullOrWhiteSpace), null, memberExp);
                    break;

                case MethodType.StartsWith:

                    /*
                        * Getting StartsWith Method from string class using reflection
                    */

                    var startsWithInfo = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });
                    finalExp = Expression.Call(memberExp, startsWithInfo, convertedConstvalue);
                    break;

                case MethodType.EndsWith:

                    /*
                         * Getting EndsWith Method from string class using reflection
                    */

                    var endsWithInfo = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });
                    finalExp = Expression.Call(memberExp, endsWithInfo, convertedConstvalue);
                    break;

                default:
                    break;
            }

            var lambda = Expression.Lambda(finalExp, false, parameterExp);
            var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { sourceType }, source.Expression, lambda);

            return source.Provider.CreateQuery<TSource>(whereExpression);
        }

        private static Type GetPropertyType(Type type, string propName)
        {
            var property = type.GetProperty(propName);

            if (property == null)
            {
                return default;
            }

            return property.PropertyType;
        }

    }
}