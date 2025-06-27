using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ToDoNote;

public partial class TitleBar : UserControl
{
    public TitleBar()
    {
        InitializeComponent();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Window.GetWindow(this)?.DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this).WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this).Close();
    }

    
    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        App.Settings.IsDarkTheme = !App.Settings.IsDarkTheme;
        
        Application.Current.Resources.MergedDictionaries.Clear();

        
        var newTheme = App.Settings.IsDarkTheme
            ? new Uri("Themes/LightTheme.xaml", UriKind.Relative)
            : new Uri("Themes/DarkTheme.xaml", UriKind.Relative);

        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = newTheme });
    }
}
