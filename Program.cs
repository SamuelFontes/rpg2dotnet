
namespace RPG2Dotnet;

using System.Diagnostics;
using RPG2Dotnet.Models;
using RPG2Dotnet.Parsers;

// Open file

class Program
{
    static void Main(string[] args)
    {
        // Debug configuration - toggle these to control parser debug output
        DebugConfig.EnableDebug = true;     // Master switch
        DebugConfig.DebugFSpec = false;      // F-spec (file definitions)
        DebugConfig.DebugESpec = true;      // E-spec (arrays/tables)
        DebugConfig.DebugISpec = false;     // I-spec (input specs) - not implemented yet
        DebugConfig.DebugCSpec = false;     // C-spec (calculation specs) - not implemented yet
        DebugConfig.DebugOSpec = false;     // O-spec (output specs) - not implemented yet

        string filePath = "/home/sfontes/Code/rpg2dotnet/ExampleHidden/P56400P.MBR";

        List<DataSource> dataSources = new List<DataSource>();
        List<ArrayTable> arrayTables = new List<ArrayTable>();
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
                else if (instructionType == 'E')
                {
                    ESpecParser.ParseESpec(line, arrayTables);
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


        if (DebugConfig.IsEnabled("F"))
        {
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
                }
            }
        }
        if (DebugConfig.IsEnabled("E"))
        {
            // Output parsed arrays/tables
            Console.WriteLine("\n=== Parsed Arrays/Tables ===");
            foreach (var arr in arrayTables)
            {
                Console.WriteLine($"Array: {arr.Name}");
                Console.WriteLine($"  Entries: {arr.NumberOfEntries}");
                Console.WriteLine($"  Entry Length: {arr.EntryLength} digits");
                Console.WriteLine($"  Decimal Positions: {arr.DecimalPositions}");
                Console.WriteLine($"  Data Type: {arr.DataType}");
                if (!string.IsNullOrEmpty(arr.FromFileName))
                {
                    Console.WriteLine($"  From File: {arr.FromFileName}");
                }
                if (!string.IsNullOrEmpty(arr.ToFileName))
                {
                    Console.WriteLine($"  To File: {arr.ToFileName}");
                }
                Console.WriteLine($"  C# Type: decimal[{arr.NumberOfEntries}] (with scale of {arr.DecimalPositions})");
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }
}