namespace LStorage.Core.Setting;

public class Setting
{
    private const string name = "LStorage";
    private const string tokenFile = "token.tkn";
    private const string settingFile = "setting.txt";

    private static string _rootPath  =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), name);

    private static string _tokenPath  =
        Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), name), tokenFile);
    
    private static string _settingPath  =
        Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), name), settingFile);

    private static string? _token;
    private static string? _serverUrl;

    public static string? Token {get => _token; private set => _token = value;}

    public static string RootPath => _rootPath;
    public static string TokenPath => _tokenPath;
    public static string SettingPath => _settingPath;
    public static string? ServerUrl { get => _serverUrl; set => _serverUrl = value; }
    
    public static string Name { get; set; }
    public Setting()
    {
        
    }

    internal Setting(string token)
    {
        _token = token;
    }

    internal Setting(string token, string serverUrl)
    {
        _token = token;
        _serverUrl = serverUrl;
    }

    internal Setting(SettingDto settingDto)
    {
        _serverUrl = settingDto.ServerUrl;
    }
    
}