using System;

namespace OrmExample.Entities
{
    public class DiscountUntilExpired : IDiscountPolicy
    {
        public decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            if (IsDiscountTime())
            {
                return DiscountPercentage.Subtract(originalPrice);
            }
            return originalPrice;
        }

        private bool IsDiscountTime()
        {
            return FromTo.Encloses(DateTime.Now) == false;
        }

        public Percentage DiscountPercentage { get; set; }
        public DateSpan FromTo { get; set; }
    }
}