using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class SaleCanceledEventHandler : INotificationHandler<SaleCanceledEvent>
{
    private readonly ILogger<SaleCanceledEventHandler> _logger;

    public SaleCanceledEventHandler(ILogger<SaleCanceledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCanceledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}