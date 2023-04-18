using LStorage.Models.Dto;
using LStorage.Models.Works;

namespace LStorage.Dao;

public interface IProjectRepository
{
    Task AddProject(ProjectDto projectDto, CancellationToken cancellationToken);
    Task AddUserToProject(UserProjectDto projectDto, CancellationToken cancellationToken);
    Task DeleteUserFromProject(UserProjectDto projectDto, CancellationToken cancellationToken);
    Task DeleteProject(string projectName, CancellationToken cancellationToken);
    Task<IEnumerable<ProjectDto>> GetProjects(string? email, CancellationToken cancellationToken);
    Task<Project?> GetProject(string projectId, CancellationToken cancellationToken);
    Task<IEnumerable<ProjectDto>> GetAllProjects(CancellationToken cancellationToken);
}