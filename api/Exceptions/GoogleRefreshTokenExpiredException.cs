namespace AgendaSync.Exceptions;

public class GoogleRefreshTokenExpiredException : Exception
{
    public GoogleRefreshTokenExpiredException(string message)
        : base(message) { }
}
