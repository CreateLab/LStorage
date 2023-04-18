using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace LStorageAdmin.Views;

public partial class AddProjectWindow : Window
{
    public string Name { get; private set; }
    
    public AddProjectWindow()
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
        Name = this.FindControl<TextBox>("NameField").Text;
        Close();
    }
}