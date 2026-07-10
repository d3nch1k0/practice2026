using System;
using System.Reflection;

namespace task09
{
    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {

            var classDisplay = type.GetCustomAttribute<DisplayNameAttribute>();
            if (classDisplay != null)
            {
                Console.WriteLine("Имя класса: " + classDisplay.DisplayName);
            }

            var classVersion = type.GetCustomAttribute<VersionAttribute>();
            if (classVersion != null)
            {
                Console.WriteLine("Версия класса: " + classVersion.Major + "." + classVersion.Minor);
            }


            Console.WriteLine("Свойства:");
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var propDisplay = prop.GetCustomAttribute<DisplayNameAttribute>();
                if (propDisplay != null)
                {
                    Console.WriteLine(prop.Name + ": " + propDisplay.DisplayName);
                }
            }


            Console.WriteLine("Методы:");
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var methodDisplay = method.GetCustomAttribute<DisplayNameAttribute>();
                if (methodDisplay != null)
                {
                    Console.WriteLine(method.Name + ": " + methodDisplay.DisplayName);
                }
            }
        }
    }
}
