namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;
internal abstract class ModuloInstructionBase : InstructionBase
{
    protected override OperandType OperandType => OperandType.Combo;

    protected override void DoApply(ProgramState state, long operandValue)
    {
        var result = operandValue & 0b111;

        StoreResult(state, (byte)result);
    }

    protected abstract void StoreResult(ProgramState state, byte result);
}
