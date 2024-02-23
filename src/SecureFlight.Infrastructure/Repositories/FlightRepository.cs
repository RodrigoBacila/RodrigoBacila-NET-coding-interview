using Microsoft.EntityFrameworkCore;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SecureFlight.Infrastructure.Repositories;

public class FlightRepository(SecureFlightDbContext context) : IRepository<Flight>
{
    public async Task<IReadOnlyList<Flight>> FilterAsync(Expression<Func<Flight, bool>> predicate)
        => await context
            .Set<Flight>()
            .Include(flight => flight.Passengers)
            .Where(predicate)
            .ToListAsync();

    public async Task<IReadOnlyList<Flight>> GetAllAsync()
        => await context
            .Set<Flight>()
            .ToListAsync();

    public async Task<Flight> GetByIdAsync(params object[] keyValues)
    {
        return await context
            .Set<Flight>()
            .Include(flight => flight.Passengers)
            .FirstOrDefaultAsync(flight => flight.Id == (long)keyValues.FirstOrDefault());
    }

    public async Task<int> SaveChangesAsync()
        => await context.SaveChangesAsync();

    public Flight Update(Flight entity)
    {
        var entry = context.Entry(entity);

        entry.State = EntityState.Modified;

        context.SaveChanges();

        return entity;
    }

    public bool Delete(Flight entity)
    {
        var entry = context.Entry(entity);

        entry.State = EntityState.Deleted;

        context.SaveChanges();

        return true;
    }
}
