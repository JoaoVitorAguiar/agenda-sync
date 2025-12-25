using AgendaSync.Entities;

namespace AgendaSync.Services.Interfaces;

public interface IUserRepository
{
    Task CreateUserAsync(User user);
    Task<User?> GetUserByExternalSubjectAsync(string externalSubject);
    Task<User?> GetByIdAsync(Guid id);
    Task UpdateUserAsync(User user);
}