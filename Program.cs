
namespace RPG2Dotnet;

using RPG2Dotnet.Models;
using RPG2Dotnet.Parsers;

// Open file

class Program
{
    static void Main(string[] args)
    {
        string filePath = "/home/sfontes/Code/rpg2dotnet/ExampleHidden/P56400P.MBR";

        List<DataSource> dataSources = new List<DataSource>();
        DataSource? currentDataSource = null;

        // read line by line
        using (StreamReader sr = new StreamReader(filePath))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                // Skip empty or too-short lines
                if (line.Length < 6)
                {
                    continue;
                }

                // Skip if line is comment (column 7 = index 6)
                if (line.Length >= 7 && line[6] == '*')
                {
                    continue;
                }

                // Get instruction type from column 6 (index 5) - RPG fixed format
                // Columns 1-5 are typically sequence numbers or blank
                // Column 6 is the spec type (F, I, C, O, etc.)
                char instructionType = line.Length > 5 ? line[5] : ' ';

                if (instructionType == 'F')
                {
                    FSpecParser.ParseFSpec(line, dataSources, ref currentDataSource);
                }
                else
                {
                    // // break line into words/tokens, only get after column 6
                    // string[] tokens = line.Length >= 7 ? line.Substring(6).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();

                    // // Iterate through tokens
                    // foreach (string token in tokens)
                    // {
                    //     // Handle comments by skipping tokens that start with '*'
                    //     if (token.StartsWith("*"))
                    //     {
                    //         break;
                    //     }

                    //     Console.WriteLine($"Token: {token}");
                    // }
                }
            }
        }

        // Output parsed data sources
        Console.WriteLine("\n=== Parsed Data Sources ===");
        foreach (var ds in dataSources)
        {
            Console.WriteLine($"File: {ds.Name}");
            Console.WriteLine($"  Alias: {ds.Alias}");
            Console.WriteLine($"  Device: {ds.Device}");
            Console.WriteLine($"  Read: {ds.ReadAccess}, Write: {ds.WriteAccess}, Update: {ds.UpdateAccess}");
            Console.WriteLine($"  Externally Described: {ds.IsExternallyDescribed}");
            Console.WriteLine($"  Keyed: {ds.IsKeyed}");
            Console.WriteLine($"  Full Procedural: {ds.IsFullProcedural}");
            if (!string.IsNullOrEmpty(ds.RecordFormatName))
            {
                Console.WriteLine($"  Record Format: {ds.RecordFormatName}");
            }
            if (!string.IsNullOrEmpty(ds.RecordFormatRename))
            {
                Console.WriteLine($"  Record Format Rename: {ds.RecordFormatRename}");
            }
            Console.WriteLine();
        }
    }
}