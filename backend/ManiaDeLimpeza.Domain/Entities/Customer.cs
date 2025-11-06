using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class Customer 
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "CompanyId is required")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        public Address Address { get; set; } = new Address();

        public Phone Phone { get; set; } = new Phone();

        public string Email { get; set; } = string.Empty;

        public string? Observations { get; set; }

        public List<CustomerRelationship> CustomerRelationships { get; set; } = new();
        public List<Quote> Quotes { get; set; } = new();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
