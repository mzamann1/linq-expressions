using System;
using System.Collections;
using System.Collections.Generic;
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

        public static IQueryable<TSource> WhereCustom<TSource>(this IQueryable<TSource> source, string key, string value, MethodType methodType = MethodType.Equal)
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

                case MethodType.Contains:

                    /*
                         * Getting EndsWith Method from string class using reflection
                    */

                    var containsInfo = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
                    finalExp = Expression.Call(memberExp, containsInfo, convertedConstvalue);
                    break;

                case MethodType.Equal:

                    finalExp = Expression.Equal(memberExp, convertedConstvalue);
                    break;

                case MethodType.NotEqual:

                    finalExp = Expression.NotEqual(memberExp, convertedConstvalue);
                    break;

                default:
                    break;
            }

            var lambda = Expression.Lambda(finalExp, false, parameterExp);
            var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { sourceType }, source.Expression, lambda);

            return source.Provider.CreateQuery<TSource>(whereExpression);
        }

        public static IQueryable<TSource> WhereCustom<TSource>(this IQueryable<TSource> source, string key, string value, ConditionalOperatorType operatorType = ConditionalOperatorType.Equals)
        {
            /*
            *   Check if Key is null or empty space, or
            *   if someone has provided only the key 
            */
            if (string.IsNullOrWhiteSpace(key) || (string.IsNullOrWhiteSpace(value) && operatorType != ConditionalOperatorType.Empty))
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


            if (IsNumericType(sourceType) && IsNumericValue(value))
            {

            }
            var convertedConstvalue = Expression.Convert(Expression.Constant(value), propertyType);

            Expression finalExp = default;

            switch (operatorType)
            {

                case ConditionalOperatorType.Empty:
                    return source;

                case ConditionalOperatorType.Equals:

                    finalExp = Expression.Equal(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.NotEquals:

                    finalExp = Expression.NotEqual(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.GreaterThan:

                    finalExp = Expression.GreaterThan(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.GreaterThanOrEqual:

                    finalExp = Expression.GreaterThanOrEqual(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.LessThan:

                    finalExp = Expression.LessThan(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.LessThanOrEqual:

                    finalExp = Expression.LessThanOrEqual(memberExp, convertedConstvalue);
                    break;

                default:
                    break;
            }

            var lambda = Expression.Lambda(finalExp, false, parameterExp);
            var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { sourceType }, source.Expression, lambda);

            return source.Provider.CreateQuery<TSource>(whereExpression);
        }


        public static IQueryable<TSource> OrderByCustom<TSource>(this IQueryable<TSource> source, string key, OrderByType orderBy = OrderByType.Ascending)
        {
            try
            {
                /*
            *   Check if Key is null or empty space, or
            *   if someone has provided only the key 
            */
                string methodName = orderBy == OrderByType.Ascending ? "OrderBy" : "OrderByDescending";

                /*
                * Checking the type of TSource Object , for further processing
                */

                Type sourceType = typeof(TSource);

                /*
                * Creating Parameter Expression  t = >  , parameter  t will be of type TSource
                */

                var property = sourceType.GetProperty(key);


                if (property != null)
                {
                    var parameterExp = Expression.Parameter(sourceType, "t");

                    /*
                    * Getting Type of the Key tName
                    */

                    var memberExpression = Expression.PropertyOrField(parameterExp, property.Name);


                    var lambda = Expression.Lambda(memberExpression, new[] { parameterExp });

                    var sortExpression = Expression.Call(typeof(Queryable), methodName, new[] { sourceType, memberExpression.Type }, source.Expression, lambda);

                    return source.Provider.CreateQuery<TSource>(sortExpression);
                }

                throw new ArgumentNullException($"Unable to find Property with name ={key} ");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return source;
            }
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

        private static bool IsNumericType(Type type)
        {
            HashSet<Type> numericTypes = new() { typeof(int), typeof(double), typeof(decimal), typeof(long), typeof(short), typeof(sbyte), typeof(byte), typeof(ulong), typeof(ushort), typeof(uint), typeof(float) };
            return numericTypes.Contains(type) || numericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        private static bool IsNumericValue(object value)
        {
            return value is byte or short or int or long or sbyte or ushort or uint or ulong or decimal or double or float;
        }

    }
}