using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public static class ClassGenerator
    {
        public static ICalculator CreateCalculator(string row)
        {
            if (string.IsNullOrWhiteSpace(row))
            {
                throw new ArgumentNullException(nameof(row));
            }
            string newRow = row.Replace("public class Calculator","public class Calculator: task11.ICalculator");

            var syntaxTree = CSharpSyntaxTree.ParseText(newRow);

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
                MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location),
            };

            var compilation = CSharpCompilation.Create(
                $"DynamicCalculator_{Guid.NewGuid():N}",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    throw new InvalidOperationException("Ошибка компиляции");
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());

                Type calculatorType = assembly.GetType("Calculator");
                if (calculatorType == null)
                {
                    throw new InvalidOperationException("Ошибка, не найден класс Calculator");
                }
                object rawInstance = Activator.CreateInstance(calculatorType);

                return (ICalculator)rawInstance;
            }

            
        }
    }
}
