using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleCanceledEvent : BaseEvent
    { 
        public Sale Sale { get; }
        public SaleCanceledEvent(Sale sale)
        {
            Sale = sale;
        }
    } 
}
