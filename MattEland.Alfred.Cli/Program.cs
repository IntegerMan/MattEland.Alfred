namespace MattEland.Alfred.Cli;

internal class Program
{
    private static int Main(string[] args)
    {
        AlfredProgram alfred = new();
        return alfred.Run(args);
    }
}