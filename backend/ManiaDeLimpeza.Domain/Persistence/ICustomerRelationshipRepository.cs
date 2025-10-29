using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Persistence;
public interface ICustomerRelationshipRepository
{
    Task<CustomerRelationship?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerRelationship>> ListByCustomerIdAsync(int customerId);
    Task AddAsync(CustomerRelationship entity);
    Task UpdateAsync(CustomerRelationship entity);
}
