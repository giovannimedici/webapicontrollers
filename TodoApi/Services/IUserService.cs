using TodoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApi.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(User user);
    Task<User> LoginAsync(string email, string password);
}