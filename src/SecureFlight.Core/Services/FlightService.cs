using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Core.Services;

public class FlightService(IRepository<Flight> repository) : IService<Flight>
{
    public async Task<OperationResult> DeleteAsync(Flight entity) 
        => await Task.FromResult(OperationResult<bool>.Success(repository.Delete(entity)));

    public async Task<OperationResult<IReadOnlyList<Flight>>> FilterAsync(Expression<Func<Flight, bool>> predicate) 
        => OperationResult<IReadOnlyList<Flight>>.Success(await repository.FilterAsync(predicate));

    public async Task<OperationResult<Flight>> FindAsync(params object[] keyValues)
    {
        var entity = await repository.GetByIdAsync(keyValues);

        return entity is null ?
            OperationResult<Flight>.NotFound($"Entity with key values {string.Join(", ", keyValues)} was not found") :
            OperationResult<Flight>.Success(entity);
    }

    public async Task<OperationResult<IReadOnlyList<Flight>>> GetAllAsync() 
        => OperationResult<IReadOnlyList<Flight>>.Success(await repository.GetAllAsync());

    public async Task<OperationResult<Flight>> UpdateAsync(Flight entity) 
        => await Task.FromResult(OperationResult<Flight>.Success(repository.Update(entity)));
}
