namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;
internal class JnzInstruction : InstructionBase
{
    protected override OperandType OperandType => OperandType.Literal;

    protected override void DoApply(ProgramState state, long operandValue)
    {
        if (state.A != 0)
        {
            state.IP = operandValue;
        }
    }

    protected override void Jump(ProgramState state)
    {
        if (state.A == 0)
        {
            base.Jump(state);
        }
    }
}
