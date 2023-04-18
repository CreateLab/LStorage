using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Flurl.Http;
using LStorage.Core.Client;
using LStorage.Core.Setting;
using LStorageClient.ViewModels;

namespace LStorageClient.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async Task CheckToken()
    {
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
                var lStorage = new LStorage();
                lStorage.DataContext = new LStorageVM(lStorage);
                lStorage.Show();
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