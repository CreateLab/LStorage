using LStorage.Models.Works;

namespace LStorage.Models.Dto;

public class UserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public IEnumerable<ProjectDto> Projects { get; set; }
}