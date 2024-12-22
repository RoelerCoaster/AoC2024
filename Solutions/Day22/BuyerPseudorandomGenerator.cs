namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day22;
internal class BuyerPseudorandomGenerator
{

    public long Seed { get; }
    private long _secret;

    public BuyerPseudorandomGenerator(long seed)
    {
        Seed = seed;
        _secret = seed;
    }


    public long Next()
    {
        var mixer = _secret << 6; // Multiply by 64 is left shift by 6
        _secret ^= mixer; // Mix
        _secret &= (16777216 - 1); // Prune. 16777216 = 2^24, so mod is simply a bitwise and

        mixer = _secret >> 5; // Divide by 32 is right shift by 5
        _secret ^= mixer; // Mix
        _secret &= (16777216 - 1);

        mixer = _secret << 11; // Multiply by 2048 is left shift by 11
        _secret ^= mixer; // Mix
        _secret &= (16777216 - 1);


        return _secret;
    }


}
