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
    internal static char[] backupFileNameChars = { 'A' };

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
#if !DEBUG
            subFolder.Delete();
#endif
        }
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(intercept: true);
    }

    internal static void HandleFoundFile(FolderManager destinationFolder, FileInfo file)
    {
        AudioFileProcessor? fileProcessor = null;
        try
        {
            // process the file
            fileProcessor = new AudioFileProcessor(file.FullName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error trying to process file: {file.FullName}");
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.StackTrace);
        }
#if DEBUG
        Console.WriteLine(fileProcessor?.OpeningText(silenceTimeout: 2));
#else
        if (!destinationFolder.MoveFileHere(file.FullName))
        {
            Console.WriteLine($"Error when trying to move a file into the target folder.");
        }
        destinationFolder.RenameFile(file.Name, backupFileNameChars + ".mp3");
        backupFileNameChars = IncrementCharExcelStyle(backupFileNameChars);
#endif
        // for safety
        fileProcessor?.WavAudioData.Dispose();
    }

    internal static char[] IncrementCharExcelStyle(char[] chars)
    {
        char[] incrementedChar;
        if (chars.Last() == 'Z')
        {
            // incremented char is one char longer, and reset to all 'A's
            incrementedChar = new char[chars.Length + 1];
            for (int i = 0; i < incrementedChar.Length; i++)
            {
                incrementedChar[i] = 'A';
            }
        }
        else
        {
            // return char array is the same length, with the last char incremented by 1
            incrementedChar = new char[chars.Length];
            Array.Copy(chars, incrementedChar, chars.Length);
            incrementedChar[incrementedChar.Length - 1] = (char)(Convert.ToUInt16(chars[chars.Length - 1]) + 1);
        }

        return incrementedChar;
    }
}