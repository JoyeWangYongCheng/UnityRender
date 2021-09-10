using System;
using System.IO;
using System.Collections.Generic;

public class FileHelper
{
    Dictionary<int, Dictionary<int, string>> contents;

    public FileHelper()
    {
        contents = new Dictionary<int, Dictionary<int, string>>();
    }

    public bool CreateFile(string path, string[] columns)
    {
        if (File.Exists(path))
        {
            Console.WriteLine(string.Format("文件已存在:{0}", path));
            return false;
        }

        File.WriteAllLines(path, new string[] { string.Join(",", columns) });
        return true;
    }

    public void SaveFile(string path)
    {
        Dictionary<int, Dictionary<int, string>>.Enumerator didise = contents.GetEnumerator();
        Dictionary<int, string>.Enumerator dise;
        string line = string.Empty;
        List<string> lines = new List<string>(contents.Count);
        while (didise.MoveNext())
        {
            dise = didise.Current.Value.GetEnumerator();
            while (dise.MoveNext())
            {
                if (!string.IsNullOrEmpty(line))
                {
                    line += ",";
                }
                line += dise.Current.Value;
            }
            line += "\n";
            lines.Add(line);

            File.AppendAllText(path, line);

            line = string.Empty;
        }

        contents.Clear();

        //File.AppendAllText(path, lines);
    }

    public void WriteFile(int row, int column, string val)
    {
        if (!contents.ContainsKey(row))
        {
            contents.Add(row, new Dictionary<int, string>());
        }
        if (!contents[row].ContainsKey(column))
        {
            contents[row].Add(column, val);
        }
        else
        {
            contents[row][column] = val;
        }
    }
}
