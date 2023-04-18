using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LStorage.Models.Auth;
using LStorage.Models.Dto;

namespace LStorageAdmin.Views;

public interface IUserWindowApi
{
    Task<SingUpData> ShowRegisterWindow();

    Task<ProjectDto> ShowProjectWindow();
    Task ShowProjectInfoWindow(ProjectDto project, List<UserDto> usersInProject, IEnumerable<UserDto> users);
    Task ShowUserInfoWindow(UserDto user);
}