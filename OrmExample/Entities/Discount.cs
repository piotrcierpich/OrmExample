namespace OrmExample.Entities
{
    public class Discount
    {
        public decimal GetDiscountedPrice()
        {
            return DiscountPolicy.CalculateDiscountedPrice(Product.Price);
        }

        public int Id { get; set; }
        public Product Product { get; set; }
        public IDiscountPolicy DiscountPolicy { get; set; }
    }
}