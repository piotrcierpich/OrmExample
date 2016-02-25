using System;

namespace OrmExample.Entities
{
    public class PromoDay : DiscountPolicyBase
    {
        public override decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            if (IsThisToday())
                return DiscountPercentage.Subtract(originalPrice);

            return originalPrice;
        }

        private bool IsThisToday()
        {
            return DateTime.Today == DayDate.Date;
        }

        public DateTime DayDate { get; set; }
    }
}