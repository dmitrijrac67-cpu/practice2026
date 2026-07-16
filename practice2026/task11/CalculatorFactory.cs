using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11;

public static class CalculatorFactory
{
    public static ICalculator BuildDynamicCalculator()
    {
        string code = @"
            using task11;
            public class Calculator : ICalculator
            {
                public int Add(int a, int b) => a + b;
                public int Minus(int a, int b) => a - b;
                public int Mul(int a, int b) => a * b;
                public int Div(int a, int b) => a / b;
            }";

        var tree = CSharpSyntaxTree.ParseText(code);

        var refs = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
        };

        var compilation = CSharpCompilation.Create(
            "DynamicCalculatorAssembly",
            new[] { tree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);

        if (!emitResult.Success)
        {
            var errors = emitResult.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
            var message = string.Join("\n", errors.Select(e => e.GetMessage()));
            throw new InvalidOperationException($"Ошибка компиляции:\n{message}");
        }

        stream.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(stream.ToArray());
        var calcType = assembly.GetType("Calculator")!;

        return (ICalculator)Activator.CreateInstance(calcType)!;
    }
}