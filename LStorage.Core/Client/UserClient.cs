using Flurl.Http;
using LStorage.Models.Auth;
using LStorage.Models.Dto;
using Serilog;

namespace LStorage.Core.Client;

public class UserClient
{
    private const string _getUsers = "/auth/getusers";
    private const string _createUser = "/auth/createuser";
    private const string _deleteUser = "/auth/deleteuser";
    private string _serverUrl;

    public UserClient(string serverUrl)
    {
        _serverUrl = serverUrl;
    }

    public async Task<List<UserDto>> GetUsers()
    {
        try
        {
            var url = _serverUrl + _getUsers;
            var token = Setting.Setting.Token;
            var response = await url.WithOAuthBearerToken(token).WithHeader("Accept", "text/plain").GetAsync();
            var json = await response.GetJsonAsync<List<UserDto>>();
            return json;
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(GetUsers), e.Message);
        }

        return null;
    }

    public async Task CreateUser(SingUpData data)
    {
        try
        {
            var url = _serverUrl + _createUser;
            await url.WithOAuthBearerToken(Setting.Setting.Token).PostJsonAsync(data);
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(CreateUser), e.Message);
        }
    }

    public async Task DeleteUser(string userEmail)
    {
        try
        {
            var url = _serverUrl + _deleteUser;
            await url.WithOAuthBearerToken(Setting.Setting.Token).PostStringAsync(userEmail);
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(DeleteUser), e.Message);
        }
    }
}