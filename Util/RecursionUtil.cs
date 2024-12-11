namespace RoelerCoaster.AdventOfCode.Year2024.Util;
public static class RecursionUtil
{

    public static Func<TArg, TResult> Memoize<TArg, TResult>(Func<Func<TArg, TResult>, TArg, TResult> implementation, EqualityComparer<TArg>? equalityComparer = null)
        where TArg : notnull
    {
        var cache = new Dictionary<TArg, TResult>(equalityComparer ?? EqualityComparer<TArg>.Default);

        TResult MemoizedImplementation(TArg arguments)
        {
            if (cache.TryGetValue(arguments, out var cached))
            {
                return cached;
            }


            var result = implementation(MemoizedImplementation, arguments);
            cache.Add(arguments, result);

            return result;
        }

        return MemoizedImplementation;
    }
}
