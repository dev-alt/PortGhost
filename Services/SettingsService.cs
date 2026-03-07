using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PortGhost.Services;

public class AppSettings
{
    public List<int> CustomPresets { get; set; } = new() { 3000, 8080, 5173, 4200 };
}

public static class SettingsService
{
    private static readonly string _settingsDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PortGhost");

    private static readonly string _settingsPath =
        Path.Combine(_settingsDir, "settings.json");

    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch
        {
            // Return defaults on any read/parse error
        }
        return new AppSettings();
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(_settingsDir);
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
            // Silently fail — settings are not critical
        }
    }
}
