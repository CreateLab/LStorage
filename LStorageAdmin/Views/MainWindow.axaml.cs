using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using LStorage.Core.Client;
using LStorage.Core.Setting;
using LStorageAdmin.ViewModels;

namespace LStorageAdmin.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async Task CheckToken()
    {
        var o = new object();
        var s = o.ToString();
        SettingLoader.SetUp();
        if (Setting.ServerUrl != null)
        {
            var tokenClient = new TokenClient(Setting.ServerUrl);
            await tokenClient.RefreshToken();
            if (Setting.Token == null)
            {
                var dc = (IShowRegisterWindow)DataContext;
                dc.IsShowRegisterWindow = true;
            }
            else
            {
                var adminWindow = new AdminWindow();
                adminWindow.DataContext = new AdminViewModel(adminWindow);
                adminWindow.Show();
                this.Close();
            }
        }
        else
        {
            var dc = (IShowRegisterWindow)DataContext;
            dc.IsShowRegisterWindow = true;
        }
    }


    private async void TopLevel_OnOpened(object? sender, EventArgs e)
    {
        await CheckToken();
    }
}