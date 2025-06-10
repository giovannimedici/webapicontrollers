using TodoApi.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace TodoApi.Services;

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _userCollection;


    public UserService(MongoDbService mongoDbService, IConfiguration configuration)
    {
        var userCollectionName = configuration["Collections:User"] 
            ?? throw new ArgumentNullException("Collections:User", "O nome da collection de usuários não foi configurado.");
        _userCollection = mongoDbService.GetCollection<User>(userCollectionName);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        ValidateUser(user);

        if (await UserExistsAsync(user.Email))
            throw new InvalidOperationException("Usuário já existe.");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        await _userCollection.InsertOneAsync(user);
        return user;
    }

    private void ValidateUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException("Email é obrigatório.");

        if (!IsValidEmail(user.Email))
            throw new ArgumentException("Email inválido.");

        if (string.IsNullOrWhiteSpace(user.Password))
            throw new ArgumentException("Senha é obrigatória.");
    }

    private async Task<bool> UserExistsAsync(string email)
    {
        var existingUser = await _userCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
        return existingUser != null;
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _userCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            throw new Exception("User or password incorrect");
        }
        return user;
    }

    private bool IsValidEmail(string email)
    {
        return new System.Net.Mail.MailAddress(email).Address == email;
    }
}