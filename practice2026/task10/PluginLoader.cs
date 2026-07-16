using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace task10;

public class PluginLoader
{
    public List<Type> FoundPlugins { get; } = new();

    public void LoadPluginsFromDirectory(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        string[] dllFiles = Directory.GetFiles(folderPath, "*.dll");

        foreach (string dll in dllFiles)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(dll);
                IEnumerable<Type> pluginTypes = assembly.GetTypes()
                    .Where(t => t.IsClass
                             && !t.IsAbstract
                             && typeof(IPlugin).IsAssignableFrom(t)
                             && t.GetCustomAttribute<PluginLoadAttribute>() != null);

                FoundPlugins.AddRange(pluginTypes);
            }
            catch
            {
                continue;
            }
        }
    }

    public void RunAll()
    {
        List<Type> orderedPlugins = GetTopologicalOrder();

        foreach (Type pluginType in orderedPlugins)
        {
            IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType)!;
            plugin.Execute();
        }
    }

    private List<Type> GetTopologicalOrder()
    {
        var result = new List<Type>();
        var visited = new HashSet<Type>();
        var inProgress = new HashSet<Type>();

        void Dfs(Type current)
        {
            if (visited.Contains(current))
            {
                return;
            }

            if (inProgress.Contains(current))
            {
                throw new InvalidOperationException("Обнаружена циклическая зависимость.");
            }

            inProgress.Add(current);

            var attr = current.GetCustomAttribute<PluginLoadAttribute>();
            string[] deps = attr?.Dependencies ?? Array.Empty<string>();

            foreach (string depName in deps)
            {
                Type? depType = FoundPlugins.FirstOrDefault(t => t.Name == depName);
                if (depType != null)
                {
                    Dfs(depType);
                }
            }

            inProgress.Remove(current);
            visited.Add(current);
            result.Add(current);
        }

        foreach (Type plugin in FoundPlugins)
        {
            if (!visited.Contains(plugin))
            {
                Dfs(plugin);
            }
        }

        return result;
    }
}