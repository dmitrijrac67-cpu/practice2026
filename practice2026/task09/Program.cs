using System;
using System.Linq;
using System.Reflection;

namespace task09;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        Assembly assembly = Assembly.LoadFrom(args[0]);
        Console.WriteLine($"Библиотека: {assembly.GetName().Name}");

        Type[] allTypes = assembly.GetTypes();
        foreach (Type currentType in allTypes)
        {
            if (!currentType.IsClass)
            {
                continue;
            }

            Console.WriteLine($"Класс {currentType.Name}:");
            Console.WriteLine("Методы:");

            MethodInfo[] methodInfos = currentType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo methodInfo in methodInfos)
            {
                string accessModifier = methodInfo.IsPublic ? "public" : "private";
                string returnType = methodInfo.ReturnType.Name;
                string methodName = methodInfo.Name;
                ParameterInfo[] methodParams = methodInfo.GetParameters();
                string paramsStr = string.Join(", ", methodParams.Select(p => $"{p.ParameterType.Name} {p.Name}"));

                Console.WriteLine($"{accessModifier} {returnType} {methodName}({paramsStr})");
            }

            Console.WriteLine("Атрибуты:");
            object[] attrList = currentType.GetCustomAttributes(false);
            foreach (object attr in attrList)
            {
                Type attrType = attr.GetType();
                Console.WriteLine($"{attrType.Name}");
            }

            Console.WriteLine("Конструкторы:");
            ConstructorInfo[] constructorInfos = currentType.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (ConstructorInfo constructorInfo in constructorInfos)
            {
                string accessModifier = constructorInfo.IsPublic ? "public" : "private";
                ParameterInfo[] constructorParams = constructorInfo.GetParameters();
                string paramsStr = string.Join(", ", constructorParams.Select(p => $"{p.ParameterType.Name} {p.Name}"));

                Console.WriteLine($"{accessModifier} {currentType.Name}({paramsStr})");
            }
        }
    }
}