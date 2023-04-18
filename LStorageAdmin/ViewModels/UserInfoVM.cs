using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LStorage.Core.Client;
using LStorage.Core.Setting;
using LStorage.Models.Dto;

namespace LStorageAdmin.ViewModels;

public class UserInfoVM: ViewModelBase
{
    public string Email { get; set; }
    private UserDto _userDto;
    public ObservableCollection<ProjectDto> Projects { get; set; } = new();
    
    public UserInfoVM(UserDto userDto)
    {
        Email = userDto.Email;
        _userDto = userDto;
        Projects = new ObservableCollection<ProjectDto>(userDto.Projects);
    }
    
    public async Task RemoveUserFromProject(object projectId)
    {
        var projectClient = new ProjectClient(Setting.ServerUrl);
        await projectClient.RemoveUserFromProject(projectId.ToString(), Email);
        var project = Projects.FirstOrDefault(x => x.Id == projectId.ToString());
        if (project != null)
        {
            Projects.Remove(project);
        }
    }
}