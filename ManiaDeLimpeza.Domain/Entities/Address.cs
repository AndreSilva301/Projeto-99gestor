using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class Address
    {
        public string Street { get; set; } = string.Empty;

        public string Number { get; set; } = string.Empty;

        public string? Complement { get; set; }

        public string Neighborhood { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;
    }
}
