using Abstractions.Commands;

using Contracts.Responses.AnswerService;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using ReputationService.Application.Features.AcceptedAnswerChanged;
using ReputationService.Application.Features.VoteChanged;
using ReputationService.Application.Policies;
using ReputationService.Domain.Policies;
using ReputationService.Domain.Repositories;
using ReputationService.Infrastructure.Repositories;

namespace ReputationService.Api.Extensions;

public static class HandlerCollectionExtensions {

  public static WebApplicationBuilder AddHandlers (this WebApplicationBuilder builder) {
    
    builder.Services.AddScoped<IReputationRepository, ReputationRepository> ();
    builder.Services.AddScoped<IReputationPolicy, DefaultReputationPolicy> ();
    
    builder.Services.AddScoped<ICommandHandler<VoteChangedCommand>, VoteChangedHandler> ();
    builder.Services.AddScoped<ICommandHandler<AcceptedAnswerChangedCommand>, AcceptedAnswerChangedHandler> ();
    
    return builder;
  }

}