using System.Collections.Generic;
using System.Linq;
using OrmExample.Mapping;

namespace OrmExample.Entities
{
    public class Client : EntityInUow
    {
        private string name;
        private string address;

        public Client()
        {
            Offers = new List<Discount>();
        }

        public void Offer(Discount discount)
        {
            Offers.Add(discount);
        }

        public decimal GetPriceForThisClient(Product product)
        {
            Discount specialOffer = GetOfferForProductOrNull(product);
            return specialOffer != null
                        ? specialOffer.GetDiscountedPrice()
                        : product.Price;
        }

        private Discount GetOfferForProductOrNull(Product product)
        {
            return Offers.FirstOrDefault(offering => offering.Product.Equals(product));
        }

        public ICollection<Discount> Offers { get; set; }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                MarkDirty();
            }
        }

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                MarkDirty();
            }
        }
    }
}
