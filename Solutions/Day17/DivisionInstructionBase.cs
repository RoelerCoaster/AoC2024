namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;
internal abstract class DivisionInstructionBase : InstructionBase
{
    protected override OperandType OperandType => OperandType.Combo;

    protected override void DoApply(ProgramState state, long operandValue)
    {
        var result = state.A >> (int)Math.Min(64, operandValue);

        StoreResult(state, result);
    }

    protected abstract void StoreResult(ProgramState state, long result);
}
