using System;

namespace OrmExample.Entities
{
    public class PromoDay : IDiscountPolicy
    {
        public decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            if (IsThisToday())
                return DiscountPercentage.Subtract(originalPrice);

            return originalPrice;
        }

        private bool IsThisToday()
        {
            return DateTime.Today == DayDate.Date;
        }

        public Percentage DiscountPercentage { get; set; }
        public DateTime DayDate { get; set; }
    }
}