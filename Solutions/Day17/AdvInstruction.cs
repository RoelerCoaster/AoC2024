namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;

internal class AdvInstruction : DivisionInstructionBase
{
    protected override void StoreResult(ProgramState state, long result)
    {
        state.A = result;
    }
}
