using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RSTechAppTest.Models;
using RSTechAppTest.Services;
using RSTechAppTest.ViewModels;
using Xunit;

namespace RSTechAppTest.Tests;

public class TaskManagerTests
{
    // Вспомогательный метод для создания чистой изолированной БД в оперативной памяти для каждого теста
    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя базы для изоляции тестов
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ValidTask_ShouldBeSavedInDatabase()
    {
        // Arrange (Настройка)
        using var context = CreateInMemoryDbContext();
        var repository = new PostgresTaskRepository(context);
        var task = new TaskItem { Id = Guid.NewGuid(), Title = "Выпить кофе", CreatedAt = DateTime.UtcNow };

        // Act (Действие)
        await repository.AddAsync(task);
        await repository.SaveChangesAsync();

        // Assert (Проверка)
        var activeTasks = await repository.GetAllActiveAsync();
        Assert.Single(activeTasks);
        Assert.Equal("Выпить кофе", activeTasks.First().Title);
    }

    [Fact]
    public async Task AddAsync_EmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var repository = new PostgresTaskRepository(context);
        var invalidTask = new TaskItem { Id = Guid.NewGuid(), Title = "   ", CreatedAt = DateTime.UtcNow };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => repository.AddAsync(invalidTask));
    }

    [Fact]
    public async Task AddAsync_TitleTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var repository = new PostgresTaskRepository(context);
        var longTitle = new string('A', 101); // 101 символ (граница валидации по ТЗ — 100)
        var invalidTask = new TaskItem { Id = Guid.NewGuid(), Title = longTitle, CreatedAt = DateTime.UtcNow };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => repository.AddAsync(invalidTask));
    }

    [Fact]
    public async Task GetAllActiveAsync_SoftDeletedTask_ShouldNotBeReturned()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var repository = new PostgresTaskRepository(context);

        var activeTask = new TaskItem { Id = Guid.NewGuid(), Title = "Активная задача", IsDeleted = false };
        var deletedTask = new TaskItem { Id = Guid.NewGuid(), Title = "Удаленная задача", IsDeleted = true };

        // Добавляем напрямую через контекст, минуя валидацию репозитория для проверки фильтра
        await context.Tasks.AddRangeAsync(activeTask, deletedTask);
        await context.SaveChangesAsync();

        // Act
        var result = (await repository.GetAllActiveAsync()).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Активная задача", result.First().Title);
        Assert.DoesNotContain(result, t => t.Title == "Удаленная задача");
    }

    [Fact]
    public async Task MainViewModel_AddTask_WithEmptyInput_ShouldSetErrorMessage()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var repository = new PostgresTaskRepository(context);
        var viewModel = new MainViewModel(repository, NullLogger<MainViewModel>.Instance);

        viewModel.NewTaskTitle = ""; // Имитируем пустой ввод пользователя в TextBox

        // Act
        await viewModel.AddTaskCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.Equal("Название задачи не может быть пустым!", viewModel.ErrorMessage);
        Assert.Empty(viewModel.Tasks); // Таблица на экране должна остаться пустой
    }
}
