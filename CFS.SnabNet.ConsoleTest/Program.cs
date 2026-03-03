using CFS.SnabNet;
using Microsoft.Extensions.FileSystemGlobbing;
using Newtonsoft.Json;

if (args.Length > 0)
{
    Matcher inputFileGlob = new();
    inputFileGlob.AddIncludePatterns(args);
    foreach (string inputFilePath in inputFileGlob.GetResultsInFullPath(Environment.CurrentDirectory))
    {
        Console.WriteLine("Parsing SNAB file: " + Path.GetFileName(inputFilePath));
        string outputFilePath = Path.ChangeExtension(inputFilePath, ".json");

        object? parsedData;
        using (SnabReader reader = new(File.OpenRead(inputFilePath)))
        {
            parsedData = reader.Deserialize();
        }

        Console.WriteLine("Serializing to JSON file: " + Path.GetFileName(outputFilePath));
        JsonSerializer serializer = JsonSerializer.Create();
        using (StreamWriter streamWriter = new(outputFilePath))
        using (JsonTextWriter jsonWriter = new(streamWriter)
        {
            Formatting = Formatting.Indented,
        })
        {
            serializer.Serialize(jsonWriter, parsedData);
        }
    }
    return 0;
}
else
{
    Console.Error.WriteLine("No command-line arguments were specified.");
    return -1;
}
