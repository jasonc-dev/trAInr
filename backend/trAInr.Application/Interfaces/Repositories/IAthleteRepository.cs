using trAInr.Domain.Aggregates;

namespace trAInr.Application.Interfaces.Repositories;

/// <summary>
///     Repository interface for Athlete aggregate root.
/// </summary>
public interface IAthleteRepository
{
    Task<Athlete?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Athlete?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Athlete?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<Athlete>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task AddAsync(Athlete athlete, CancellationToken cancellationToken = default);
    Task UpdateAsync(Athlete athlete, CancellationToken cancellationToken = default);
    Task DeleteAsync(Athlete athlete, CancellationToken cancellationToken = default);
}

