namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;

internal class OutInstruction : ModuloInstructionBase
{
    protected override void StoreResult(ProgramState state, byte result)
    {
        state.Out.Add(result);
    }
}