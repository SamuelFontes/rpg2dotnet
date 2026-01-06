namespace RPG2Dotnet.Models;

public class Column
{
    public string Name { get; set; } = string.Empty;
    public string LanguageFriendlyName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
}

public class DataSource
{
    public string Name { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public bool ReadAccess { get; set; }
    public bool WriteAccess { get; set; }
    public bool UpdateAccess { get; set; }
    public bool IsExternallyDescribed { get; set; }
    public bool IsKeyed { get; set; }
    public bool IsFullProcedural { get; set; }
    public string Device { get; set; } = string.Empty;
    public string RecordFormatName { get; set; } = string.Empty;
    public string RecordFormatRename { get; set; } = string.Empty;
    public List<Column> Columns { get; set; } = new List<Column>();
}
