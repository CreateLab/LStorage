namespace LStorage.Models.Works;

public class File
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
}