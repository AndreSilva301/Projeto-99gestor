using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;

namespace ManiaDeLimpeza.Domain.Entities;
public class Company
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Company Name is required")]
    public string Name { get; set; } = string.Empty;

    public string? CNPJ { get; set; } 
    public Address Address { get; set; } = new Address();
    public Phone Phone { get; set; } = new Phone();

    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    public List<User> Users { get; set; } = new();
}
