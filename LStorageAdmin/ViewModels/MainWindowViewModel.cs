using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using LStorage.Core.Client;
using LStorage.Core.Setting;
using LStorageAdmin.Views;
using ReactiveUI;

namespace LStorageAdmin.ViewModels;

public class MainWindowViewModel : ViewModelBase, IShowRegisterWindow
{
    private bool _isShowRegisterWindow;
    public bool _isNotOk;
    
    private string _email;
    private string _password;
    private string _serverUrl;

    public string Name => "ООО \"Линейные изыскания\"";
    public bool IsShowRegisterWindow
    {
        get => _isShowRegisterWindow;
        set => this.RaiseAndSetIfChanged(ref _isShowRegisterWindow, value);
    }

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string ServerUrl
    {
        get => _serverUrl;
        set => this.RaiseAndSetIfChanged(ref _serverUrl, (string)value);
    }

    public bool IsNotOk
    {
        get => _isNotOk;
        set => this.RaiseAndSetIfChanged(ref _isNotOk, value);
    }
    

    public async Task TryLogin()
    {
        IsShowRegisterWindow = false;
        if (!string.IsNullOrEmpty(_serverUrl) && !string.IsNullOrEmpty(_email) && !string.IsNullOrEmpty(_password))
        {
            
            var tokenClient = new TokenClient(_serverUrl);
            //await Task.Run(async () => await tokenClient.CreateToken(_email, _password));
            await tokenClient.CreateToken(_email, _password);
            if (Setting.Token != null)
            {
                #region swapWindow

                var adminWindow = new AdminWindow();
                adminWindow.DataContext = new AdminViewModel(adminWindow);
                adminWindow.Show();
                var classicDesktopStyleApplicationLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
                classicDesktopStyleApplicationLifetime?.MainWindow.Close();

                #endregion
               
            }
            else
            {
                IsNotOk = true;
            }
        }
        else
        {
            IsNotOk = true;
        }
        IsShowRegisterWindow = true;
        if(Setting.Token == null)
            IsNotOk = true;
    }
}