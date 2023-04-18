using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LStorageAdmin.Views;

public partial class UserInfoWindow : Window
{
    public UserInfoWindow()
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
}