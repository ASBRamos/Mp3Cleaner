using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3Cleaner;

internal class FolderManager
{
    public string FolderName { get; set; }

    public FolderManager(string folderName)
    {
        FolderName = folderName;
    }

    public IEnumerable<FolderManager> Subdirectories()
    {
        return Directory.GetDirectories(FolderName).Select(x => new FolderManager(x));
    }

    public IEnumerable<FileInfo> Files()
    {
        return Directory.GetFiles(FolderName).Select(x => new FileInfo(x));
    }

    public bool MoveFileHere(string filePath, string? newFileName = null)
    {
        try
        {
            string fileName = Path.GetFileName(filePath);
            File.Move(filePath, Path.Combine(FolderName, newFileName ?? fileName));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool Delete()
    {
        try
        {
            Directory.Delete(FolderName, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool RenameFile(string fileName, string newFileName)
    {
        try
        {
            string filePath = Path.Combine(FolderName, fileName);
            string newFilePath = Path.Combine(FolderName, newFileName);
            File.Move(filePath, newFilePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool HasFile(string fileName)
    {
        return this.Files().Select(file => file.Name).Contains(fileName);
    }
}
