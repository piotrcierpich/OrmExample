namespace OrmExample.Entities
{
    public interface IDiscountPolicy
    {
        decimal CalculateDiscountedPrice(decimal price);
        Percentage DiscountPercentage { get; set; }
    }
}