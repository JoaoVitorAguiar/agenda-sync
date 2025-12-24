using AgendaSync.Entities;

namespace AgendaSync.Services;

public interface IUserRepository
{
    Task CreateUserAsync(User user);
    Task<User?> GetUserByExternalSubjectAsync(string externalSubject);

    Task UpdateUserAsync(User user);
}