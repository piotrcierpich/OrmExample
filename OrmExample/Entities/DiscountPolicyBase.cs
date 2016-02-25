using OrmExample.Mapping;

namespace OrmExample.Entities
{
    public abstract class DiscountPolicyBase : EntityInUow, IDiscountPolicy
    {
        public abstract decimal CalculateDiscountedPrice(decimal price);
        public Percentage DiscountPercentage { get; set; }
    }
}