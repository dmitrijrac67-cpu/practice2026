using System;
using System.Reflection;

namespace task07
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public DisplayNameAttribute(string name) => DisplayName = name;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : Attribute
    {
        public int Major { get; }
        public int Minor { get; }

        public VersionAttribute(int majorVersion, int minorVersion)
        {
            Major = majorVersion;
            Minor = minorVersion;
        }
    }

    [DisplayName("Пример класса")]
    [Version(1, 0)]
    public class SampleClass
    {
        [DisplayName("Числовое свойство")]
        public int Number { get; set; }

        [DisplayName("Тестовый метод")]
        public void TestMethod() { }
    }

    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            if (type.GetCustomAttribute<DisplayNameAttribute>() is { } displayAttr)
            {
                Console.WriteLine($"[{type.Name}] - {displayAttr.DisplayName}");
            }

            if (type.GetCustomAttribute<VersionAttribute>() is { } versionAttr)
            {
                Console.WriteLine($"Версия: {versionAttr.Major}.{versionAttr.Minor}");
            }

            Console.WriteLine("\n--- Методы ---");
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
            bool foundMethods = false;

            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<DisplayNameAttribute>() is { } methodAttr)
                {
                    Console.WriteLine($"{method.Name}() -> {methodAttr.DisplayName}");
                    foundMethods = true;
                }
            }
            if (!foundMethods) Console.WriteLine("Методов с атрибутами не найдено.");

            Console.WriteLine("\n--- Свойства ---");
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
            bool foundProperties = false;

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute<DisplayNameAttribute>() is { } propAttr)
                {
                    Console.WriteLine($"{prop.Name} (тип {prop.PropertyType.Name}) -> {propAttr.DisplayName}");
                    foundProperties = true;
                }
            }
            if (!foundProperties) Console.WriteLine("Свойств с атрибутами не найдено.");
        }
    }
}