using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Infrastructure.Repositories;

public class AirportRepository(SecureFlightDbContext context) : IRepository<Airport>
{
    public async Task<IReadOnlyList<Airport>> FilterAsync(Expression<Func<Airport, bool>> predicate) => await context.Set<Airport>().AsNoTracking().Where(predicate).ToListAsync();

    public async Task<IReadOnlyList<Airport>> GetAllAsync() => await context.Set<Airport>().AsNoTracking().ToListAsync();

    public async Task<Airport> GetByIdAsync(params object[] keyValues)
    {
        return await context
            .Set<Airport>()
            .Include(airport => airport.OriginFlights)
            .Include(airport => airport.DestinationFlights)
            .FirstOrDefaultAsync(airport => airport.Code == (string)keyValues.FirstOrDefault() || airport.Name == (string)keyValues.FirstOrDefault());
    }

    public async Task<int> SaveChangesAsync() 
        => await context.SaveChangesAsync();

    public Airport Update(Airport entity)
    {
        var entry = context.Entry(entity);

        entry.State = EntityState.Modified;

        context.SaveChanges();

        return entity;
    }

    public bool Delete(Airport entity)
    {
        var entry = context.Entry(entity);

        entry.State = EntityState.Deleted;

        context.SaveChanges();

        return true;
    }
}