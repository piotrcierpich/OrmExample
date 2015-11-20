using System;
using System.Collections.Generic;
using System.Linq;
using OrmExample.Mapping;

namespace OrmExample.Entities
{
    public class Client : EntityInUow
    {
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
        public string Name { get; set; }
        public string Address { get; set; }
    }

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

    public interface IDiscountPolicy
    {
        decimal CalculateDiscountedPrice(decimal price);
        Percentage DiscountPercentage { get; set; }
    }

    public class Percentage
    {
        private int value;

        public Percentage(int value)
        {
            this.value = value;
        }

        public decimal Subtract(decimal price)
        {
            decimal fraction = (decimal)Value / 100;
            decimal pricePercentage = price * fraction;
            return price - pricePercentage;
        }

        public int Value
        {
            get { return value; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("percenttage cannot be less than 0");
                if (value > 100)
                    throw new ArgumentException("percentage cannot be more than 100");

                this.value = value;
            }
        }
    }

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

    public class DiscountUntilCancelled : IDiscountPolicy
    {
        public decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            if (Cancelled)
                return originalPrice;

            return DiscountPercentage.Subtract(originalPrice);
        }

        public Percentage DiscountPercentage { get; set; }
        public bool Cancelled { get; set; }
    }

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

    public class Product : EntityInUow
    {
        public override bool Equals(object obj)
        {
            Product other = obj as Product;
            if (other == null) return false;
            return other.Id == Id;
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class DateSpan
    {
        public bool Encloses(DateTime dateTime)
        {
            return StartDate >= dateTime && EndDate <= dateTime;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
