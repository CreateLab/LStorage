using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using LStorage.Core.Client;
using LStorage.Core.Setting;
using LStorage.Models.Dto;
using LStorageAdmin.Views;
using ReactiveUI;

namespace LStorageAdmin.ViewModels;

public class AdminViewModel : ViewModelBase, IShowRegisterWindow, ISetDefaultValues
{
    private bool _isShowRegisterWindow;
    private readonly IUserWindowApi _userWindowApi;
    public ObservableCollection<UserDto> Users { get; set; }
    public ObservableCollection<ProjectDto> Projects { get; set; }

    public bool IsShowRegisterWindow
    {
        get => _isShowRegisterWindow;
        set => this.RaiseAndSetIfChanged(ref _isShowRegisterWindow, value);
    }

    public AdminViewModel(IUserWindowApi userWindowApi)
    {
        Users = new ObservableCollection<UserDto>();
        Projects = new ObservableCollection<ProjectDto>();
        _userWindowApi = userWindowApi;
    }

    public async Task UpdateWindow()
    {
        IsShowRegisterWindow = true;
        var userClient = new UserClient(Setting.ServerUrl);
        var users = await userClient.GetUsers();
        Users.Clear();
        Users.AddRange(users);
        IsShowRegisterWindow = false;
    }

    public async Task AddUser()
    {
        IsShowRegisterWindow = true;
        var data = await _userWindowApi.ShowRegisterWindow();
        var userClient = new UserClient(Setting.ServerUrl);
        if (!string.IsNullOrEmpty(data.Email) && !string.IsNullOrEmpty(data.Name) &&
            !string.IsNullOrEmpty(data.Password) && !string.IsNullOrEmpty(data.Role))
        {
            await userClient.CreateUser(data);
            var users = await userClient.GetUsers();
            Users.Clear();
            Users.AddRange(users);
        }

        IsShowRegisterWindow = false;
    }

    public async Task AddProject()
    {
        IsShowRegisterWindow = true;
        var data = await _userWindowApi.ShowProjectWindow();
        var projectClient = new ProjectClient(Setting.ServerUrl);
        if (!string.IsNullOrEmpty(data.Name))
        {
            await projectClient.CreateProject(data);
            var projects = await projectClient.GetAllProjects();
            Projects.Clear();
            Projects.AddRange(projects);
        }

        IsShowRegisterWindow = false;
    }

    public async Task DeleteUser(object userEmail)
    {
        IsShowRegisterWindow = true;
        var user = Users.FirstOrDefault(x => x.Email == userEmail.ToString());
        if (user == null)
        {
            IsShowRegisterWindow = false;
            return;
        }
        var userClient = new UserClient(Setting.ServerUrl);
        await userClient.DeleteUser(user.Email);
        var users = await userClient.GetUsers();
        Users.Clear();
        Users.AddRange(users);
        IsShowRegisterWindow = false;
    }

    public void SetUsers(IEnumerable<UserDto> users)
    {
        Users.Clear();
        Users.AddRange(users);
    }

    public void SetProjects(IEnumerable<ProjectDto> projects)
    {
        Projects.Clear();
        Projects.AddRange(projects);
    }

    public async Task ShowUserInfo(object email)
    {
        IsShowRegisterWindow = true;
        var user = Users.FirstOrDefault(x => x.Email == email.ToString());
        if (user != null)
        {
            await _userWindowApi.ShowUserInfoWindow(user);
            var userClient = new UserClient(Setting.ServerUrl);
            var users = await userClient.GetUsers();
            Users.Clear();
            Users.AddRange(users);
        }

        IsShowRegisterWindow = false;
    }

    public async Task ShowProjectInfoWindow(object projectId)
    {
        IsShowRegisterWindow = true;
        var userClient = new UserClient(Setting.ServerUrl);
        var users = await userClient.GetUsers();
        var projectClient = new ProjectClient(Setting.ServerUrl);
        var projects = await projectClient.GetAllProjects();
        Users.Clear();
        Users.AddRange(users);
        Projects.Clear();
        Projects.AddRange(projects);
        var project = Projects.FirstOrDefault(x => x.Id == projectId.ToString());
        if (project != null)
        {
            var usersInProject = Users.Where(x => x.Projects.Select(p => p.Id).Contains(project.Id)).ToList();
            await _userWindowApi.ShowProjectInfoWindow(project, usersInProject, Users.AsEnumerable());
            users = await userClient.GetUsers();
            Users.Clear();
            Users.AddRange(users);
        }

        IsShowRegisterWindow = false;
    }
}