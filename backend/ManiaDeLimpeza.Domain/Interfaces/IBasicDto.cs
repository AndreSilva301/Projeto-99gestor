using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Interfaces
{
    public interface IBasicDto
    {
        public List<string> Validate();

        public bool IsValid();
    }
}
