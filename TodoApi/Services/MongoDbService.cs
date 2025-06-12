using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TodoApi.Services;

public class MongoDbService
{
    private readonly IMongoDatabase _database;
    private readonly IConfiguration _configuration;

    public MongoDbService(IOptions<Models.MongoDbSettings> mongoDBSettings, IConfiguration configuration)
    {
        _configuration = configuration;
        MongoClient client = new MongoClient(_configuration["MongoDB:ConnectionURI"]);
        _database = client.GetDatabase(_configuration["MongoDB:DatabaseName"]);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}