namespace RPG2Dotnet;

public static class DebugConfig
{
    // Toggle these flags to enable/disable debug output for specific parsers
    public static bool DebugFSpec { get; set; } = true;
    public static bool DebugESpec { get; set; } = true;
    public static bool DebugISpec { get; set; } = true;
    public static bool DebugCSpec { get; set; } = true;
    public static bool DebugOSpec { get; set; } = true;
    
    // Master debug toggle - if false, all debug output is disabled
    public static bool EnableDebug { get; set; } = true;
    
    public static bool IsEnabled(string specType)
    {
        if (!EnableDebug) return false;
        
        return specType.ToUpper() switch
        {
            "F" => DebugFSpec,
            "E" => DebugESpec,
            "I" => DebugISpec,
            "C" => DebugCSpec,
            "O" => DebugOSpec,
            _ => false
        };
    }
}
