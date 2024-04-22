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
        Console.WriteLine("\n\n");
        SortedDictionary<string, int> filesAndCatalogs = GetFilesAndCatalogsSortedByLength(dir);
        DisplayFileExtensions(filesAndCatalogs);
        
        BinarySerialize(filesAndCatalogs);
        BinaryDeserialize();
    }
    static void DisplayDirectoryContents(DirectoryInfo dir, int indent)
    {
        foreach (var subDir in dir.GetDirectories())
        {
            Console.WriteLine(new String(' ', indent) + subDir.Name + " (" + subDir.GetFileSystemInfos().Length + $") {GetDosAttributes(subDir)}");
            DisplayDirectoryContents(subDir, indent + 4);
        }
        foreach (var f in dir.GetFiles())
        {
            Console.WriteLine(new String(' ', indent) + f.Name + " " + f.Length + " bajtów " + $" {GetDosAttributes(f)}");
        }
        
    }
    
    static string GetDosAttributes(FileSystemInfo file)
    {
        string attributes = "";
        if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        {
            attributes += "R";
        }else
        {
            attributes += "-";
        }
        if ((file.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
        {
            attributes += "A";
        }else
        {
            attributes += "-";
        }
        if ((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
        {
            attributes += "H";
        }else
        {
            attributes += "-";
        }
        if ((file.Attributes & FileAttributes.System) == FileAttributes.System)
        {
            attributes += "S";
        }else
        {
            attributes += "-";
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
        if(oldestItem == null)
        {
            Console.WriteLine("Directory is empty");
            return;
        }
        Console.WriteLine("\nNajstarszy element: " + oldestItem.Name + " " + oldest);
    }

    static SortedDictionary<string, int> GetFilesAndCatalogsSortedByLength(DirectoryInfo dir)
    {
        Comparer comparer = new Comparer();
        SortedDictionary<string, int> files = new SortedDictionary<string, int>(comparer);

        foreach (var f in dir.GetFiles())
        {
            //if file add number of bytes
            files.Add(f.Name, (int)f.Length);
        }
        foreach (var subDir in dir.GetDirectories())
        {
            //if catalog add number of files
            files.Add(subDir.Name, subDir.GetFileSystemInfos().Length);
        }
        return files;
    }

    static void DisplayFileExtensions(SortedDictionary<string, int> filesAndCatalogs)
    {
        foreach (var ext in filesAndCatalogs)
        {
            Console.WriteLine(ext.Key + " " + ext.Value);
        }
    }
    
    static void BinarySerialize(SortedDictionary<string, int> filesAndCatalogs)
    {
        Console.WriteLine("\n\n\nStarting binary serialization");
        FileStream fs = new FileStream("filesAndCatalogs.bin", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fs, filesAndCatalogs);
        fs.Close();
        Console.WriteLine("Serialized");
    }
    
    static void BinaryDeserialize()
    {
        Console.WriteLine("\n\n\nStarting binary deserialization");
        FileStream fs = new FileStream("filesAndCatalogs.bin", FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        SortedDictionary<string, int> filesAndCatalogs = (SortedDictionary<string, int>)formatter.Deserialize(fs);
        fs.Close();
        Console.WriteLine("Deserialization of fileExtensions.bin:");
        foreach (var ext in filesAndCatalogs)
        {
            Console.WriteLine(ext.Key + " " + ext.Value);
        }
        Console.WriteLine("Deserialized");
    }
}
[Serializable]
public class Comparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if(x == null || y == null)
        {
            throw new ArgumentNullException();
        }
        if (x.Length > y.Length)
        {
            return 1;
        }
        else if (x.Length < y.Length)
        {
            return -1;
        }
        else
        {
            return x.CompareTo(y);
        }
    }
}