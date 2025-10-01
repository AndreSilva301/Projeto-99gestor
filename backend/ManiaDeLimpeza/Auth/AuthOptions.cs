namespace ManiaDeLimpeza.Api.Auth
{
    public class AuthOptions
    {
        public const string SECTION = "Auth";
        public string JwtSecret { get; set; }
        public int ExpireTimeInSeconds { get; set; } = 12*60*60; //12h
    }
}
