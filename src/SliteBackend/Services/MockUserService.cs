using SliteBackend.Models;

namespace SliteBackend.Services;

public class MockUserService : IUserService
{
    private static readonly List<User> _users = new()
    {
        new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
        new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
    };

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        await Task.Delay(10);
        return _users;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        await Task.Delay(10);
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        await Task.Delay(10);
        return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await Task.Delay(10);
        user.Id = _users.Count + 1;
        _users.Add(user);
        return user;
    }

    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        await Task.Delay(10);
        var existingUser = _users.FirstOrDefault(u => u.Id == id);
        if (existingUser != null)
        {
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
        }
        return existingUser;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        await Task.Delay(10);
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            _users.Remove(user);
            return true;
        }
        return false;
    }

    public async Task<bool> UserExistsAsync(int id)
    {
        await Task.Delay(10);
        return _users.Any(u => u.Id == id);
    }
}