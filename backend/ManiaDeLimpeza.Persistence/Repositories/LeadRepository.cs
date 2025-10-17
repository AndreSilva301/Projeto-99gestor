using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace ManiaDeLimpeza.Persistence.Repositories;
public class LeadRepository : BaseRepository<Lead>, ILeadRepository, IScopedDependency
{
    protected readonly ApplicationDbContext _context;
    public LeadRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}