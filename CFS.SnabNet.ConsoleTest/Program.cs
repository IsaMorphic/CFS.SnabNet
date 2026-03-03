using CFS.SnabNet;
using Microsoft.Extensions.FileSystemGlobbing;

if (args.Length > 0)
{
    Matcher inputFileGlob = new();
    inputFileGlob.AddIncludePatterns(args);
    foreach (string inputFilePath in inputFileGlob.GetResultsInFullPath(Environment.CurrentDirectory))
    {
        Console.WriteLine("Parsing SNAB file: " + Path.GetFileName(inputFilePath));
        string outputFilePath = Path.ChangeExtension(inputFilePath, ".snab.out");

        object? parsedData;
        using (SnabReader reader = new(File.OpenRead(inputFilePath)))
        {
            parsedData = reader.Deserialize();
        }

        Console.WriteLine("Serializing to SNAB file: " + Path.GetFileName(outputFilePath));
        using (SnabWriter writer = new(File.Create(outputFilePath), SnabFlags.None))        
        {
            writer.Serialize(parsedData);
        }
    }
    return 0;
}
else
{
    Console.Error.WriteLine("No command-line arguments were specified.");
    return -1;
}
