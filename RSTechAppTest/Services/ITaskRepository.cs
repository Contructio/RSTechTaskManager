using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RSTechAppTest.Models;

namespace RSTechAppTest.Services;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllActiveAsync();
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task SaveChangesAsync();
}
