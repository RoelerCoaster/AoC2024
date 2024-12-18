namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;
internal class BxcInstruction : BxlInstruction
{
    protected override long GetOperandValue(ProgramState state, byte operand)
    {
        return state.C;
    }
}
