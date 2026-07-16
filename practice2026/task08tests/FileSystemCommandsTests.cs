using FileSystemCommands;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace task08tests;

public class FileSystemCommandsTests : IDisposable
{
    private readonly string _tempDir;

    public FileSystemCommandsTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"Task08Test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Fact]
    public void DirectorySizeCommand_CalculatesCorrectSize_InRootDirectory()
    {
        File.WriteAllText(Path.Combine(_tempDir, "file_a.txt"), "Hello");
        File.WriteAllText(Path.Combine(_tempDir, "file_b.txt"), "World");

        var cmd = new DirectorySizeCommand(_tempDir);
        cmd.Execute();

        Assert.Equal(10, cmd.TotalSizeBytes);
    }

    [Fact]
    public void FindFilesCommand_FiltersCorrectly_ByExtension()
    {
        File.WriteAllText(Path.Combine(_tempDir, "target.txt"), "Text");
        File.WriteAllText(Path.Combine(_tempDir, "ignore.log"), "Log");

        var cmd = new FindFilesCommand(_tempDir, "*.txt");
        cmd.Execute();

        Assert.Single(cmd.FoundFilePaths);
        Assert.Contains(cmd.FoundFilePaths, p => Path.GetFileName(p) == "target.txt");
    }

    [Fact]
    public void DirectorySizeCommand_CalculatesCorrectSize_WithNestedDirectories()
    {
        var nestedDir1 = Path.Combine(_tempDir, "Level1");
        var nestedDir2 = Path.Combine(nestedDir1, "Level2");
        Directory.CreateDirectory(nestedDir2);

        File.WriteAllText(Path.Combine(_tempDir, "root.doc"), "Hello World");
        File.WriteAllText(Path.Combine(nestedDir1, "sub1.txt"), "1234567890");
        File.WriteAllText(Path.Combine(nestedDir2, "sub2.doc"), "Document");
        File.WriteAllText(Path.Combine(nestedDir2, "sub3.txt"), "README");

        var cmd = new DirectorySizeCommand(_tempDir);
        cmd.Execute();

        Assert.Equal(35, cmd.TotalSizeBytes);
    }

    [Fact]
    public void FindFilesCommand_FindsFiles_AcrossNestedDirectories()
    {
        var nestedDir1 = Path.Combine(_tempDir, "Level1");
        var nestedDir2 = Path.Combine(nestedDir1, "Level2");
        Directory.CreateDirectory(nestedDir2);

        File.WriteAllText(Path.Combine(_tempDir, "root.doc"), "Hello World");
        File.WriteAllText(Path.Combine(nestedDir1, "sub1.txt"), "1234567890");
        File.WriteAllText(Path.Combine(nestedDir2, "sub2.doc"), "Document");
        File.WriteAllText(Path.Combine(nestedDir2, "sub3.txt"), "README");

        var cmd = new FindFilesCommand(_tempDir, "*.doc");
        cmd.Execute();

        Assert.Equal(2, cmd.FoundFilePaths.Count);
        Assert.Contains(cmd.FoundFilePaths, p => Path.GetFileName(p) == "root.doc");
        Assert.Contains(cmd.FoundFilePaths, p => Path.GetFileName(p) == "sub2.doc");
    }
}