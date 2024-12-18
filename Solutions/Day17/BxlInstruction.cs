namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;
internal class BxlInstruction : InstructionBase
{
    protected override OperandType OperandType => OperandType.Literal;

    protected override void DoApply(ProgramState state, long operandValue)
    {
        state.B = state.B ^ operandValue;
    }
}
