using Abstractions.Commands;

namespace TagService.Application.Features.WatchedTags.CheckUserSubscription;

public record CheckUserSubscriptionCommand (Guid UserId, int TagId) : ICommand;