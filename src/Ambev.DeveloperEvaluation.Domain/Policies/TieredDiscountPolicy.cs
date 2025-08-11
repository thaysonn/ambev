namespace Ambev.DeveloperEvaluation.Domain.Policies
{
    public sealed class TieredDiscountPolicy : IDiscountPolicy
    {
        public decimal GetPercent(int quantity)
        {
            if (quantity >= 10 && quantity <= 20) return 0.20m;
            if (quantity >= 4 && quantity <= 9) return 0.10m;
            return 0.00m;
        }
    }
}
