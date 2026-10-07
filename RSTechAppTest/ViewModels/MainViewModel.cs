using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RSTechAppTest.Models;
using RSTechAppTest.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RSTechAppTest.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<MainViewModel> _logger;

    [ObservableProperty] private string _newTaskTitle = string.Empty;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _hasError;

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    public MainViewModel(ITaskRepository repository, ILogger<MainViewModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [RelayCommand]
    public async Task LoadTasksAsync()
    {
        try
        {
            ResetError();
            var items = await _repository.GetAllActiveAsync();
            Tasks.Clear();
            foreach (var item in items)
            {
                Tasks.Add(item);
            }
        }
        catch (Exception ex)
        {
            ShowError("Ошибка при загрузке задач из базы данных.");
            _logger.LogError(ex, "Failed to load tasks");
        }
    }

    [RelayCommand]
    public async Task AddTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
        {
            ShowError("Название задачи не может быть пустым!");
            return;
        }

        if (NewTaskTitle.Length > 100)
        {
            ShowError("Название задачи не должно превышать 100 символов.");
            return;
        }

        try
        {
            ResetError();
            var newTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = NewTaskTitle.Trim(),
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newTask);
            await _repository.SaveChangesAsync();

            Tasks.Insert(0, newTask);
            NewTaskTitle = string.Empty;
        }
        catch (Exception ex)
        {
            ShowError("Не удалось сохранить задачу.");
            _logger.LogError(ex, "Error while adding task");
        }
    }
    [RelayCommand]
    public async Task ToggleCompletionAsync(TaskItem task)
    {
        if (task == null) return;
        try
        {
            ResetError();

            // Создаем чистый изолированный scope для транзакции обновления
            using (var scope = Program.ServiceProvider.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
                await repo.UpdateAsync(task);
                await repo.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            ShowError("Ошибка при обновлении статуса задачи.");
            _logger.LogError(ex, "Error toggling task completion");
        }
    }

    [RelayCommand]
    public async Task DeleteTaskAsync(TaskItem task)
    {
        if (task == null) return;
        try
        {
            ResetError();
            task.IsDeleted = true; 

            // Создаем чистый изолированный scope для транзакции удаления
            using (var scope = Program.ServiceProvider.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
                await repo.UpdateAsync(task);
                await repo.SaveChangesAsync();
            }

            Tasks.Remove(task); 
        }
        catch (Exception ex)
        {
            ShowError("Ошибка при удалении задачи.");
            _logger.LogError(ex, "Error deleting task");
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void ResetError()
    {
        ErrorMessage = null;
        HasError = false;
    }
}
