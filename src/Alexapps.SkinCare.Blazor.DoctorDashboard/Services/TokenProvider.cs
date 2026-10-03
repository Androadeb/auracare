namespace ynex.Services
{
    public interface ITokenProvider
    {
        string? AccessToken { get; set; }
        string? RefreshToken { get; set; }
    }

    public class TokenProvider : ITokenProvider
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
