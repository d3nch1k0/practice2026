using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CommandLib;

namespace FileSystemCommands
{
    public class FindFilesCommand : ICommand
    {
        private readonly string DirPath;
        private readonly string SearchPattern;
        public List<string> FoundFiles { get; private set; } = new List<string>();

        public FindFilesCommand(string dirPath, string searchPattern)
        {
            DirPath = dirPath;
            SearchPattern = searchPattern;
        }

        public void Execute()
        {
            if (!Directory.Exists(DirPath))
            {
                Console.WriteLine("Директория не найдена: " + DirPath);
                return;
            }
            FoundFiles = Directory.GetFiles(DirPath, SearchPattern).ToList();

            Console.WriteLine("Файлы по маске " + SearchPattern + ":");
            foreach (var file in FoundFiles)
            {
                Console.WriteLine(Path.GetFileName(file));
            }
        }
    }
}
