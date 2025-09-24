using FileNet.Application.Abstractions.Messaging;
using FileNet.Domain.Events;
using MediatR;

namespace FileNet.Infrastructure.Messaging;

public sealed class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public MediatRDomainEventDispatcher(IMediator mediator) => _mediator = mediator;

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var e in events)
            await _mediator.Publish(e, ct);
    }
}
