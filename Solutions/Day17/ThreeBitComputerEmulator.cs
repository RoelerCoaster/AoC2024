namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;
internal static class ThreeBitComputerEmulator
{

    private static readonly IReadOnlyDictionary<byte, InstructionBase> Instructions = new Dictionary<byte, InstructionBase>
    {
        {0, new AdvInstruction() },
        {1, new BxlInstruction() },
        {2, new BstInstruction() },
        {3, new JnzInstruction() },
        {4, new BxcInstruction() },
        {5, new OutInstruction() },
        {6, new BdvInstruction() },
        {7, new CdvInstruction() },
    };

    public static void Run(ProgramState state, byte[] program)
    {
        while (state.IP < program.Length)
        {
            var instruction = Instructions[program[state.IP]];
            var operand = program[state.IP + 1];

            instruction.Apply(state, operand);
        }
    }

}
