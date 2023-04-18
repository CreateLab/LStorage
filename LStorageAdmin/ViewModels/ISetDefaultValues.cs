using System.Collections.Generic;
using LStorage.Models.Dto;

namespace LStorageAdmin.ViewModels;

public interface ISetDefaultValues
{
    public void SetUsers(IEnumerable<UserDto> users);
    public void SetProjects(IEnumerable<ProjectDto> projects);
}