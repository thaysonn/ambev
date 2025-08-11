using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleUpdatedEvent : BaseEvent
{
    public Sale Sale { get; }
    public SaleUpdatedEvent(Sale sale)
    {
        Sale = sale;
    }
}
