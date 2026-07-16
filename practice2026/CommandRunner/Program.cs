using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using CommandLib;

namespace CommandRunner;

public class Program
{
    public static void Main()
    {
        string demoDir = Path.Combine(Path.GetTempPath(), "Task08_DemoDir");
        Directory.CreateDirectory(demoDir);
        File.WriteAllText(Path.Combine(demoDir, "data1.txt"), "Hello FIIT");
        File.WriteAllText(Path.Combine(demoDir, "data2.log"), "Some log information");
        File.WriteAllText(Path.Combine(demoDir, "data3.txt"), "C# is awesome");

        try
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string libraryPath = Path.Combine(appDir, "FileSystemCommands.dll");

            if (!File.Exists(libraryPath))
            {
                Console.WriteLine("Ошибка: не удалось найти FileSystemCommands.dll в папке приложения.");
                return;
            }

            Assembly assembly = Assembly.LoadFrom(libraryPath);

            Type dirSizeType = assembly.GetType("FileSystemCommands.DirectorySizeCommand")!;
            if (dirSizeType != null)
            {
                CommandLib.ICommand dirSizeCmd = (CommandLib.ICommand)Activator.CreateInstance(dirSizeType, demoDir)!;
                dirSizeCmd.Execute();

                PropertyInfo sizeProp = dirSizeType.GetProperty("TotalSizeBytes")!;
                long totalSize = (long)sizeProp.GetValue(dirSizeCmd)!;

                Console.WriteLine($"[Информация о каталоге]\nПуть: {demoDir}");
                Console.WriteLine($"Общий размер: {totalSize} байт.\n");
            }

            Type findFilesType = assembly.GetType("FileSystemCommands.FindFilesCommand")!;
            if (findFilesType != null)
            {
                CommandLib.ICommand findCmd = (CommandLib.ICommand)Activator.CreateInstance(findFilesType, demoDir, "*.txt")!;
                findCmd.Execute();

                PropertyInfo foundFilesProp = findFilesType.GetProperty("FoundFilePaths")!;
                var matchedFiles = (List<string>)foundFilesProp.GetValue(findCmd)!;

                Console.WriteLine($"[Результаты поиска (*.txt)]\nНайдено файлов: {matchedFiles.Count}");
                foreach (var filePath in matchedFiles)
                {
                    Console.WriteLine($" - {Path.GetFileName(filePath)}");
                }
            }
        }
        finally
        {
            if (Directory.Exists(demoDir))
            {
                Directory.Delete(demoDir, true);
            }
        }
    }
}