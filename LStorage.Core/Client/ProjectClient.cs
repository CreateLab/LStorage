using System.Data;
using Flurl.Http;
using LStorage.Models.Dto;
using Serilog;

namespace LStorage.Core.Client;

public class ProjectClient
{
    private readonly string _serverUrl;

    public ProjectClient(string serverUrl)
    {
        _serverUrl = serverUrl;
    }

    private const string _createProject = "/project/createproject";
    private const string _getProjects = "/project/getprojects";
    private const string _getAllProjects = "/project/getallprojects";
    private const string _deleteProject = "/project/deleteproject";
    private const string _addUserToProject = "/project/addusertoproject";
    private const string _removeUserFromProject = "/project/removeuserfromproject";


    public async Task<IEnumerable<ProjectDto>> GetProjects()
    {
        try
        {
            var url = _serverUrl + _getProjects;
            var response = await url.WithOAuthBearerToken(Setting.Setting.Token).GetAsync();
            var json = await response.GetJsonAsync<IEnumerable<ProjectDto>>();
            return json;
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(GetProjects), e.Message);
        }

        return null;
    }

    public async Task CreateProject(ProjectDto data)
    {
        try
        {
            data.Id = string.Empty;
            var url = _serverUrl + _createProject;
            var token = Setting.Setting.Token;
            await url.WithOAuthBearerToken(token).PostJsonAsync(data);
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(CreateProject), e.Message);
        }
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjects()
    {
        try
        {
            var url = _serverUrl + _getAllProjects;
            var response = await url.WithOAuthBearerToken(Setting.Setting.Token).GetAsync();
            var json = await response.GetJsonAsync<IEnumerable<ProjectDto>>();
            return json;
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(GetAllProjects), e.Message);
        }

        return null;
    }

    public async Task AddUserToProject(string projectId, string email)
    {
        try
        {
            var url = _serverUrl + _addUserToProject;
            await url.WithOAuthBearerToken(Setting.Setting.Token).PostJsonAsync(new UserProjectDto
            {
                ProjectId = projectId,
                UsersEmails = new List<string> { email }
            });
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(AddUserToProject), e.Message);
        }
    }


    public async Task RemoveUserFromProject(string projectId, string email)
    {
        try
        {
            var url = _serverUrl + _removeUserFromProject;
            await url.WithOAuthBearerToken(Setting.Setting.Token).PostJsonAsync(new UserProjectDto
            {
                ProjectId = projectId,
                UsersEmails = new List<string> { email }
            });
        }
        catch (Exception e)
        {
            Log.Logger.Error("{ {@Date} Exception {@Method} {@Param} {@ExceptionMessage} }", DateTime.Now.Ticks,
                nameof(RemoveUserFromProject), e.Message);
        }
        
    }
}