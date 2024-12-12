namespace RoelerCoaster.AdventOfCode.Year2024.Util;
public static class GenericExtensions
{

    public static bool IsIn<T>(this T item, params HashSet<T> items)
    {
        return items.Contains(item);
    }
}
