namespace LStorage.Models.Dto;

public class UserProjectDto
{
    public string ProjectId { get; set; }
    public IEnumerable<string> UsersEmails { get; set; }
}