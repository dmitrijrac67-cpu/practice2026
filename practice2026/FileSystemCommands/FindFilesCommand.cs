using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands;

public class FindFilesCommand : CommandLib.ICommand
{
    public string TargetDirectory { get; }
    public string SearchPattern { get; }
    public List<string> FoundFilePaths { get; private set; }

    public FindFilesCommand(string directoryPath, string pattern)
    {
        TargetDirectory = directoryPath;
        SearchPattern = pattern;
        FoundFilePaths = new List<string>();
    }

    public void Execute()
    {
        FoundFilePaths.Clear();

        if (!Directory.Exists(TargetDirectory))
            return;

        var dirInfo = new DirectoryInfo(TargetDirectory);

        FoundFilePaths = dirInfo
            .EnumerateFiles(SearchPattern, SearchOption.AllDirectories)
            .Select(file => file.FullName)
            .ToList();
    }
}