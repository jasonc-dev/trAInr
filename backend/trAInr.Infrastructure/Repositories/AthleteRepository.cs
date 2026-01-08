using Microsoft.EntityFrameworkCore;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Aggregates;
using trAInr.Infrastructure.Data;

namespace trAInr.Infrastructure.Repositories;

/// <summary>
///     Repository implementation for Athlete aggregate root.
/// </summary>
public class AthleteRepository(TrainrDbContext context) : IAthleteRepository
{
    public async Task<Athlete?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Athletes
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Athlete?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Athletes
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
    }

    public async Task<Athlete?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Athletes
            .FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
    }

    public async Task<IEnumerable<Athlete>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Athletes
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Athletes
            .AnyAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Athletes
            .AnyAsync(a => a.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Athletes
            .AnyAsync(a => a.Username == username, cancellationToken);
    }

    public async Task AddAsync(Athlete athlete, CancellationToken cancellationToken = default)
    {
        await context.Athletes.AddAsync(athlete, cancellationToken);
    }

    public async Task UpdateAsync(Athlete athlete, CancellationToken cancellationToken = default)
    {
        context.Athletes.Update(athlete);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Athlete athlete, CancellationToken cancellationToken = default)
    {
        context.Athletes.Remove(athlete);
        await Task.CompletedTask;
    }
}

