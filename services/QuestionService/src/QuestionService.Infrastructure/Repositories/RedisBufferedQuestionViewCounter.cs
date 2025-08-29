using QuestionService.Application.Abstractions;

using StackExchange.Redis;

namespace QuestionService.Infrastructure.Repositories;

public class RedisBufferedQuestionViewCounter : IQuestionViewCounter
{
  private readonly IDatabase _redis;
  public RedisBufferedQuestionViewCounter(IConnectionMultiplexer mux) => _redis = mux.GetDatabase();

  public Task IncrementAsync(Guid questionId, CancellationToken ct)
    => _redis.StringIncrementAsync($"viewbuf:q:{questionId:N}");
}