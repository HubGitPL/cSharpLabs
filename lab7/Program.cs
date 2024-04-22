using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

class Program
{
    static void Main(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("Give a path as an argument");
            return;
        }
        string path = args[0];
        DirectoryInfo dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            Console.WriteLine("Directory does not exist");
            return;
        }
        DisplayDirectoryContents(dir, 0);
        
        DisplayTheOldestItem(dir);
        //
        // SortedDictionary<string, int> fileExtensions = GetFileExtensions(dir);
        // DisplayFileExtensions(fileExtensions);
        //
        // BinarySerialize(fileExtensions);
        // BinaryDeserialize();
    }
    static void DisplayDirectoryContents(DirectoryInfo dir, int indent)
    {
        foreach (var f in dir.GetFiles())
        {
            Console.WriteLine(new String('-', indent) + f.Name + " " + f.Length + " bajtów " + $"{GetDosAttributes(f)}");
        }
        foreach (var subDir in dir.GetDirectories())
        {
            Console.WriteLine(new String('-', indent) + subDir.Name + " (" + subDir.GetFileSystemInfos().Length + ")");
            DisplayDirectoryContents(subDir, indent + 4);
        }
    }
    
    static string GetDosAttributes(FileInfo file)
    {
        string attributes = "";
        if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        {
            attributes += "R";
        }
        if ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
        {
            attributes += "H";
        }
        if ((file.Attributes & FileAttributes.System) == FileAttributes.System)
        {
            attributes += "S";
        }
        if ((file.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
        {
            attributes += "A";
        }
        
        return attributes;
    }
    
    static void DisplayTheOldestItem(DirectoryInfo dir)
    {
        DateTime oldest = DateTime.Now;
        FileSystemInfo oldestItem = null;
        foreach (var f in dir.GetFiles())
        {
            if (f.CreationTime < oldest)
            {
                oldest = f.CreationTime;
                oldestItem = f;
            }
        }
        
        foreach (var subDir in dir.GetDirectories())
        {
            if (subDir.CreationTime < oldest)
            {
                oldest = subDir.CreationTime;
                oldestItem = subDir;
            }
        }
        
        Console.WriteLine("Najstarszy element: " + oldestItem.Name + " " + oldest);
    }
}