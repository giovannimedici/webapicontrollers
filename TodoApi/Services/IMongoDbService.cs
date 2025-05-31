using TodoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Services
{
    public interface IMongoDbService
    {
        Task<List<TodoItem>> GetAllItensAsync();
        Task<TodoItem?> GetItemById(string id);
        Task CreateAsync(TodoItem todoItem);
        Task UpdateTodoItemAsync(string id, TodoItem updatedTodoItem);
        Task DeleteAsync(string id);
    }
} 