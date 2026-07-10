using System;
using System.IO;
using System.Reflection;

namespace task09
{
    [DisplayName("Тест")]
    [Version(2, 6)]
    public class SampleTestClass
    {
        public string Data { get; set; }
        public SampleTestClass(string initialData, int count)
        {
            Data = initialData;
        }

        [DisplayName("Метод обработки файлов")]
        public bool ProcessFile(string path, int mode)
        {
            return true;
        }
    }
    class Program
    {
        static void PrintParameters(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {

                Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");

                if (i < parameters.Length - 1) Console.Write(", ");
            }
        }
        static void Main(string[] args)
        {

            string dllPath;


            if (args.Length > 0)
            {
                dllPath = args[0]; 
            }
            else
            {

                dllPath = Assembly.GetExecutingAssembly().Location;
            }

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Ошибка. Файл не найден по пути: {dllPath}");
                return;
            }

            try
            {

                Assembly assembly = Assembly.LoadFrom(dllPath);
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {

                    if (!type.IsClass) continue;

                    Console.WriteLine($"Класс: {type.FullName}");

                    var classAttributes = type.GetCustomAttributes();
                    foreach (var attr in classAttributes)
                    {
                        if (attr.GetType().Name.StartsWith("Nullable"))
                            continue;
                        if (attr is DisplayNameAttribute dna)
                            Console.WriteLine($"Атрибут DisplayName = \"{dna.DisplayName}\"");
                        else if (attr is VersionAttribute va)
                            Console.WriteLine($"Атрибут Version = {va.Major}.{va.Minor}");
                        else
                            Console.WriteLine($"Атрибут {attr.GetType().Name}");
                    }

                    Console.WriteLine("Конструкторы:");
                    ConstructorInfo[] constructors = type.GetConstructors();
                    foreach (var ctor in constructors)
                    {
                        Console.Write($"- {type.Name}");
                        ParameterInfo[] parameters = ctor.GetParameters();
                        PrintParameters(parameters);
                        Console.WriteLine();
                        
                    }


                    Console.WriteLine("Методы:");
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {

                        if (method.IsSpecialName) continue;

                        Console.Write($"- {method.ReturnType.Name} {method.Name}");
                        ParameterInfo[] parameters = method.GetParameters();
                        PrintParameters(parameters);
                        Console.WriteLine();

                        var methodAttrs = method.GetCustomAttributes();
                        foreach (var attr in methodAttrs)
                        {
                            if (attr is DisplayNameAttribute dna)
                                Console.WriteLine($"Атрибут метода DisplayName = \"{dna.DisplayName}\"");
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Ошибка загрузки типов: {ex.Message}");
                if (ex.LoaderExceptions != null)
                {
                    foreach (var loaderEx in ex.LoaderExceptions)
                    {
                        if (loaderEx != null)
                            Console.WriteLine($"  - {loaderEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

        }

    }
    
}
