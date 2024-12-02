using RoelerCoaster.AdventOfCode.Year2024.Solutions;

namespace RoelerCoaster.AdventOfCode.Year2024.Internals;
internal class DayResolver
{
    private readonly IEnumerable<DayBase> _days;

    public DayResolver(IEnumerable<DayBase> days)
    {
        _days = days;
    }

    public DayBase GetLatest()
    {
        var d = _days
                .OrderByDescending(d => d.Day)
                .FirstOrDefault(d => d.IsActive);

        return d ?? throw new InvalidOperationException($"No active days found.");
    }

    public DayBase Get(int day)
    {
        var d = _days.FirstOrDefault(d => d.Day == day);

        return d ?? throw new InvalidOperationException($"Implementation for day {day} not found.");
    }
}
