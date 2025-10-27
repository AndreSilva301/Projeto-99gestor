using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos;
public class CustomerListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PhoneDto Phone { get; set; } = new PhoneDto();
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
