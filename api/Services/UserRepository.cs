using AgendaSync.Data;
using AgendaSync.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendaSync.Services;

public class UserRepository(AgendaSyncDbContext context) : IUserRepository
{
    public async Task CreateUserAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByExternalSubjectAsync(string externalSubject)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.ExternalSubject == externalSubject);
    }

    public async Task UpdateUserAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
}