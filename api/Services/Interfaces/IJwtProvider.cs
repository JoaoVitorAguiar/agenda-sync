namespace AgendaSync.Services.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(string userId, string email);
}