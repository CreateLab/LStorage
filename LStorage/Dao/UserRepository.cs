using LStorage.DB;
using LStorage.Models.Auth;
using LStorage.Models.Dto;
using LStorage.Models.Links;
using LStorage.Models.Works;
using Microsoft.EntityFrameworkCore;

namespace LStorage.Dao;

public class UserRepository : IUserRepository
{
    private static readonly SemaphoreSlim _userLock = new SemaphoreSlim(1, 1);
    private readonly IDbContextFactory<AppDbContext> _contextFactory;


    public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveUser(User user, CancellationToken cancellationToken)
    {
        await _userLock.WaitAsync();
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _userLock.Release();
        }
    }

    public async Task DeleteUser(User user)
    {
        await _userLock.WaitAsync();
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
        finally
        {
            _userLock.Release();
        }
    }

    public async Task<IEnumerable<UserDto>> GetUsers(CancellationToken cancellationToken)
    {
        await _userLock.WaitAsync();
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var usersProjects = await context.ProjectUsers.ToListAsync(cancellationToken);
            var projects = await context.Projects.ToListAsync(cancellationToken);
            var users = await context.Users.ToListAsync(cancellationToken);
            return users.Select(u => new UserDto
            {
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                Projects = CreateProjectDtos(u, usersProjects, projects)
            });
        }
        finally
        {
            _userLock.Release();
        }
    }

    public async Task UpdateUser(User user, CancellationToken token)
    {
        await _userLock.WaitAsync(token);
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(token);
            context.Users.Update(user);
            await context.SaveChangesAsync(token);
        }
        finally
        {
            _userLock.Release();
        }
    }

    private IEnumerable<ProjectDto> CreateProjectDtos(User user, List<ProjectUsers> usersProjects,
        List<Project?> projects)
    {
        var userProjects = usersProjects.Where(up => up.UserId == user.Id).ToList();
        var userProjectIds = userProjects.Select(up => up.ProjectId).ToList();
        var userProjectsList = projects.Where(p => userProjectIds.Contains(p.Id)).ToList();
        return userProjectsList.Select(p => new ProjectDto
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            CreatedAt = p.CreatedAt,
        });
    }
}