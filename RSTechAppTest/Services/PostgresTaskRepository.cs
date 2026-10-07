using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RSTechAppTest.Models;

namespace RSTechAppTest.Services;

public class PostgresTaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public PostgresTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetAllActiveAsync()
    {
        return await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task AddAsync(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        if (string.IsNullOrWhiteSpace(task.Title) || task.Title.Length > 100)
            throw new ArgumentException("Некорректное название задачи.");

        await _context.Tasks.AddAsync(task);
    }

    public async Task UpdateAsync(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);

        // Проверяем, следит ли уже контекст за этим объектом
        var trackedEntity = _context.Tasks.Local.FirstOrDefault(t => t.Id == task.Id);
        if (trackedEntity != null)
        {
            // Если следит — открепляем старый трекер, чтобы избежать конфликта
            _context.Entry(trackedEntity).State = EntityState.Detached;
        }

        // Принудительно сообщаем EF Core, что эту модель нужно обновить в базе
        _context.Tasks.Update(task);
        await Task.CompletedTask;
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
