using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace LStorageAdmin.Views;

public partial class AddUserWindow : Window
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string Role { get; private set; }
    public AddUserWindow()
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

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {

        Email = this.FindControl<TextBox>("EmailField").Text;
        Name = this.FindControl<TextBox>("NameField").Text;
        Password = this.FindControl<TextBox>("PasswordField").Text;
        Role = this.FindControl<TextBox>("RoleField").Text;
        Close();
    }
}