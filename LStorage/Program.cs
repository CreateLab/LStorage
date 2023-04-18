using LStorage.Auth;
using LStorage.Dao;
using LStorage.DB;
using LStorage.Extension;
using LStorage.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

/*const string adminEmail = "B.sorokin@linizvrn.com";
const string password = "suDiUmvE";*/


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // укзывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,

            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,

            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite("Data Source=helloapp.db"));

builder.Services.AddTransient<IFileRepository, FileRepository>();
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IFileSystemRepository, FileSystemRepository>();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.WebHost.UseUrls("https://localhost:7067;http://localhost:5126");
var app = builder.Build();

#region setupAdmin

var factory = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
using (var context = factory.CreateDbContext())
{
    if (File.Exists("./Password.txt"))
    {
        context.Database.EnsureCreated();
        var lines = File.ReadLines("./Password.txt").ToArray();

        if (lines.Count() >= 2)
        {
            var adminEmail = lines[0].Trim();
            var password = lines[0].Trim();
            var admin = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
            if (admin == null)
            {
                var newUser = new User
                {
                    Email = adminEmail,
                    Name = "Boris",
                    Password = password.Hash(),
                    Role = "admin",
                    CreatedAt = DateTime.Now
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();
            }
        }
    }
}

#endregion


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();