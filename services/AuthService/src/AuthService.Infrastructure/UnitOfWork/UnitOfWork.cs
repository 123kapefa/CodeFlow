using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork {

  private readonly AuthServiceDbContext _context;
  private readonly ILogger<UnitOfWork> _logger;
  private readonly List<Func<Task>> _afterCommitHooks = new ();

  public UnitOfWork (AuthServiceDbContext context, ILogger<UnitOfWork> logger) {
    _context = context;
    _logger = logger;
  }

  public void AddAfterCommitHook (Func<Task> hook) {
    _afterCommitHooks.Add (hook);
  }

  public async Task SaveChangesAsync (CancellationToken cancellationToken = default) {
    LogChanges();
    
    // 🔁 Транзакция с Outbox поддержкой
    var strategy = _context.Database.CreateExecutionStrategy ();
    await strategy.ExecuteAsync (async () =>
    {
      await using var transaction = await _context.Database.BeginTransactionAsync (cancellationToken);
      try {
        await _context.SaveChangesAsync (cancellationToken);

        // Выполняем хуки только если всё прошло успешно
        foreach (var hook in _afterCommitHooks) {
          await hook ();
        }

        await transaction.CommitAsync (cancellationToken);
      }
      catch (Exception ex) {
        _logger.LogError (ex, "Ошибка при сохранении изменений");
        await transaction.RollbackAsync (cancellationToken);
        throw;
      }
    });
  }

  private void LogChanges () {
    var entries = _context.ChangeTracker.Entries ()
     .Where (e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

    foreach (var entry in entries) {
      var entityName = entry.Entity.GetType ().Name;
      var state = entry.State.ToString ();
      var key = entry.Properties.FirstOrDefault (p => p.Metadata.IsPrimaryKey ())?.CurrentValue;

      _logger.LogInformation ("[AUDIT] Entity: {Entity}, State: {State}, Key: {Key}", entityName, state, key);

      if (entry.State == EntityState.Modified) {
        foreach (var prop in entry.Properties.Where (p => p.IsModified)) {
          _logger.LogInformation (" └─ {Property}: {Old} → {New}", prop.Metadata.Name, prop.OriginalValue
            , prop.CurrentValue);
        }
      }
    }
  }

}