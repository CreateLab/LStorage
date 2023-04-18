using System.Text.Json;
using Serilog;

namespace LStorage.Core.Setting;

public static class SettingLoader
{
    private static bool IsSeted { get; set; }

    public static bool SetUp()
    {
        var tokenDirectory = Setting.RootPath;
        if (!Directory.Exists(tokenDirectory))
        {
            Directory.CreateDirectory(tokenDirectory);
        }

        var logPath = Path.Combine(tokenDirectory, "log.log");
        Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
        var filePath = Setting.TokenPath;
        if (File.Exists(filePath))
        {
            var token = File.ReadAllText(filePath);
            if (token.Length > 0)
            {
                new Setting(token);
            }
        }
        else
        {
            File.Create(filePath);
        }

        var settingPath = Setting.SettingPath;
        if (File.Exists(settingPath))
        {
            var setting = File.ReadAllText(settingPath);
            if (setting.Length > 0)
            {
                var settingDto = JsonSerializer.Deserialize<SettingDto>(setting);
                new Setting(settingDto);
            }
        }
        else
        {
            File.Create(settingPath);
        }

        IsSeted = true;
        return true;
    }

    public static bool UpdateToken(string token)
    {
        if (!IsSeted)
        {
            throw new Exception("Not Seted");
        }

        new Setting(token);
        File.WriteAllText(Setting.TokenPath, token);
        return true;
    }

    public static bool UpdateSetting(SettingDto settingDto)
    {
        if (!IsSeted)
        {
            throw new Exception("Not Seted");
        }

        new Setting(settingDto);
        var setting = JsonSerializer.Serialize(settingDto);
        File.WriteAllText(Setting.SettingPath, setting);
        return true;
    }
}