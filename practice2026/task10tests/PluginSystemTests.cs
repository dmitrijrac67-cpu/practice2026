using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using task10;

namespace task10tests;

public static class ExecutionTracker
{
    public static List<string> Order = new();
}

[PluginLoad]
public class AlphaPlugin : IPlugin
{
    public void Execute()
    {
        ExecutionTracker.Order.Add("Alpha");
    }
}

[PluginLoad(Dependencies = new[] { "AlphaPlugin" })]
public class BetaPlugin : IPlugin
{
    public void Execute()
    {
        ExecutionTracker.Order.Add("Beta");
    }
}

[PluginLoad(Dependencies = new[] { "AlphaPlugin", "BetaPlugin" })]
public class GammaPlugin : IPlugin
{
    public void Execute()
    {
        ExecutionTracker.Order.Add("Gamma");
    }
}

public class PluginSystemTests : IDisposable
{
    private readonly string _tempFolder;

    public PluginSystemTests()
    {
        _tempFolder = Path.Combine(Path.GetTempPath(), $"Task10Tests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempFolder);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempFolder))
        {
            Directory.Delete(_tempFolder, true);
        }
    }

    [Fact]
    public void Loader_ExecutesPlugins_InCorrectTopologicalOrder()
    {
        ExecutionTracker.Order.Clear();

        var loader = new PluginLoader();
        string assemblyPath = Path.GetDirectoryName(typeof(PluginSystemTests).Assembly.Location)!;

        loader.LoadPluginsFromDirectory(assemblyPath);
        loader.RunAll();

        Assert.Equal(new[] { "Alpha", "Beta", "Gamma" }, ExecutionTracker.Order);
    }

    [Fact]
    public void Loader_HandlesEmptyDirectory_WithoutExceptions()
    {
        var loader = new PluginLoader();
        loader.LoadPluginsFromDirectory(_tempFolder);
        loader.RunAll();

        Assert.Empty(loader.FoundPlugins);
    }
}