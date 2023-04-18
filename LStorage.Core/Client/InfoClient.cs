using Flurl.Http;
using Serilog;

namespace LStorage.Core.Client;

public class InfoClient
{
    private string? _serverUrl;
    private const string getFreeSpaceUrl = "/info/getFreeSpace";
    private const string getTotalSpaceUrl = "/info/getTotalSpace";
    
    public InfoClient(string? serverUrl)
    {
        _serverUrl = serverUrl;
    }
    
    public async Task<int> GetFreeSpace()
    {
        try
        {
            var url = _serverUrl + getFreeSpaceUrl;
            var result = await url.WithOAuthBearerToken(Setting.Setting.Token).GetJsonAsync<int>();
            return result;
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }",DateTime.Now.Ticks, nameof(GetFreeSpace),e.Message);
        }

        return 0;
    }
    
    public async Task<int> GetTotalSpace()
    {
        try
        {
            var url = _serverUrl + getTotalSpaceUrl;
            return await url.WithOAuthBearerToken(Setting.Setting.Token).GetJsonAsync<int>();
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }",DateTime.Now.Ticks, nameof(GetTotalSpace),e.Message);
        }

        return 0;
    }

}