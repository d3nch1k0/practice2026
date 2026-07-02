using System;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string DirPath;
        public long CalculatedSize { get; private set; }

        public DirectorySizeCommand(string dirPath)
        {
            DirPath = dirPath;
        }

        public void Execute()
        {
            if (!Directory.Exists(DirPath))
            {
                Console.WriteLine("Директория не найдена: " + DirPath);
                return;
            }
            CalculatedSize = Directory.GetFiles(DirPath, "*.*", SearchOption.AllDirectories)
            .Select(f => new FileInfo(f).Length)
            .Sum();

            Console.WriteLine("Размер: '" + DirPath + "': " + CalculatedSize + " байт.");
        }
    }
}
