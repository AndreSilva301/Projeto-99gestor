using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManiaDeLimpeza.Domain.Entities;
public class CustomerRelationship
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime DateTime { get; set; } = DateTime.UtcNow;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; } = false; // IsDeleted para nao trazer registros apagados
}
