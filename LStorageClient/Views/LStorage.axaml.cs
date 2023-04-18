using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using LStorage.Core.Client;
using LStorage.Core.Setting;
using LStorageClient.ViewModels;

namespace LStorageClient.Views;

public partial class LStorage : Window, IUserWindowApi
{
    public LStorage()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        var ddControl = this.FindControl<Border>("DragDropControl");
        ddControl.AddHandler(DragDrop.DropEvent, DragEnter);
    }

    public async void DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.FileNames))
        {
            e.DragEffects = DragDropEffects.Copy;
            e.Handled = true;
            var file = e.Data.GetFileNames().FirstOrDefault();
            if (file != null)
            {
                var rc = (ISetDefaultValues)DataContext;
                var uc = (IShowRegisterWindow)DataContext;
                uc.IsShowRegisterWindow = true;
                await rc.UploadFile(file);
                uc.IsShowRegisterWindow = false;
            }
        }
    }

    private async void TopLevel_OnOpened(object? sender, EventArgs e)
    {
        var rc = (ISetDefaultValues)DataContext;
        var uc = (IShowRegisterWindow)DataContext;
        uc.IsShowRegisterWindow = true;
        var projectClient = new ProjectClient(Setting.ServerUrl);
        var infoClient = new InfoClient(Setting.ServerUrl);
        var freeSpace = await infoClient.GetFreeSpace();
        var totalSpace = await infoClient.GetTotalSpace();
        var projects = await projectClient.GetProjects();
        rc.SetProjects(projects);
        rc.SetSpace(freeSpace, totalSpace);
        uc.IsShowRegisterWindow = false;
    }

    public async Task<string> GetFilePath()
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
        var result = await openFileDialog.ShowAsync(this);
        return result?[0];
    }

    public Task<string> GetFolderPath(string? fileName)
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.InitialFileName = fileName;
        saveFileDialog.Filters.Add(new FileDialogFilter() { Name = "All", Extensions = { "*" } });
        return saveFileDialog.ShowAsync(this);
    }
}