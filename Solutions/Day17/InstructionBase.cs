namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;
internal abstract class InstructionBase
{
    protected abstract OperandType OperandType { get; }

    public void Apply(ProgramState state, byte operand)
    {
        var operandValue = GetOperandValue(state, operand);
        DoApply(state, operandValue);
        Jump(state);
    }

    protected abstract void DoApply(ProgramState state, long operandValue);

    protected virtual void Jump(ProgramState state)
    {
        state.IP += 2;
    }

    protected virtual long GetOperandValue(ProgramState state, byte operand)
    {
        return OperandType switch
        {
            OperandType.Literal => operand,
            OperandType.Combo => GetComboOperandValue(state, operand),
            _ => throw new NotSupportedException()
        };
    }

    private long GetComboOperandValue(ProgramState state, byte operand)
    {
        return operand switch
        {
            <= 3 => operand,
            4 => state.A,
            5 => state.B,
            6 => state.C,
            _ => throw new NotSupportedException()
        };
    }


}
