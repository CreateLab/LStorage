using System.Security.Claims;
using LStorage.Dao;
using LStorage.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LStorage.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ProjectController: ControllerBase
{
    private IProjectRepository _projectRepository;

    public ProjectController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }
    
    [HttpPost]
    [Authorize(Roles = "admin")]
    public Task CreateProject([FromBody]ProjectDto projectDto, CancellationToken token)
    {
        projectDto.CreatedAt = DateTime.Now;
        return _projectRepository.AddProject(projectDto, token);
       
    }
    
    [HttpPost]
    [Authorize(Roles = "admin")]
    public Task AddUserToProject(UserProjectDto projectDto, CancellationToken token)
    {
        return _projectRepository.AddUserToProject(projectDto, token);
    }
    
    [HttpPost]
    [Authorize(Roles = "admin")]
    public Task RemoveUserFromProject(UserProjectDto projectDto, CancellationToken token)
    {
        return _projectRepository.DeleteUserFromProject(projectDto, token);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public Task DeleteProject(string projectName, CancellationToken token)
    {
        return _projectRepository.DeleteProject(projectName, token);
    }

    [HttpGet]
    [Authorize]
    public Task<IEnumerable<ProjectDto>> GetProjects(CancellationToken token)
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return role == "admin" ? _projectRepository.GetAllProjects(token) : _projectRepository.GetProjects(email, token);
    }
    
    [HttpGet]
    [Authorize(Roles = "admin")]
    public Task<IEnumerable<ProjectDto>> GetAllProjects(CancellationToken token)
    {
      
        return _projectRepository.GetAllProjects(token);
    }
}