namespace Ambev.DeveloperEvaluation.Domain.Policies
{
    public interface IDiscountPolicy
    {
        decimal GetPercent(int quantity);
    }
}
