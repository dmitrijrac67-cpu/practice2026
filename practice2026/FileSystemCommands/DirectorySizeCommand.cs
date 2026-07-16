using CommandLib;
using System.IO;
using System.Linq;

namespace FileSystemCommands;

public class DirectorySizeCommand : CommandLib.ICommand
{
    public string TargetDirectory { get; }
    public long TotalSizeBytes { get; private set; }

    public DirectorySizeCommand(string directoryPath)
    {
        TargetDirectory = directoryPath;
    }

    public void Execute()
    {
        TotalSizeBytes = 0;

        if (!Directory.Exists(TargetDirectory))
            return;

        var dirInfo = new DirectoryInfo(TargetDirectory);

        TotalSizeBytes = dirInfo
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .Sum(file => file.Length);
    }
}