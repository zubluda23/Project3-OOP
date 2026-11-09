using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Project3;

public class JsonConferenceService
{
    private readonly JsonSerializerOptions _options;

    public JsonConferenceService()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    public List<Conference> LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл не знайдено: {filePath}");
        }

        string jsonString = File.ReadAllText(filePath);
        var result = JsonSerializer.Deserialize<List<Conference>>(jsonString, _options);
        return result ?? new List<Conference>();
    }

    public void SaveToFile(string filePath, List<Conference> data)
    {
        string jsonString = JsonSerializer.Serialize(data, _options);
        File.WriteAllText(filePath, jsonString);
    }
}