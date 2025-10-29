using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Exceptions;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Services;
public class CustomerRelationshipService : ICustomerRelationshipService, IScopedDependency
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerRelationshipRepository _relationshipRepository;

    public CustomerRelationshipService(
        ICustomerRepository customerRepository,
        ICustomerRelationshipRepository relationshipRepository)
    {
        _customerRepository = customerRepository;
        _relationshipRepository = relationshipRepository;
    }

    public async Task<IEnumerable<CustomerRelationshipDto>> AddOrUpdateRelationshipsAsync(
        int customerId,
        IEnumerable<CustomerRelationshipCreateOrUpdateDto> relationships,
        int userId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new BusinessException("Customer not found.");

        var result = new List<CustomerRelationshipDto>();

        foreach (var rel in relationships)
        {
            CustomerRelationship entity;

            if (rel.Id == 0)
            {
                entity = new CustomerRelationship
                {
                    CustomerId = customerId,
                    Description = rel.Description,
                    DateTime = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };
                await _relationshipRepository.AddAsync(entity);
            }
            else
            {
                entity = await _relationshipRepository.GetByIdAsync(rel.Id);
                if (entity == null)
                    continue;

                entity.Description = rel.Description;
                entity.UpdatedDate = DateTime.UtcNow;

                await _relationshipRepository.UpdateAsync(entity);
            }

            result.Add(new CustomerRelationshipDto
            {
                Id = entity.Id,
                Description = entity.Description,
                DateTime = entity.DateTime
            });
        }

        return result;
    }

    public async Task<IEnumerable<CustomerRelationshipDto>> ListRelationshipsAsync(int customerId)
    {
        var entities = await _relationshipRepository.ListByCustomerIdAsync(customerId);

        return entities
            .Where(r => !r.IsDeleted)
            .Select(r => new CustomerRelationshipDto
            {
                Id = r.Id,
                Description = r.Description,
                DateTime = r.DateTime
            })
            .OrderByDescending(r => r.DateTime)
            .ToList();
    }

    public async Task<bool> ValidateOwnershipAsync(int customerId, IEnumerable<int> relationshipIds)
    {
        var entities = await _relationshipRepository.ListByCustomerIdAsync(customerId);
        var validIds = entities.Select(r => r.Id).ToHashSet();
        return relationshipIds.All(id => validIds.Contains(id));
    }

    public async Task DeleteRelationshipsAsync(int customerId, IEnumerable<int> relationshipIds)
    {
        var entities = await _relationshipRepository.ListByCustomerIdAsync(customerId);
        var toDelete = entities.Where(e => relationshipIds.Contains(e.Id));

        foreach (var rel in toDelete)
        {
            rel.IsDeleted = true;
            rel.UpdatedDate = DateTime.UtcNow;
            await _relationshipRepository.UpdateAsync(rel);
        }
    }
}
