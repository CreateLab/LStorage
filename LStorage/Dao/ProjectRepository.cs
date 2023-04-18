using LStorage.DB;
using LStorage.Models.Dto;
using LStorage.Models.Links;
using LStorage.Models.Works;
using Microsoft.EntityFrameworkCore;

namespace LStorage.Dao;

public class ProjectRepository : IProjectRepository
{
    private readonly IFileSystemRepository _fileSystemRepository;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    private static readonly SemaphoreSlim _projectLock = new SemaphoreSlim(1, 1);

    public ProjectRepository(IFileSystemRepository fileSystemRepository, IDbContextFactory<AppDbContext> contextFactory)
    {
        _fileSystemRepository = fileSystemRepository;
        _contextFactory = contextFactory;
    }

    public async Task AddProject(ProjectDto projectDto, CancellationToken cancellationToken)
    {
        var path = await _fileSystemRepository.CreateProject(projectDto.Name);
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        context.Projects.Add(new Project
        {
            Name = projectDto.Name,
            Path = path,
            CreatedAt = DateTime.Now,
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddUserToProject(UserProjectDto projectDto, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var project =
            await context.Projects.FirstOrDefaultAsync(p => p.Id.ToString() == projectDto.ProjectId, cancellationToken);
        var users = await context.Users.Where(u => projectDto.UsersEmails.Contains(u.Email))
            .ToListAsync(cancellationToken);
        if (project == null || users.Count == 0)
        {
            throw new Exception("Project or user not found");
        }

        var isProjectUsersExist = await context.ProjectUsers.FirstOrDefaultAsync(
            pu => pu.ProjectId == project.Id && users.Select(u => u.Id).Contains(pu.UserId), cancellationToken);
        if (isProjectUsersExist != null)
        {
            throw new Exception("User already in project");
        }

        var userProjects = users.Select(user => new ProjectUsers { ProjectId = project.Id, UserId = user.Id }).ToList();
        context.ProjectUsers.AddRange(userProjects);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserFromProject(UserProjectDto projectDto, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var project =
            await context.Projects.FirstOrDefaultAsync(p => p.Id.ToString() == projectDto.ProjectId, cancellationToken);
        var users = await context.Users.Where(u => projectDto.UsersEmails.Contains(u.Email))
            .ToListAsync(cancellationToken);
        if (project == null || users.Count == 0)
        {
            throw new Exception("Project or user not found");
        }

        foreach (var user in users)
        {
            var userProject =
                await context.ProjectUsers.FirstOrDefaultAsync(pu =>
                    pu.ProjectId == project.Id && pu.UserId == user.Id, cancellationToken);
            if (userProject != null)
            {
                context.ProjectUsers.Remove(userProject);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProject(string projectName, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var project = await context.Projects.FirstOrDefaultAsync(p => p.Name == projectName, cancellationToken);
        if (project == null)
        {
            throw new Exception("Project not found");
        }

        var projectUsers = await context.ProjectUsers.Where(pu => pu.ProjectId == project.Id)
            .ToListAsync(cancellationToken);
        context.ProjectUsers.RemoveRange(projectUsers);
        context.Projects.Remove(project);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProjectDto>> GetProjects(string? email, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var userId = await context.Users.Where(u => u.Email == email).Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);
        var projectUsers = await context.ProjectUsers.Where(pu => pu.UserId == userId).ToListAsync(cancellationToken);
        var projects = await context.Projects.Where(p => projectUsers.Select(pu => pu.ProjectId).Contains(p.Id))
            .ToListAsync(cancellationToken);
        return projects.OrderBy(x => x.CreatedAt).Select(p => new ProjectDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
        });
    }

    public async Task<Project?> GetProject(string projectId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Projects.FirstOrDefaultAsync(p => p.Id.ToString() == projectId, cancellationToken);
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjects(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var projects = await context.Projects.OrderBy(x => x.CreatedAt).ToListAsync(cancellationToken);
        return projects.Select(p => new ProjectDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
        });
    }
}