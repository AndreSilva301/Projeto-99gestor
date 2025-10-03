using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(400)]
        public Address Address { get; set; } = new Address();

        [Required(ErrorMessage = "Number is required")]
        [MaxLength(11)]
        public Phone Phone { get; set; } = new Phone();


        [Required(ErrorMessage = "Email is required")]
        [MaxLength(250)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Observations { get; set; }

        public List<CustumerRelationship> CostumerRelationships { get; set; } = new();
        public List<Quote> Quotes { get; set; } = new();
        public DateTime? DateTime { get; set; }
    }
}
