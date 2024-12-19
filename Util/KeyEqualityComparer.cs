using System.Diagnostics.CodeAnalysis;

namespace RoelerCoaster.AdventOfCode.Year2024.Util;
public class KeyEqualityComparer<T, TKey> : EqualityComparer<T> where TKey : notnull
{
    private readonly Func<T?, TKey> _keySelector;
    private readonly IEqualityComparer<TKey> _equalityComparer;


    public KeyEqualityComparer(Func<T?, TKey> keySelector) : this(keySelector, EqualityComparer<TKey>.Default)
    {

    }

    public KeyEqualityComparer(Func<T?, TKey> keySelector, IEqualityComparer<TKey> equalityComparer)
    {
        _keySelector = keySelector;
        _equalityComparer = equalityComparer;
    }


    public override bool Equals(T? x, T? y)
    {
        return _equalityComparer.Equals(_keySelector(x), _keySelector(y));
    }

    public override int GetHashCode([DisallowNull] T obj)
    {
        return _equalityComparer.GetHashCode(_keySelector(obj));
    }
}
