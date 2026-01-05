
// Open file
string filePath = "/home/sfontes/Code/rpg2dotnet/ExampleHidden/P56400P.MBR";

// read line by line
using (StreamReader sr = new StreamReader(filePath))
{
    string line;
    while ((line = sr.ReadLine()) != null)
    {
        // break line into words/tokens
        string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
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