using CommandLib;
using System.Reflection;

namespace PluginSystem;

class Program
{
    static List<Type> allPluginTypes = new List<Type>();
    static List<Type> sortedPlugins = new List<Type>();

    static HashSet<string> visiting = new HashSet<string>();
    static HashSet<string> visited = new HashSet<string>();

    static void SortPlugins(Type type)
    {
        if (visited.Contains(type.Name)) return;

        if (visiting.Contains(type.Name))
        {
            throw new InvalidOperationException($"Обнаружена циклическая зависимость в плагине: {type.Name}!");
        }

        visiting.Add(type.Name);

        var attribute = type.GetCustomAttribute<PluginLoadAttribute>();
        if (attribute != null)
        {
            string pastName = attribute.PluginLoadPastNode;

            if (!string.IsNullOrEmpty(pastName))
            {
                foreach (var plugin in allPluginTypes)
                {
                    if (plugin.Name == pastName)
                    {
                        SortPlugins(plugin);
                        break;
                    }
                }
            }
        }

        visiting.Remove(type.Name);
        visited.Add(type.Name);
        sortedPlugins.Add(type);
    }

    public static void Main()
    {

        string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        if (!Directory.Exists(pluginsDir))
        {
            Directory.CreateDirectory(pluginsDir);
        }

        string[] dllFiles = Directory.GetFiles(pluginsDir, "*.dll");

        foreach (string dllPath in dllFiles)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);

                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && typeof(ICommand).IsAssignableFrom(type))
                    {
                        allPluginTypes.Add(type);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось загрузить сборку {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        try
        {
            foreach (var type in allPluginTypes)
            {
                SortPlugins(type);
            }

            Console.WriteLine($"Найдено плагинов: {sortedPlugins.Count}\n");


            foreach (var type in sortedPlugins)
            {
                if (Activator.CreateInstance(type) is ICommand instance)
                {
                    instance.Execute();
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.ResetColor();
        }
    }
}
