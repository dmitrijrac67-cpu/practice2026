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
        string demoPath = Path.Combine(Path.GetTempPath(), "Task08_DemoDir");
        Directory.CreateDirectory(demoPath);
        File.WriteAllText(Path.Combine(demoPath, "data1.txt"), "Hello FIIT");
        File.WriteAllText(Path.Combine(demoPath, "data2.log"), "Some log information");
        File.WriteAllText(Path.Combine(demoPath, "data3.txt"), "C# is awesome");

        try
        {
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            string libraryFile = Path.Combine(appFolder, "FileSystemCommands.dll");

            if (!File.Exists(libraryFile))
            {
                Console.WriteLine("Ошибка: не удалось найти FileSystemCommands.dll в папке приложения.");
                return;
            }

            Assembly assembly = Assembly.LoadFrom(libraryFile);

            Type dirSizeType = assembly.GetType("FileSystemCommands.DirectorySizeCommand")!;
            if (dirSizeType != null)
            {
                CommandLib.ICommand dirSizeCmd = (CommandLib.ICommand)Activator.CreateInstance(dirSizeType, demoPath)!;
                dirSizeCmd.Execute();

                PropertyInfo sizeProp = dirSizeType.GetProperty("TotalSizeBytes")!;
                long totalBytes = (long)sizeProp.GetValue(dirSizeCmd)!;

                Console.WriteLine($"[Информация о каталоге]\nПуть: {demoPath}");
                Console.WriteLine($"Общий размер: {totalBytes} байт.\n");
            }

            Type findFilesType = assembly.GetType("FileSystemCommands.FindFilesCommand")!;
            if (findFilesType != null)
            {
                CommandLib.ICommand findCmd = (CommandLib.ICommand)Activator.CreateInstance(findFilesType, demoPath, "*.txt")!;
                findCmd.Execute();

                PropertyInfo foundProp = findFilesType.GetProperty("FoundFilePaths")!;
                var filesList = (List<string>)foundProp.GetValue(findCmd)!;

                Console.WriteLine($"[Результаты поиска (*.txt)]\nНайдено файлов: {filesList.Count}");
                foreach (var file in filesList)
                {
                    Console.WriteLine($" - {Path.GetFileName(file)}");
                }
            }
        }
        finally
        {
            if (Directory.Exists(demoPath))
            {
                Directory.Delete(demoPath, true);
            }
        }
    }
}