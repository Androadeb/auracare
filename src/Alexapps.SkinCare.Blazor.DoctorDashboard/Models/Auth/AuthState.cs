namespace ynex.Models.Auth
{
    public  class AuthState
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
    }
}
