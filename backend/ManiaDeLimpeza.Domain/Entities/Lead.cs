namespace ManiaDeLimpeza.Domain.Entities;
public class Lead
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsNew { get; set; } = true;
}
