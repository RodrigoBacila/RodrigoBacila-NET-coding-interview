using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Infrastructure.Repositories;

public class PassengerRepository(SecureFlightDbContext context) : IRepository<Passenger>
{
    public async Task<IReadOnlyList<Passenger>> FilterAsync(Expression<Func<Passenger, bool>> predicate) 
        => await context
            .Set<Passenger>()
            .Where(predicate)
            .ToListAsync();

    public async Task<IReadOnlyList<Passenger>> GetAllAsync() 
        => await context
            .Set<Passenger>()
            .ToListAsync();

    public async Task<Passenger> GetByIdAsync(params object[] keyValues)
    {
        return await context
            .Set<Passenger>()
            .Include(passenger => passenger.Flights)
            .FirstOrDefaultAsync(Passenger => Passenger.Id == (string)keyValues.FirstOrDefault());
    }

    public async Task<int> SaveChangesAsync() 
        => await context.SaveChangesAsync();

    public Passenger Update(Passenger entity)
    {
        var entry = context.Entry(entity);

        entry.State = EntityState.Modified;

        context.SaveChanges();

        return entity;
    }

    public bool Delete(Passenger entity)
    {
        var entry = context.Entry(entity);

        entry.State = EntityState.Deleted;

        context.SaveChanges();

        return true;
    }
}
