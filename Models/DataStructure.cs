namespace RPG2Dotnet.Models;

public class DataStructure
{
    public string Name { get; set; }
    public string Type { get; set; } // "UDS", "SDS", "DS", "LDA", etc.
    public bool IsExternallyDescribed { get; set; }
    public string ExternalFile { get; set; }
    public List<DataStructureField> Fields { get; set; }
    
    public DataStructure()
    {
        Name = string.Empty;
        Type = "DS";
        ExternalFile = string.Empty;
        Fields = new List<DataStructureField>();
    }
}

public class DataStructureField
{
    public string Name { get; set; }
    public int FromPosition { get; set; }
    public int ToPosition { get; set; }
    public int Length => ToPosition - FromPosition + 1;
    public int DecimalPositions { get; set; }
    public string DataType { get; set; } // "Character", "Numeric", "Packed", etc.
    
    public DataStructureField()
    {
        Name = string.Empty;
        DataType = "Character";
    }
}

public class CopyDirective
{
    public string Library { get; set; }
    public string Member { get; set; }
    
    public CopyDirective()
    {
        Library = string.Empty;
        Member = string.Empty;
    }
}
