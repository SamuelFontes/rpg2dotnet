namespace RPG2Dotnet.Parsers;

using RPG2Dotnet.Models;

public static class ISpecParser
{
    public static void ParseISpec(string line, List<DataStructure> dataStructures, 
                                   List<CopyDirective> copyDirectives, ref DataStructure? currentDS)
    {
        // I-spec fixed column layout (accounting for 5-column sequence area):
        // Col 1-5 (idx 0-4): Sequence number area
        // Col 6 (idx 5): Spec type ('I')
        // Col 7-16 (idx 6-15): Name area (DS name, file name, etc.)
        // Col 17-18 (idx 16-17): External/Data Structure flags
        // Col 19-20 (idx 18-19): DS type (DS, UDS, SDS, etc.)
        // Col 21-30 (idx 20-29): External name / additional info
        // Col 31-43 (idx 30-42): From/To positions for fields
        // Col 44-52 (idx 43-51): Decimal positions, data type
        // Col 53-58 (idx 52-57): Field name
        
        // Check for /COPY directive first
        if (line.Length > 6 && line.Substring(6).TrimStart().StartsWith("/COPY"))
        {
            ParseCopyDirective(line, copyDirectives);
            return;
        }
        
        // Extract the name area (columns 7-16, idx 6-15)
        string nameArea = line.Length > 15 ? line.Substring(6, 10).Trim() : "";
        
        // Check if this is a DS header line
        if (IsDSHeader(line, nameArea))
        {
            currentDS = ParseDSHeader(line, nameArea);
            dataStructures.Add(currentDS);
            
            if (DebugConfig.IsEnabled("I"))
            {
                Console.WriteLine($"Parsed I-spec DS: {currentDS.Name} ({currentDS.Type})");
            }
            return;
        }
        
        // If we're in a DS, parse subfield
        if (currentDS != null)
        {
            ParseDSSubfield(line, currentDS);
        }
    }
    
    private static bool IsDSHeader(string line, string nameArea)
    {
        // Check for various DS indicators
        if (line.Length < 20) return false;
        
        // Look for DS indicators in the line
        string dsArea = line.Length > 25 ? line.Substring(16, Math.Min(10, line.Length - 16)) : "";
        
        // Check for: "DS", "UDS", "SDS", "E DS"
        return dsArea.Contains("DS") || dsArea.Contains("UDS") || dsArea.Contains("SDS") ||
               (!string.IsNullOrEmpty(nameArea) && dsArea.Trim().StartsWith("E ")) ||
               (line.Length > 20 && line.Substring(16, Math.Min(5, line.Length - 16)).Trim() == "IDS");
    }
    
    private static DataStructure ParseDSHeader(string line, string nameArea)
    {
        DataStructure ds = new DataStructure
        {
            Name = nameArea
        };
        
        // Check the area after the name for DS type indicators
        if (line.Length > 20)
        {
            string typeArea = line.Substring(16).Trim();
            
            if (typeArea.Contains("UDS"))
            {
                ds.Type = "UDS";
            }
            else if (typeArea.Contains("SDS"))
            {
                ds.Type = "SDS";
            }
            else if (typeArea.StartsWith("E ") || typeArea.StartsWith("E  DS"))
            {
                ds.IsExternallyDescribed = true;
                ds.Type = "DS";
                
                // Extract external file name (e.g., "DSF98LDA")
                string[] parts = typeArea.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    ds.ExternalFile = parts[1];
                }
            }
            else if (typeArea.Contains("DS") || typeArea.StartsWith("IDS"))
            {
                ds.Type = "DS";
            }
        }
        
        return ds;
    }
    
    private static void ParseDSSubfield(string line, DataStructure ds)
    {
        // Subfield lines have positions and field names
        // From position: typically around columns 31-35 (idx 30-34)
        // To position: typically around columns 36-43 (idx 35-42)
        // Decimal: around columns 44-45
        // Field name: around columns 53-58 (idx 52-57)
        
        if (line.Length < 52) return;
        
        // Extract the numeric area (contains from/to positions)
        // They're right-aligned numbers
        string positionArea = line.Substring(30, Math.Min(22, line.Length - 30));
        
        // Split and parse numbers
        string[] parts = positionArea.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length < 2) return; // Need at least from and to positions
        
        if (!int.TryParse(parts[0], out int fromPos)) return;
        if (!int.TryParse(parts[1], out int toPos)) return;
        
        // Extract field name (after positions, typically starts around column 53)
        string fieldName = line.Length > 52 ? line.Substring(52).Trim() : "";
        
        if (string.IsNullOrEmpty(fieldName)) return;
        
        DataStructureField field = new DataStructureField
        {
            Name = fieldName,
            FromPosition = fromPos,
            ToPosition = toPos
        };
        
        // Check for decimal positions (if provided)
        if (parts.Length >= 3 && int.TryParse(parts[2], out int decimals) && decimals > 0)
        {
            field.DecimalPositions = decimals;
            field.DataType = "Numeric";
        }
        
        ds.Fields.Add(field);
        
        if (DebugConfig.IsEnabled("I"))
        {
            Console.WriteLine($"  Field: {field.Name} pos {field.FromPosition}-{field.ToPosition} ({field.Length} bytes)");
        }
    }
    
    private static void ParseCopyDirective(string line, List<CopyDirective> copyDirectives)
    {
        // /COPY format: I/COPY library,member
        string copyLine = line.Substring(6).Trim();
        
        if (!copyLine.StartsWith("/COPY")) return;
        
        // Extract the library,member part
        string copySpec = copyLine.Substring(5).Trim();
        
        string[] parts = copySpec.Split(',');
        
        CopyDirective directive = new CopyDirective();
        
        if (parts.Length > 0)
        {
            directive.Library = parts[0].Trim();
        }
        
        if (parts.Length > 1)
        {
            directive.Member = parts[1].Trim();
        }
        
        copyDirectives.Add(directive);
        
        if (DebugConfig.IsEnabled("I"))
        {
            Console.WriteLine($"Parsed I-spec /COPY: {directive.Library},{directive.Member}");
        }
    }
}
