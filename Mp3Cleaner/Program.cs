using System.IO;
using CommandLine;

namespace Mp3Cleaner;

public class Program
{
    internal static long MinimumMp3SizeBytes = 20 * 1024 * 1024; // 20 MB

    static void Main(string[] args)
    {
        string targetDirectory = string.Empty;
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed<CommandLineOptions>(o =>
            {
                if (o.TargetDirectory != null && o.TargetDirectory.Length > 0)
                {
                    targetDirectory = o.TargetDirectory;
                }
            });

        if (targetDirectory.Equals(string.Empty))
        {
            targetDirectory = Directory.GetCurrentDirectory();
        }

        FolderManager topLevelFolder = new FolderManager(targetDirectory);

        foreach (var subFolder in topLevelFolder.Subdirectories())
        {
            foreach (var subFile in subFolder.Files())
            {
                if (subFile.Length > MinimumMp3SizeBytes)
                {
                    if (!topLevelFolder.MoveFileHere(subFile.FullName))
                    {
                        Console.WriteLine($"Error when trying to move a file into the target folder.");
                    }
                    topLevelFolder.RenameFile(subFile.Name, subFile.Name + ".mp3");
                }
            }
            subFolder.Delete();
        }
    }
}