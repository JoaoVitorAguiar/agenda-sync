namespace AgendaSync.Services;

public interface IJwtProvider
{
    string GenerateToken(string userId, string email);
}