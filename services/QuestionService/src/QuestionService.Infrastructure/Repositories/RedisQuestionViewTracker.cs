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
    // Ключ-троттлер "пользователь X смотрел вопрос Y за последние 24ч"
    // Пример: view:q:1b2c...:u:7f8e...
    var throttleKey = $"view:q:{questionId:N}:u:{viewerKey}";

    // SET NX EX 86400 — создастся, если не было, и сразу поставится TTL
    // Если reply == true -> первый раз за 24ч
    var created = await _redis.StringSetAsync (throttleKey, "1", _ttl, When.NotExists);

    return created;
  }
  
}