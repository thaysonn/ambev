using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCreatedEvent : BaseEvent
{
    public Sale Sale { get; }
    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
    }
}
