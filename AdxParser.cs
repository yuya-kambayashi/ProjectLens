using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp3
{
    internal class AdxParser
    {
        public Dictionary<string, List<string>> Sections { get; } = new();

        public void Load(string filePath)
        {
            using var reader = new StreamReader(filePath);
            string? line;
            string? currentSection = null;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("*"))
                {
                    currentSection = line.Substring(1).Trim(); // 例: *PROJECT → PROJECT
                    if (!Sections.ContainsKey(currentSection))
                        Sections[currentSection] = new();
                }
                else if (currentSection != null)
                {
                    Sections[currentSection].Add(line);
                }
            }
        }

        public void PrintAll()
        {
            foreach (var section in Sections)
            {
                Console.WriteLine($"[{section.Key}]");
                foreach (var line in section.Value)
                    Console.WriteLine($"  {line}");
            }
        }
    }
}
