namespace AuthService.Infrastructure.UnitOfWork;

public interface IUnitOfWork {

  Task SaveChangesAsync (CancellationToken cancellationToken = default);
  void AddAfterCommitHook (Func<Task> hook);

}