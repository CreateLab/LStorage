using System.Collections.Generic;
using System.Threading.Tasks;
using LStorage.Models.Dto;

namespace LStorageClient.ViewModels;

public interface ISetDefaultValues
{
    void SetProjects(IEnumerable<ProjectDto> projects);
    void SetSpace(int freeSpace, int totalSpace);
    Task UploadFile(string file);
}