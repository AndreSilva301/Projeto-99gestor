using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerRelationshipCreateOrUpdateDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
}
