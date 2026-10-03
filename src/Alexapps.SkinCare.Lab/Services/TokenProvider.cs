namespace starterkit.Services
{
  public interface ITokenProvider
  {
    string? AccessToken { get; set; }
    string? RefreshToken { get; set; }
  }

  public class InMemoryTokenProvider : ITokenProvider
  {
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
  }
}
