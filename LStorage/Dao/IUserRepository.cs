using LStorage.Models.Auth;
using LStorage.Models.Dto;

namespace LStorage.Dao;

public interface IUserRepository
{
    public Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken);
    public Task SaveUser(User user, CancellationToken cancellationToken);
    Task DeleteUser(User user);
    Task<IEnumerable<UserDto>> GetUsers(CancellationToken cancellationToken);
    Task UpdateUser(User user, CancellationToken token);
}