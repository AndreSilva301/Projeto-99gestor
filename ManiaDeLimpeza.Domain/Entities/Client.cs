using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Address Address { get; set; } = new Address();

        public Phone Phone { get; set; } = new Phone();

        public DateTime? Birthday { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? Observations { get; set; }
    }
}
