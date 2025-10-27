using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerRelationshipUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;
}