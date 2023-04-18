using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LStorage.Core.Client;
using LStorage.Core.Setting;
using LStorage.Models.Dto;

namespace LStorageAdmin.ViewModels;

public class ProjectInfoVM: ViewModelBase
{
    public string ProjectName { get; set; }
    public ObservableCollection<UserDto> UsersInProject { get; set; }
    
    public ObservableCollection<UserDto> UsersOutOfProject { get; set; }
    
    private ProjectDto _projectDto;
    public ProjectInfoVM(ProjectDto project, List<UserDto> usersInProject, IEnumerable<UserDto> userDtos)
    {
        _projectDto = project;
        ProjectName = project.Name;
        UsersInProject = new ObservableCollection<UserDto>(usersInProject);
        UsersOutOfProject = new ObservableCollection<UserDto>(userDtos.Except(usersInProject));
    }
    
    public async Task AddUserToProject(object email)
    {
        var movedUser = UsersOutOfProject.FirstOrDefault(x => x.Email == email.ToString());
        if (movedUser != null)
        {
            
            var projectClient = new ProjectClient(Setting.ServerUrl);
            await projectClient.AddUserToProject(_projectDto.Id, email.ToString());
            UsersOutOfProject.Remove(movedUser);
            UsersInProject.Add(movedUser);
        }


    }

    public async Task RemoveUserFromProject(object email)
    {
        var movedUser = UsersInProject.FirstOrDefault(x => x.Email == email.ToString());
        if (movedUser != null)
        {
            var projectClient = new ProjectClient(Setting.ServerUrl);
            await projectClient.RemoveUserFromProject(_projectDto.Id, email.ToString());
            UsersInProject.Remove(movedUser);
            UsersOutOfProject.Add(movedUser);
        }
    }
    
    
    
}