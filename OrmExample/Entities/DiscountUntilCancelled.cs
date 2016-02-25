namespace OrmExample.Entities
{
    public class DiscountUntilCancelled : DiscountPolicyBase
    {
        public override decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            if (Cancelled)
                return originalPrice;

            return DiscountPercentage.Subtract(originalPrice);
        }

        public bool Cancelled { get; set; }
    }
}