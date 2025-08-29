using Abstractions.Commands;

using Contracts.Publishers.VoteService;
using Contracts.Responses.VoteService;

using VoteService.Application.Handlers.SetVote;
using VoteService.Application.Services;
using VoteService.Domain.Repositories;
using VoteService.Domain.Services;
using VoteService.Infrastructure.Repositories;

namespace VoteService.Api.Extensions;

public static class HandlerCollectionExtensions {

  public static WebApplicationBuilder AddHandlers (this WebApplicationBuilder builder) {

    builder.Services.AddDistributedMemoryCache();
    
    builder.Services.AddScoped<IVoteRepository, VoteRepository> ();
    builder.Services.AddScoped<IAntiAbusePolicy, DefaultAntiAbusePolicy> ();
    
    builder.Services.AddScoped<
      ICommandHandler<VoteResponse, SetVoteCommand>,
      SetVoteHandler>();
    return builder;
  }

}