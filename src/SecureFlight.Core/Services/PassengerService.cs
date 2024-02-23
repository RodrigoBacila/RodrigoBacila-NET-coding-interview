using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Core.Services;

public class PassengerService(IRepository<Passenger> repository) : IService<Passenger>
{
    public async Task<OperationResult> DeleteAsync(Passenger entity)
        => await Task.FromResult(OperationResult<bool>.Success(repository.Delete(entity)));

    public async Task<OperationResult<IReadOnlyList<Passenger>>> FilterAsync(Expression<Func<Passenger, bool>> predicate)
        => OperationResult<IReadOnlyList<Passenger>>.Success(await repository.FilterAsync(predicate));

    public async Task<OperationResult<Passenger>> FindAsync(params object[] keyValues)
    {
        var entity = await repository.GetByIdAsync(keyValues);

        return entity is null ?
            OperationResult<Passenger>.NotFound($"Entity with key values {string.Join(", ", keyValues)} was not found") :
            OperationResult<Passenger>.Success(entity);
    }

    public async Task<OperationResult<IReadOnlyList<Passenger>>> GetAllAsync()
        => OperationResult<IReadOnlyList<Passenger>>.Success(await repository.GetAllAsync());

    public async Task<OperationResult<Passenger>> UpdateAsync(Passenger entity)
        => await Task.FromResult(OperationResult<Passenger>.Success(repository.Update(entity)));
}
