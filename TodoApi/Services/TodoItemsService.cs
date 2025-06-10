using TodoApi.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Services
{
    public class TodoItemsService : ITodoItemsService
    {
        private readonly IMongoCollection<TodoItem> _todoItems;

        public TodoItemsService(MongoDbService mongoDbService, IConfiguration configuration)
        {
            var todoItemsCollectionName = configuration["Collections:TodoList"] 
                ?? throw new ArgumentNullException("Collections:TodoList", "O nome da collection de itens não foi configurado.");
            _todoItems = mongoDbService.GetCollection<TodoItem>(todoItemsCollectionName);
        }

        public async Task<List<TodoItem>> GetAllItensAsync() => await _todoItems.Find(_ => true).ToListAsync();
        public async Task<TodoItem?> GetItemById(string id) => await _todoItems.Find(xs => xs.Id == id).FirstOrDefaultAsync();
        public async Task CreateAsync(TodoItem todoItem) => await _todoItems.InsertOneAsync(todoItem);
        public async Task UpdateTodoItemAsync(string id, TodoItem updatedTodoItem) => await _todoItems.ReplaceOneAsync(x => x.Id == id, updatedTodoItem);
        public async Task DeleteAsync(string id) => await _todoItems.DeleteOneAsync(xs => xs.Id == id);
    }
} 