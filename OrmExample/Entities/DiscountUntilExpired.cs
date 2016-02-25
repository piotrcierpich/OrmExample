using System;

namespace OrmExample.Entities
{
    public class DiscountUntilExpired : DiscountPolicyBase
    {
        public override decimal CalculateDiscountedPrice(decimal originalPrice)
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

        public DateSpan FromTo { get; set; }
    }
}