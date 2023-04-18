using LStorage.DB;
using LStorage.Models.Dto;
using LStorage.Models.Works;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LStorage.Dao;

public class FileRepository : IFileRepository
{
    private IProjectRepository _projectRepository;
    private IUserRepository _userRepository;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly IFileSystemRepository _fileSystemRepository;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);


    public FileRepository(IProjectRepository projectRepository, IUserRepository userRepository,
        IFileSystemRepository fileSystemRepository, IDbContextFactory<AppDbContext> contextFactory)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _fileSystemRepository = fileSystemRepository;
        _contextFactory = contextFactory;
    }

    public async Task StoreFile(string fileName, Stream openReadStream, string projectId, string? email,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProject(projectId, cancellationToken);
        if (project == null)
        {
            throw new Exception("Project not found");
        }

        var user = await _userRepository.GetUserByEmail(email, cancellationToken);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        if (!isAdmin)
        {
            var isUserInProject =
                await context.ProjectUsers.FirstOrDefaultAsync(x => x.ProjectId == project.Id && x.UserId == user.Id,
                    cancellationToken);
            if (isUserInProject == null)
            {
                throw new Exception("User not in project");
            }
        }

        var filePath = await _fileSystemRepository.StoreFile(fileName, openReadStream, project.Path, cancellationToken);

        var file = new Models.Works.File
        {
            Name = fileName,
            Path = filePath,
            UserId = user.Id,
            ProjectId = project.Id,
            CreatedAt = DateTime.Now,
        };

        context.Files.Add(file);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<FileDto>> GetFiles(string projectId, string? email,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await _userRepository.GetUserByEmail(email, cancellationToken);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var projectUsers = await context.ProjectUsers.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken);
        var project = await context.Projects.FirstOrDefaultAsync(x => x.Id.ToString() == projectId, cancellationToken);
        if (project == null)
        {
            throw new Exception("Project not found");
        }

        if (!isAdmin)
        {
            if (projectUsers.All(x => x.ProjectId != project.Id))
            {
                throw new Exception("User not in project");
            }
        }

        var files = await context.Files.Where(x => x.ProjectId == project.Id).OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return files.Select(x => new FileDto
        {
            Name = x.Name,
            CreatedAt = x.CreatedAt,
            Id = x.Id.ToString(),
        });
    }

    public async Task<FileStreamResult> DownloadFile(ulong fileId, string projectId, string? email, bool isAdmin,
        CancellationToken cancellationToken)
    {
        string filePath = "";
        Project localProject = null;
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        if (!isAdmin)
        {
            var user = await _userRepository.GetUserByEmail(email, cancellationToken);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var projectUsers =
                await context.ProjectUsers.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken);
            var project =
                await context.Projects.FirstOrDefaultAsync(x => x.Id.ToString() == projectId, cancellationToken);
            if (project == null)
            {
                throw new Exception("Project not found");
            }

            if (projectUsers.All(x => x.ProjectId != project.Id))
            {
                throw new Exception("User not in project");
            }

            localProject = project;
        }
        else
        {
            localProject =
                await context.Projects.FirstOrDefaultAsync(x => x.Id.ToString() == projectId, cancellationToken);
            if (localProject == null)
            {
                throw new Exception("Project not found");
            }
        }

        var file = await context.Files.FirstOrDefaultAsync(x => x.Id == fileId && x.ProjectId == localProject.Id,
            cancellationToken);
        if (file == null)
        {
            throw new Exception("File not found");
        }

        filePath = file.Path;
        return _fileSystemRepository.DownloadFile(filePath, cancellationToken);
    }

    public async Task DeleteFile(string fileId, string projectId, string? email, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await _userRepository.GetUserByEmail(email, cancellationToken);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var projectUsers = await context.ProjectUsers.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken);
        var project = await context.Projects.FirstOrDefaultAsync(x => x.Id.ToString() == projectId, cancellationToken);
        if (project == null)
        {
            throw new Exception("Project not found");
        }

        if (projectUsers.All(x => x.ProjectId != project.Id))
        {
            throw new Exception("User not in project");
        }

        var file = await context.Files.FirstOrDefaultAsync(x => x.Id.ToString() == fileId && x.ProjectId == project.Id,
            cancellationToken);
        if (file == null)
        {
            throw new Exception("File not found");
        }

        await _fileSystemRepository.DeleteFile(file.Path, cancellationToken);
        context.Files.Remove(file);
        await context.SaveChangesAsync(cancellationToken);
    }
}