using System.IO;
using System.Runtime.CompilerServices;
using CommandLine;
using AudioFileUtils;
using System.Runtime.Versioning;

namespace Mp3Cleaner;

[SupportedOSPlatform("windows")]
public class Program
{
    internal static long MinimumMp3SizeBytes = 20 * 1024 * 1024; // 20 MB
    internal static int UnknownFileCount = 1;

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
                    HandleFoundFile(topLevelFolder, subFile);
                }
            }
            subFolder.Delete();
        }
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(intercept: true);
    }

    internal static void HandleFoundFile(FolderManager destinationFolder, FileInfo file)
    {
        string destinationFileName = string.Empty;
        try
        {
            // process the file
            AudioFileProcessor fileProcessor = new AudioFileProcessor(file.FullName);
            string bestGuessChapterNumberFileName = fileProcessor.OpeningText(silenceTimeout: 1).Split(' ').First().ToString();
            if (destinationFolder.HasFile($"{bestGuessChapterNumberFileName}.mp3"))
            {
                // TODO: handle multiple duplicates (unlikely?)
                bestGuessChapterNumberFileName = $"{bestGuessChapterNumberFileName} (1)";
            }
            destinationFileName = $"{bestGuessChapterNumberFileName}.mp3";
#if DEBUG
            Console.WriteLine(bestGuessChapterNumberFileName);
#endif
            // for safety, clean up memory - big stream objects!
            fileProcessor.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error trying to process file: {file.FullName}");
            Console.WriteLine(ex.ToString());

            destinationFileName = $"Unknown_{UnknownFileCount}.mp3";
            UnknownFileCount++;
        }

        if (!destinationFolder.MoveFileHere(file.FullName, destinationFileName))
        {
            Console.WriteLine($"Error when trying to move a file into the target folder.");
        }
    }
}