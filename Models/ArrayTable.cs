namespace RPG2Dotnet.Models;

public class ArrayTable
{
    public string Name { get; set; }
    public int NumberOfEntries { get; set; }
    public int EntryLength { get; set; }
    public int DecimalPositions { get; set; }
    public string DataType { get; set; } // "Numeric" or "Character"
    public string FromFileName { get; set; }
    public string ToFileName { get; set; }
    
    public ArrayTable()
    {
        Name = string.Empty;
        FromFileName = string.Empty;
        ToFileName = string.Empty;
        DataType = "Numeric";
    }
}
