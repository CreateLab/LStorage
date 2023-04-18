using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LStorage.Core.Client;
using LStorage.Core.Setting;
using LStorage.Models.Auth;
using LStorage.Models.Dto;
using LStorageAdmin.ViewModels;

namespace LStorageAdmin.Views;

public partial class AdminWindow : Window, IUserWindowApi
{
    public AdminWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public async Task<SingUpData> ShowRegisterWindow()
    {
        var addUserWindow = new AddUserWindow();
        await addUserWindow.ShowDialog(this);

        return new SingUpData
        {
            Name = addUserWindow.Name,
            Email = addUserWindow.Email,
            Password = addUserWindow.Password,
            Role = addUserWindow.Role
        };
    }

    public async Task<ProjectDto> ShowProjectWindow()
    {
        var addProjectWindow = new AddProjectWindow();
        await addProjectWindow.ShowDialog(this);
        
        return new ProjectDto
        {
            Name = addProjectWindow.Name
        };
    }

    public async Task ShowProjectInfoWindow(ProjectDto project, List<UserDto> usersInProject,
        IEnumerable<UserDto> users)
    {
        var projectInfoWindow = new ProjectInfoWindow();
        projectInfoWindow.DataContext = new ProjectInfoVM(project, usersInProject, users);
        await projectInfoWindow.ShowDialog(this);
    }

    public Task ShowUserInfoWindow(UserDto user)
    {
        var w = new UserInfoWindow();
        w.DataContext = new UserInfoVM(user);
        return w.ShowDialog(this);
    }


    private async void TopLevel_OnOpened(object? sender, EventArgs e)
    {
        var rc = (ISetDefaultValues)DataContext;
        var uc = (IShowRegisterWindow)DataContext;
        uc.IsShowRegisterWindow = true;
        var UserClient = new UserClient(Setting.ServerUrl);
        var users = await UserClient.GetUsers();
        var projectClient = new ProjectClient(Setting.ServerUrl);
        var projects = await projectClient.GetAllProjects();
        rc.SetUsers(users);
        rc.SetProjects(projects);
        uc.IsShowRegisterWindow = false;
    }
}