using QuestionService.Application.Abstractions;

using StackExchange.Redis;

namespace QuestionService.Infrastructure.Repositories;

public class RedisQuestionViewTracker : IQuestionViewTracker {

  private readonly IDatabase _redis;
  private readonly TimeSpan _ttl = TimeSpan.FromHours (24);

  public RedisQuestionViewTracker (IConnectionMultiplexer mux) {
    _redis = mux.GetDatabase ();
  }

  public async Task<bool> TryTrackAsync (Guid questionId, string viewerKey, CancellationToken ct) {
    var throttleKey = $"view:q:{questionId:N}:u:{viewerKey}";
    var created = await _redis.StringSetAsync (throttleKey, "1", _ttl, When.NotExists);

    return created;
  }
  
}