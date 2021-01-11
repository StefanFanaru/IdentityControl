using System;

// ReSharper disable once CheckNamespace
namespace Ardalis.GuardClauses
{
    public static class CustomGuardClauses
    {
        public static void OverLimit<T>(this IGuardClause guardClause, T input, T limit, string parameterName)
            where T : IComparable
        {
            if (input.CompareTo(limit) > 0)
            {
                throw new ArgumentException("Exceeded the allowed limit", parameterName);
            }
        }

        public static void UnderLimit<T>(this IGuardClause guardClause, T input, T limit, string parameterName)
            where T : IComparable
        {
            if (input.CompareTo(limit) < 0)
            {
                throw new ArgumentException("Was under the allowed limit", parameterName);
            }
        }
    }
}