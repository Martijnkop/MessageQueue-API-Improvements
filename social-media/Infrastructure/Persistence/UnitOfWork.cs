using Social.Common.Primitives.ServiceLifetimes;
using Social.Domain.Abstractions;

namespace Social.Persistence;

public class UnitOfWork : IUnitOfWork, ITransient
{
    private readonly AppDataStore _context;
    public UnitOfWork(AppDataStore context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }
}
