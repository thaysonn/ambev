using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class SaleUpdatedEventHandler : INotificationHandler<SaleUpdatedEvent>
{
    private readonly ILogger<SaleUpdatedEventHandler> _logger;

    public SaleUpdatedEventHandler(ILogger<SaleUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}
