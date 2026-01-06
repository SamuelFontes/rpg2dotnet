namespace RPG2Dotnet.Parsers;

using RPG2Dotnet.Models;

public static class ESpecParser
{
    public static void ParseESpec(string line, List<ArrayTable> arrayTables)
    {
        // E-spec fixed column layout (accounting for 5-column sequence area):
        // Col 1-5 (idx 0-4): Sequence number area
        // Col 6 (idx 5): Spec type ('E')
        // Col 7-16 (idx 6-15): From filename (for alternating tables)
        // Col 17-26 (idx 16-25): To filename (for alternating tables)
        // Col 27-32 (idx 26-31): Array/Table name
        // Col 33-35 (idx 32-34): Number of entries per record (often blank)
        // Col 36-39 (idx 35-38): Number of entries per table (often blank)
        // Col 40-42 (idx 39-41): Number of entries (actual value)
        // Col 43-45 (idx 42-44): Length of entry
        // Col 46-47 (idx 45-46): Decimal positions

        // Array/table name (columns 27-32, idx 26-31)
        string arrayName = "";
        if (line.Length > 31)
        {
            arrayName = line.Substring(26, Math.Min(6, line.Length - 26)).Trim();
        }

        if (string.IsNullOrEmpty(arrayName))
        {
            return; // Not a valid array definition
        }

        ArrayTable array = new ArrayTable
        {
            Name = arrayName
        };

        // From filename (columns 7-16, idx 6-15)
        if (line.Length > 15)
        {
            array.FromFileName = line.Substring(6, 10).Trim();
        }

        // To filename (columns 17-26, idx 16-25) - used for alternating tables
        if (line.Length > 25)
        {
            array.ToFileName = line.Substring(16, 10).Trim();
        }

        // Parse the numeric fields after the array name
        // The numbers appear space-separated starting around column 39
        if (line.Length > 38)
        {
            string numericPart = line.Substring(38).Trim();
            string[] numbers = numericPart.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Typically: [number of entries] [entry length] [decimal positions]
            if (numbers.Length >= 1 && int.TryParse(numbers[0], out int entries))
            {
                array.NumberOfEntries = entries;
            }
            
            if (numbers.Length >= 2 && int.TryParse(numbers[1], out int length))
            {
                array.EntryLength = length;
            }
            
            if (numbers.Length >= 3 && int.TryParse(numbers[2], out int decimals))
            {
                array.DecimalPositions = decimals;
                array.DataType = "Numeric";
            }
            else if (array.EntryLength > 0)
            {
                // If no decimal positions specified but has length, assume numeric with 0 decimals
                array.DecimalPositions = 0;
                array.DataType = "Numeric";
            }
        }

        arrayTables.Add(array);

        if (DebugConfig.IsEnabled("E"))
        {
            Console.WriteLine($"Parsed E-spec: Array '{array.Name}' with {array.NumberOfEntries} entries, " +
                             $"each {array.EntryLength} digits with {array.DecimalPositions} decimal places");
        }
    }
}
