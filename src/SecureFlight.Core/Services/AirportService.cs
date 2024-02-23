using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Core.Services;

public class AirportService(IRepository<Airport> repository) : IService<Airport>
{
    public async Task<OperationResult> DeleteAsync(Airport entity)
        => await Task.FromResult(OperationResult<bool>.Success(repository.Delete(entity)));

    public async Task<OperationResult<IReadOnlyList<Airport>>> FilterAsync(Expression<Func<Airport, bool>> predicate)
        => OperationResult<IReadOnlyList<Airport>>.Success(await repository.FilterAsync(predicate));

    public async Task<OperationResult<Airport>> FindAsync(params object[] keyValues)
    {
        var entity = await repository.GetByIdAsync(keyValues);

        return entity is null ?
            OperationResult<Airport>.NotFound($"Entity with key values {string.Join(", ", keyValues)} was not found") :
            OperationResult<Airport>.Success(entity);
    }

    public async Task<OperationResult<IReadOnlyList<Airport>>> GetAllAsync()
        => OperationResult<IReadOnlyList<Airport>>.Success(await repository.GetAllAsync());

    public async Task<OperationResult<Airport>> UpdateAsync(Airport entity)
        => await Task.FromResult(OperationResult<Airport>.Success(repository.Update(entity)));
}