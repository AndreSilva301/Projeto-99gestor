using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Domain.Entities;
public class Company
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Company Name is required")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public decimal CNPJ { get; set; }

    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    public List<User> Users { get; set; } = new(); 
    public List<Customer> Costumers { get; set; } = new();
}
