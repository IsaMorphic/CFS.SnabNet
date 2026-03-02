using CFS.SnabNet;
using Newtonsoft.Json;

if (args.Length == 1)
{
    string inputFilePath = args[0];
    string outputFilePath = Path.ChangeExtension(inputFilePath, ".json");

    object? parsedData;
    using (SnabReader reader = new(File.OpenRead(inputFilePath)))
    {
        parsedData = reader.Deserialize();
    }

    JsonSerializer serializer = JsonSerializer.Create();
    using (StreamWriter streamWriter = new(outputFilePath))
    using (JsonTextWriter jsonWriter = new(streamWriter) 
    {
        Formatting = Formatting.Indented,
    })
    {
        serializer.Serialize(jsonWriter, parsedData);
    }

    return 0;
}
else
{
    Console.Error.WriteLine("Invalid or none command-line arguments were specified.");
    return -1;
}
