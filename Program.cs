
// Open file
string filePath = "/home/sfontes/Code/rpg2dotnet/ExampleHidden/P56400P.MBR";

// read line by line
using (StreamReader sr = new StreamReader(filePath))
{
    string line;
    while ((line = sr.ReadLine()) != null)
    {
        Console.WriteLine(line);

        // break line into words/tokens
        string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (string token in tokens)
        {
            Console.WriteLine($"Token: {token}");
        }
    }
}