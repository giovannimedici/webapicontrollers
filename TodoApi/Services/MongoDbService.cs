using TodoApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace TodoApi.Services;

public class MongoDbService : IMongoDbService
{
    private readonly IMongoCollection<TodoItem> _todoItems;
    
    public MongoDbService(IOptions<MongoDbSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _todoItems = database.GetCollection<TodoItem>(mongoDBSettings.Value.CollectionName);
    }

    public virtual async Task<List<TodoItem>> GetAllItensAsync() => await _todoItems.Find(_ => true).ToListAsync();
    public virtual async Task<TodoItem> GetItemById(string id) => await _todoItems.Find(xs => xs.Id == id).FirstOrDefaultAsync();
    public virtual async Task CreateAsync(TodoItem todoItem) => await _todoItems.InsertOneAsync(todoItem);
    public virtual async Task UpdateTodoItemAsync(string id, TodoItem updatedTodoItem) => await _todoItems.ReplaceOneAsync(x => x.Id == id, updatedTodoItem);
    public virtual async Task DeleteAsync(string id) => await _todoItems.DeleteOneAsync(xs => xs.Id == id);
}