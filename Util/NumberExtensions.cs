using System.Numerics;

namespace RoelerCoaster.AdventOfCode.Year2024.Util;
internal static class NumberExtensions
{
    public static int NumberOfDigits<T>(this T num) where T : IBinaryInteger<T>, IComparable<T>
    {
        var test = T.Abs(num);
        var ten = T.CreateChecked(10);
        var numDigits = 0;

        do
        {
            test /= ten;
            numDigits++;
        } while (test.CompareTo(T.Zero) > 0);

        return numDigits;
    }
}
