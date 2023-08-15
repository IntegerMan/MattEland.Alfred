namespace MattEland.Alfred.Cli;

internal class Program
{
    private static int Main(string[] args)
    {
        AlfredCliProgram alfred = new();
        return alfred.Run(args);
    }
}