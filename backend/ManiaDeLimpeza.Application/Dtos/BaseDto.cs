using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos;

public abstract class BaseDto : IBasicDto
{
    protected List<string> _validationErrors = new();

    public virtual List<string> Validate()
    {
        _validationErrors.Clear();
        return _validationErrors;
    }

    public bool IsValid()
    {
        _validationErrors = Validate();
        return !_validationErrors.Any();
    }
}
