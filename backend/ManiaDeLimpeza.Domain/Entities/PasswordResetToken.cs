namespace ManiaDeLimpeza.Domain.Entities;
public class PasswordResetToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public User User { get; set; } = null!;
}
