using Flurl.Http;
using LStorage.Core.Setting;
using LStorage.Models.Auth;
using Newtonsoft.Json;
using Serilog;

namespace LStorage.Core.Client;

public class TokenClient
{
    private const string _singIn = "/auth/signin";
    private const string _refresh = "/auth/refresh";
    private string _serverUrl;
    private static bool isSeted = false;

    public TokenClient(string serverUrl)
    {
        if (!isSeted)
        {
            FlurlHttp.ConfigureClient(serverUrl, cli =>
                cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
            isSeted = true;
        }

        _serverUrl = serverUrl;
    }

    public async Task CreateToken(string email, string password)
    {
        var url = _serverUrl + _singIn;
        var singInData = new SingInData
        {
            Email = email,
            Password = password,
        };
        try
        {

            var response = await url.PostJsonAsync(singInData).ConfigureAwait(false);
            var json = await response.GetJsonAsync<ResultToken>().ConfigureAwait(false);
            Setting.Setting.Name = json.Name;
            SettingLoader.UpdateToken(json.Token);
            SettingLoader.UpdateSetting(new SettingDto
            {
                ServerUrl = _serverUrl
            });
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(CreateToken), e.Message);
        }

        
    }
    
    public async Task RefreshToken()
    {
        try
        {
            var url = _serverUrl + _refresh;
            var response = await url.WithOAuthBearerToken(Setting.Setting.Token).GetAsync();
            var json = await response.GetJsonAsync<ResultToken>();
            SettingLoader.UpdateToken(json.Token);
        }
        catch (FlurlHttpException e)
        {
            if (e.StatusCode == 401)
            {
                SettingLoader.UpdateToken(null);
            } else
            {
                Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                    nameof(RefreshToken), e.Message);
            }
        }
    }
    
    
}