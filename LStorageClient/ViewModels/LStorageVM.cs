using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DynamicData;
using LStorage.Core.Client;
using LStorage.Core.Setting;
using LStorage.Models.Dto;
using LStorageClient.Views;
using ReactiveUI;

namespace LStorageClient.ViewModels;

public class LStorageVM : ViewModelBase, ISetDefaultValues, IShowRegisterWindow
{
    private string _projectName;
    private ProjectDto? _selectedProject;
    private bool _isShowRegisterWindow;
    private IUserWindowApi _userWindowApi;
    private bool isFileUploading;
    private string _space;
    private const string _formatSpace = "Свободно {0} из {1} ГБ";
    private bool isProjectSelected;
    
    public bool IsProjectSelected
    {
        get => isProjectSelected;
        set => this.RaiseAndSetIfChanged(ref isProjectSelected, value);
    }
    public string Space
    {
        get => _space;
        set => this.RaiseAndSetIfChanged(ref _space, value);
    }

    public bool IsFileUploading
    {
        get => isFileUploading;
        set => this.RaiseAndSetIfChanged(ref isFileUploading, value);
    }

    public bool IsShowRegisterWindow
    {
        get => _isShowRegisterWindow;
        set => this.RaiseAndSetIfChanged(ref _isShowRegisterWindow, value);
    }

    public string ProjectName
    {
        get => _projectName;
        set => this.RaiseAndSetIfChanged(ref _projectName, value);
    }

    public ProjectDto? SelectedProject
    {
        get => _selectedProject;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedProject, value);
            if (value != null)
            {
                ProjectName = value.Name;
                GetFiles().ContinueWith(t =>
                {
                    if (t.IsFaulted) Console.Error.WriteLine(t.Exception);
                }, TaskContinuationOptions.ExecuteSynchronously);
                IsProjectSelected = true;
            }
        }
    }

    private async Task GetFiles()
    {
        var fileClient = new FileClient(Setting.ServerUrl);
        var files = await fileClient.GetFiles(SelectedProject.Id);
        Files.Clear();
        Files.AddRange(files);
    }


    public ObservableCollection<ProjectDto> Projects { get; set; } = new();
    public ObservableCollection<FileDto> Files { get; set; } = new();


    public LStorageVM(IUserWindowApi userWindowApi)
    {
        _userWindowApi = userWindowApi;
    }

    public void SetProjects(IEnumerable<ProjectDto> projects)
    {
        Projects.Clear();
        Projects.AddRange(projects);
    }

    public void SetSpace(int freeSpace, int totalSpace)
    {
        Space = string.Format(_formatSpace, freeSpace, totalSpace);
    }

    public async Task UploadFile(string file)
    {
        if (SelectedProject == null)
            return;
        IsFileUploading = true;
        if (!string.IsNullOrEmpty(file))
        {
            var fileName = Path.GetFileName(file);
            await using var fileStream = File.OpenRead(file);
            var fileClient = new FileClient(Setting.ServerUrl);
            await fileClient.UploadFile(fileName, fileStream, SelectedProject.Id);
            Files.Add(new FileDto
            {
                Name = fileName,
            });
        }

        IsFileUploading = false;
    }

    public async Task GetAllFiles()
    {
        var fileClient = new FileClient(Setting.ServerUrl);
        var files = await fileClient.GetFiles(SelectedProject.Id);
        Files.Clear();
        Files.AddRange(files);
    }

    public async Task UploadFile()
    {
        if (SelectedProject == null)
            return;
        IsFileUploading = true;
        var filePath = await _userWindowApi.GetFilePath();
        if (!string.IsNullOrEmpty(filePath))
        {
            var fileName = Path.GetFileName(filePath);
            await using var fileStream = File.OpenRead(filePath);
            var fileClient = new FileClient(Setting.ServerUrl);
            await fileClient.UploadFile(fileName, fileStream, SelectedProject.Id);
            Files.Add(new FileDto
            {
                Name = fileName,
            });
            var files = await fileClient.GetFiles(SelectedProject.Id);
            Files.Clear();
            Files.AddRange(files);
        }

        IsFileUploading = false;
    }

    public async Task DownloadFile(string fileID)
    {
        if (SelectedProject == null || string.IsNullOrEmpty(fileID))
            return;
        var fileName = Files.FirstOrDefault(f => f.Id == fileID)?.Name;
        var filePath = await _userWindowApi.GetFolderPath(fileName);
        
        if (!string.IsNullOrEmpty(filePath))
        {
            var fileClient = new FileClient(Setting.ServerUrl);
            await fileClient.DownloadFile(fileID, filePath, SelectedProject.Id);
        }
    }
}