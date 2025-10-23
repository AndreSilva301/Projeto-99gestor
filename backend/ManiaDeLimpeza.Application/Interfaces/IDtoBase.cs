using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Interfaces;
internal interface IDtoBase
{
    IEnumerable<string> Validate();
    bool IsValid();
}
