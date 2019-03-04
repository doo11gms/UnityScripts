using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileReaderWriter : MonoBehaviour
{
    public void Write(List<string> lines, string filePath)
    {
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            lines.ForEach(line => sw.WriteLine(line));
        }
    }

    public List<string> Read(string filePath)
    {
        using (StreamReader sr = new StreamReader(filePath))
        {
            List<string> lines = new List<string>();

            while (sr.Peek() >= 0) lines.Add(sr.ReadLine());

            return lines;
        }
    }
}