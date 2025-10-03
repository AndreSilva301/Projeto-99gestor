//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ManiaDeLimpeza.Domain.Entities;
//public class Custumer // cliente
//{
//    [Key]
//    public int Id { get; set; }

//    [Required(ErrorMessage = "CompanyId is required")]
//    [ForeignKey("Company")]
//    public int CompanyId { get; set; }
//    public Company Company { get; set; } = null!;

//    [Required(ErrorMessage = "Name is required")]
//    [MaxLength(150)]
//    public string Name { get; set; } = string.Empty;

//    [Required(ErrorMessage = "Number is required")]
//    [MaxLength(11)]
//    public string Number { get; set; } = string.Empty;

//    [Required(ErrorMessage = "Email is required")]
//    [MaxLength(250)]
//    public string Email { get; set; } = string.Empty;

//    [Required(ErrorMessage = "Address is required")]
//    [MaxLength(400)]
//    public string Address { get; set; } = string.Empty;

//    public DateTime DateTime { get; set; } = DateTime.UtcNow;
//    public List<CustumerRelationship> CostumerRelationships { get; set; } = new();
//    public List<Quote> Quotes { get; set; } = new();
//}
