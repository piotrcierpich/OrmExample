namespace OrmExample.Entities
{
    public class DiscountUntilCancelled : IDiscountPolicy
    {
        public decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            if (Cancelled)
                return originalPrice;

            return DiscountPercentage.Subtract(originalPrice);
        }

        public Percentage DiscountPercentage { get; set; }
        public bool Cancelled { get; set; }
    }
}