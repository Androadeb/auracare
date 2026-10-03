using ynex.Models.Sessions;

namespace ynex.Services.Sessions
{
    public interface IDiagnosticSessionService
    {
        Task<SessionResponse?> GetSessionsAsync(int page = 1, int limit = 10);
        Task<SessionData?> GetSessionByIdAsync(string id);
    }
}