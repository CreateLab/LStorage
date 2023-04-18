using LStorage.Models;
using LStorage.Models.Auth;
using LStorage.Models.Links;
using LStorage.Models.Works;
using Microsoft.EntityFrameworkCore;
using File = System.IO.File;

namespace LStorage.DB;

public class AppDbContext : DbContext
{
    public DbSet<User?> Users {get;set; } = null!;
    public DbSet<Project?> Projects {get;set; } = null!;
    public DbSet<Models.Works.File> Files {get;set; } = null!;
    public DbSet<ProjectUsers> ProjectUsers {get;set; } = null!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
}