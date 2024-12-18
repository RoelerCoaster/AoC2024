namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;

internal class BstInstruction : ModuloInstructionBase
{
    protected override void StoreResult(ProgramState state, byte result)
    {
        state.B = result;
    }
}
