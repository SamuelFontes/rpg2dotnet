
namespace RPG2Dotnet;

using System;

// Open file
string filePath = "/home/sfontes/Code/rpg2dotnet/ExampleHidden/P56400P.MBR";

public class DataSource
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public bool ReadAccess { get; set; }
    public bool WriteAccess { get; set; }
    // TODO: add generic collection to hold data equivalent to file rows/records
}
class Program
{
    // The Main method is the entry point
    static void Main(string[] args)
    {

        // read line by line
        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                // break line into words/tokens
                string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Check if there are enough tokens to access the 6th token
                if (tokens.Length < 6)
                {
                    continue;
                }
                // Get the 6th token as it indicates instruction type
                string instructionType = tokens[5];

                if (instructionType.IsNullOrEmpty())
                {
                    continue;
                }

                if (instructionType == 'F')
                {
                    // This is opening a file so we will threat those as Lists in C#
                }


                // Iterate through tokens
                foreach (string token in tokens)
                {
                    // Handle comments by skipping tokens that start with '*'
                    if (token.StartsWith("*"))
                    {
                        break;
                    }

                    Console.WriteLine($"Token: {token}");
                }
            }
        }
    }
}