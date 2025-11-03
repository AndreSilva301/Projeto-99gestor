using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*public class CustomerRelationshipCreateOrUpdateDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    public List<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("Description is required.");
        if (Description.Length > 500)
            errors.Add("Description maximum length is 500 characters.");
        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}
*/
