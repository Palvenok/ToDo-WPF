using System.Windows;

namespace ToDoNote;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();

        this.Loaded += MainWindow_Loaded;

        if (DataContext is MainViewModel vm)
        {
            vm.Tasks.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (TaskItem task in e.NewItems)
                    {
                        task.PropertyChanged += async (sender, args) =>
                        {
                            if (args.PropertyName == nameof(TaskItem.IsCompleted))
                            {
                                await vm.UpdateTaskAsync((TaskItem)sender);
                            }
                        };
                    }
                }
            };

            // Подпишемся на уже загруженные задачи
            foreach (var task in vm.Tasks)
            {
                task.PropertyChanged += async (sender, args) =>
                {
                    if (args.PropertyName == nameof(TaskItem.IsCompleted))
                    {
                        await vm.UpdateTaskAsync((TaskItem)sender);
                    }
                };
            }
        }

    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        TaskInputTextBox.Focus();
    }


    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Calculate the position based on the screen's work area
        Left = SystemParameters.WorkArea.Right - ActualWidth - 20;
        Top = SystemParameters.WorkArea.Top + 20;

    }
}