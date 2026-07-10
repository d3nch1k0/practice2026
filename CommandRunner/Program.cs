using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string dllName = "FileSystemCommands.dll";
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string dllPath = "";

            string path1 = Path.Combine(exePath, dllName);
            if (File.Exists(path1)) dllPath = path1;

            if (string.IsNullOrEmpty(dllPath))
            {
                string path2 = Path.Combine(exePath, "..", "..", "..", "..", "FileSystemCommands", "bin", "Debug", "net10.0", dllName);
                if (File.Exists(path2)) dllPath = path2;
            }

            if (string.IsNullOrEmpty(dllPath))
            {
                Console.WriteLine($"Не найден файл динамической библиотеки: {dllName}");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                string sampleDir = Path.Combine(Path.GetTempPath(), "RunnerTestDir_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(sampleDir);
                Console.WriteLine($"Тестовая директория: {sampleDir}");

                File.WriteAllText(Path.Combine(sampleDir, "document.txt"), "abcdefg");
                File.WriteAllText(Path.Combine(sampleDir, "text.txt"), "Text file");
                Console.WriteLine("Созданы файлы: document.txt, text.txt\n");


                var commandTypes = assembly.GetTypes()
                    .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    .ToList();

                Console.WriteLine($"Найдено команд: {commandTypes.Count}");

                foreach (var type in commandTypes)
                {
                    Console.WriteLine(type.Name);
                    ICommand commandInstance = null;

                    try
                    {
                        if (type.Name == "DirectorySizeCommand")
                        {
                            commandInstance = (ICommand)Activator.CreateInstance(type, new object[] { sampleDir });
                            Console.WriteLine($"Создан экземпляр DirectorySizeCommand с директорией: {sampleDir}");
                        }
                        else if (type.Name == "FindFilesCommand")
                        {
                            commandInstance = (ICommand)Activator.CreateInstance(type, new object[] { sampleDir, "*.txt" });
                            Console.WriteLine($"Создан экземпляр FindFilesCommand с директорией: {sampleDir} и маской: *.txt");
                        }
                        else
                        {
                            commandInstance = (ICommand)Activator.CreateInstance(type);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                        continue;
                    }

                    if (commandInstance != null)
                    {
                        try
                        {
                            Console.WriteLine("Выполнение команды:");
                            commandInstance.Execute();
                            Console.WriteLine("Команда выполнена.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка: {ex.Message}");
                        }
                    }
                    Console.WriteLine();
                }

                try
                {
                    Directory.Delete(sampleDir, true);
                    Console.WriteLine($"Директория удалена: {sampleDir}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
