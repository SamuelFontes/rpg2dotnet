namespace RPG2Dotnet.Parsers;

using RPG2Dotnet.Models;

public static class FSpecParser
{
    public static void ParseFSpec(string line, List<DataSource> dataSources, ref DataSource? currentDataSource)
    {
        // RPG fixed column layout:
        // Col 1-5 (idx 0-4): Sequence number area (often blank or line numbers)
        // Col 6 (idx 5): Spec type ('F')
        // Col 7-14 (idx 6-13): File name
        // Col 15 (idx 14): File type (I=Input, O=Output, U=Update, C=Combined)
        // Col 16 (idx 15): File designation (F=Full procedural, P=Primary, S=Secondary, etc.)
        // Col 17 (idx 16): EOF indicator / extent / overflow
        // Col 18 (idx 17): File addition
        // Col 19 (idx 18): External description (E=externally described)
        // Col 20-23 (idx 19-22): Record length/limits (often blank for externally described)
        // Col 24 (idx 23): Access method (K=Keyed)
        // Col 25-38 (idx 24-37): Reserved/additional attributes
        // Col 39-44 (idx 38-43): Device (DISK, PRINTER, WORKSTN, etc.)
        // Col 45-80 (idx 44-79): Keyword area

        // File name is in columns 7-14 (indices 6-13)
        string fileName = line.Length > 13 ? line.Substring(6, 8).Trim() : "";
        
        // If file name is empty, this is a continuation line
        if (string.IsNullOrEmpty(fileName))
        {
            if (currentDataSource != null)
            {
                ParseFSpecContinuation(line, currentDataSource);
            }
            return;
        }

        // Parse main F-spec line
        DataSource ds = new DataSource
        {
            Name = fileName
        };

        // File type (col 15, idx 14)
        if (line.Length > 14)
        {
            char fileType = line[14];
            ds.ReadAccess = fileType == 'I' || fileType == 'U' || fileType == 'C';
            ds.WriteAccess = fileType == 'O' || fileType == 'C';
            ds.UpdateAccess = fileType == 'U' || fileType == 'C';
        }

        // File designation (col 16, idx 15)
        if (line.Length > 15)
        {
            char fileDesignation = line[15];
            ds.IsFullProcedural = fileDesignation == 'F';
        }

        // External description (col 19, idx 18)
        if (line.Length > 18)
        {
            ds.IsExternallyDescribed = line[18] == 'E';
        }

        // Keyed access (col 24, idx 23)
        if (line.Length > 23)
        {
            ds.IsKeyed = line[23] == 'K';
        }

        // Device (col 39-44, idx 38-43)
        if (line.Length > 43)
        {
            ds.Device = line.Substring(38, Math.Min(6, line.Length - 38)).Trim();
        }

        // Parse keywords in keyword area (col 45+, idx 44+)
        if (line.Length > 44)
        {
            string keywordArea = line.Substring(44).Trim();
            ParseFSpecKeywords(keywordArea, ds);
        }

        dataSources.Add(ds);
        currentDataSource = ds;

        if (DebugConfig.IsEnabled("F"))
        {
            Console.WriteLine($"Parsed F-spec: {ds.Name} (Type: {(ds.ReadAccess ? "I" : "")}{(ds.WriteAccess ? "O" : "")}{(ds.UpdateAccess ? "U" : "")})");
        }
    }

    private static void ParseFSpecContinuation(string line, DataSource ds)
    {
        // Continuation lines can contain:
        // - Record format name (around cols 15-25, idx 14-24)
        // - Keywords in keyword area (col 45+, idx 44+)

        // Record format name (approximation - may be in cols 15-25 area)
        if (line.Length > 25 && string.IsNullOrEmpty(ds.RecordFormatName))
        {
            string possibleRecordFormat = line.Substring(14, Math.Min(11, line.Length - 14)).Trim();
            if (!string.IsNullOrEmpty(possibleRecordFormat) && !possibleRecordFormat.StartsWith("K"))
            {
                ds.RecordFormatName = possibleRecordFormat;
            }
        }

        // Parse keywords in keyword area (col 45+, idx 44+)
        if (line.Length > 44)
        {
            string keywordArea = line.Substring(44).Trim();
            ParseFSpecKeywords(keywordArea, ds);
        }
    }

    private static void ParseFSpecKeywords(string keywordArea, DataSource ds)
    {
        // Look for RENAME keyword (may appear as KRENAME in some formats)
        // Format: RENAME(externalFormat:internalName)
        int renameIdx = keywordArea.IndexOf("RENAME", StringComparison.OrdinalIgnoreCase);
        if (renameIdx >= 0)
        {
            // Try to extract the renamed format
            // Old fixed-format may have it as: KRENAMEI5602S or RENAME(I5602:I5602S)
            string renameSection = keywordArea.Substring(renameIdx);
            
            // Check for parentheses format first
            int openParen = renameSection.IndexOf('(');
            if (openParen > 0 && renameSection.Contains(')'))
            {
                int closeParen = renameSection.IndexOf(')');
                string renameContent = renameSection.Substring(openParen + 1, closeParen - openParen - 1);
                string[] parts = renameContent.Split(':');
                if (parts.Length == 2)
                {
                    ds.RecordFormatName = parts[0].Trim();
                    ds.RecordFormatRename = parts[1].Trim();
                }
            }
            else
            {
                // Old format: extract what follows RENAME or KRENAME
                string afterRename = renameSection.Substring("RENAME".Length).Trim();
                if (afterRename.Length > 0)
                {
                    // May be just the new name appended directly
                    ds.RecordFormatRename = afterRename.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
                }
            }
        }

        // Could parse other keywords here: PREFIX, EXTFILE, USROPN, etc.
    }
}
