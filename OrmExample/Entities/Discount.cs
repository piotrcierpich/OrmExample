using OrmExample.Mapping;

namespace OrmExample.Entities
{
    public class Discount : EntityInUow
    {
        private Product product;
        private IDiscountPolicy discountPolicy;

        public decimal GetDiscountedPrice()
        {
            return DiscountPolicy.CalculateDiscountedPrice(Product.Price);
        }

        public Product Product
        {
            get { return product; }
            set
            {
                product = value;
                MarkDirty();
            }
        }

        public IDiscountPolicy DiscountPolicy
        {
            get { return discountPolicy; }
            set
            {
                discountPolicy = value;
                MarkDirty();
            }
        }
    }
}