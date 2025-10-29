using ManiaDeLimpeza.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface ICustomerRelationshipService
{
    Task<IEnumerable<CustomerRelationshipDto>> AddOrUpdateRelationshipsAsync(
        int customerId,
        IEnumerable<CustomerRelationshipCreateOrUpdateDto> relationships,
        int userId);

    Task<IEnumerable<CustomerRelationshipDto>> ListRelationshipsAsync(int customerId);

    Task<bool> ValidateOwnershipAsync(int customerId, IEnumerable<int> relationshipIds);

    Task DeleteRelationshipsAsync(int customerId, IEnumerable<int> relationshipIds);
}
