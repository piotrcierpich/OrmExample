using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrmExample.Mapping;

namespace OrmExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Product rocketFuel = new Product { Id = 1, Name = "Rocket fuel", Price = 25.0m };
            
            MappingContext mappingContext = new MappingContext("ormExample");
            Client client = mappingContext.GetClientMapper().GetById(1);

            Product product = mappingContext.GetProductMapper().GetById(2); 

            IEnumerable<Product> blastProducts = mappingContext.GetProductMapper().GetByName("blast");
            IEnumerable<Product> allProducts = mappingContext.GetProductMapper().GetAll();

            Product kryptonite = new Product{Name = "Next generation blast", Price = 999.0m };
            mappingContext.GetProductMapper().Insert(kryptonite);

            Product rocketFuel = new Product { Id = 1, Name = "Rocket fuel", Price = 25.0m };
            Product doubleBlast = new Product { Id = 2, Name = "Double blast", Price = 40.0m };
            Discount rocketFuelDiscount = new Discount { Product = rocketFuel, DiscountPolicy = new PromoDay { DayDate = DateTime.Today, DiscountPercentage = new Percentage(20)} };
            Client josephDeer = new Client
                {
                    Name = "Joseph Deer",
                    Address = "Czerwone Maki 84",
                };

            josephDeer.Offer(rocketFuelDiscount);
            Discount doubleBlastAccount = new Discount
                {
                    Product = doubleBlast,
                    DiscountPolicy =
                        new DiscountUntilExpired
                            {
                                DiscountPercentage = new Percentage(50),
                                FromTo = new DateSpan { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(2) }
                            }
                };
            josephDeer.Offer(doubleBlastAccount);
            Client regularClient = new Client
                {
                    Name = "Jack Ryan",
                    Address = "Rynek 5"
                };


            decimal regularPrice = regularClient.GetPriceForThisClient(doubleBlast);
            decimal discountedPrice = josephDeer.GetPriceForThisClient(doubleBlast);
            Console.WriteLine("double Blast regular price ${0}, discounted price only ${1}!", regularPrice, discountedPrice);

            regularPrice = regularClient.GetPriceForThisClient(rocketFuel);
            discountedPrice = josephDeer.GetPriceForThisClient(rocketFuel);
            Console.WriteLine("rocket fuel regular price ${0}, discounted price only ${1}!", regularPrice, discountedPrice);

            Console.ReadKey();
        }
    }
}
