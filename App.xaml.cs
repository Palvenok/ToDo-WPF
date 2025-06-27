using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace ToDoNote;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static AppSettings Settings { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Загрузка настроек
        Settings = AppSettings.Load();

        var currentTheme = Application.Current.Resources.MergedDictionaries.FirstOrDefault();
        Application.Current.Resources.MergedDictionaries.Clear();

        var newTheme = Settings.IsDarkTheme
            ? new Uri("Themes/LightTheme.xaml", UriKind.Relative)
            : new Uri("Themes/DarkTheme.xaml", UriKind.Relative);

        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = newTheme });
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Сохранение настроек при выходе
        Settings.Save();
        base.OnExit(e);
    }
}

