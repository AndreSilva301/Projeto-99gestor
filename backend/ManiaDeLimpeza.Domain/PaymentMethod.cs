using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain
{
    public enum PaymentMethod
    {
        Unknown = 0,
        Cash = 1,
        CreditCard = 2,
        DebitCard = 3,
        Pix = 4,
        BankTransfer = 5
    }
}
