namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;

internal class BdvInstruction : DivisionInstructionBase
{
    protected override void StoreResult(ProgramState state, long result)
    {
        state.B = result;
    }
}
