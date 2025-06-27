using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace ToDoNote;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _dbContext;

    public ObservableCollection<TaskItem> Tasks { get; } = new ObservableCollection<TaskItem>();

    private string _newTaskTitle = "";
    public string NewTaskTitle
    {
        get => _newTaskTitle;
        set
        {
            _newTaskTitle = value;
            OnPropertyChanged(nameof(NewTaskTitle));
        }
    }

    public ICommand AddTaskCommand => new RelayCommand(AddTask);
    public ICommand DeleteTaskCommand => new RelayCommand<TaskItem>(DeleteTask);

    public MainViewModel()
    {
        // Инициализация контекста базы данных
        _dbContext = new AppDbContext();
        _dbContext.Database.EnsureCreated();
        
        // Загрузка задач из базы
        LoadTasks();
    }

    private async void LoadTasks()
    {
        var tasks = await _dbContext.Tasks.ToListAsync();
        Tasks.Clear();
        foreach (var task in tasks)
        {
            Tasks.Add(task);
        }
    }

    private async void AddTask()
    {
        if (!string.IsNullOrWhiteSpace(NewTaskTitle))
        {
            var newTask = new TaskItem { Title = NewTaskTitle, IsCompleted = false };
            
            // Добавление в базу данных
            _dbContext.Tasks.Add(newTask);
            await _dbContext.SaveChangesAsync();
            
            // Добавление в коллекцию
            Tasks.Add(newTask);
            NewTaskTitle = string.Empty;
        }
    }

    private async void DeleteTask(TaskItem task)
    {
        if (task != null)
        {
            // Удаление из базы данных
            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync();
            
            // Удаление из коллекции
            Tasks.Remove(task);
        }
    }

    // Метод для обновления статуса задачи
    public async Task UpdateTaskAsync(TaskItem task)
    {
        _dbContext.Entry(task).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}