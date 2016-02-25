using System;
using System.Collections.Generic;
using System.Linq;
using OrmExample.Entities;
using OrmExample.Mapping;
using System.Diagnostics;

namespace OrmExample
{
    class Program
    {
        static void Main()
        {
            TestIdentityMap();
            TestUow();

            TestGetClients();
            TestClientUpdate();

            TestClientInsert();

            TestProductUpdate();

            TestProductsGet();

            TestProductInsert();

            DomainTest();
        }

        private static void DomainTest()
        {
            Product rocketFuel = new Product {Id = 1, Name = "Rocket fuel", Price = 25.0m};
            Product doubleBlast = new Product {Id = 2, Name = "Double blast", Price = 40.0m};
            Discount rocketFuelDiscount = new Discount
                {
                    Product = rocketFuel,
                    DiscountPolicy = new PromoDay {DayDate = DateTime.Today, DiscountPercentage = new Percentage(20)}
                };
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
                                FromTo = new DateSpan {StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(2)}
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

        private static void TestProductsGet()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            IEnumerable<Product> allProducts = mappingContext.ProductMapper.GetAll();
        }

        private static void TestProductInsert()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Product kryptonite = new Product {Name = "Next generation blast", Price = 999.0m};
            mappingContext.ProductMapper.Insert(kryptonite);
        }

        private static void TestProductUpdate()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Product product = mappingContext.ProductMapper.GetById(2);
            product.Name += " improved!";
            mappingContext.ProductMapper.Update(product);
        }

        private static void TestClientInsert()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Client newClient = new Client {Name = "John malkovic", Address = "NY Brooklyn"};
            mappingContext.ClientMapper.Insert(newClient);
        }

        private static void TestClientUpdate()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Client clientToUpdate = mappingContext.ClientMapper.GetById(5);
            clientToUpdate.Name += " what?";
            mappingContext.ClientMapper.Update(clientToUpdate);
        }

        private static void TestGetClients()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Client client = mappingContext.ClientMapper.GetById(2);
            IEnumerable<Client> clients = mappingContext.ClientMapper.GetAll();
        }

        private static void TestUow()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Client c1 = mappingContext.ClientMapper.GetById(1);
            c1.Name += "UOW modified";
            mappingContext.SaveChanges();
            Client c2 = mappingContext.ClientMapper.GetById(1);
            Debug.Assert(c2.Name.Contains("UOW modified"));
        }

        private static void TestDelete()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            var productAt3 = mappingContext.ProductMapper.GetAll().Skip(2).First();
            //mappingContext.GetClientMapper().DeleteById(productAt3.Id);
            mappingContext.ClientMapper.DeleteById(10);
            var all = mappingContext.ClientMapper.GetAll();
        }

        private static void TestIdentityMap()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Client autocenter = new Client() {Address = "Garncarska 13 Krakow", Name = "Auto center"};
            mappingContext.ClientMapper.Insert(autocenter);
            mappingContext.SaveChanges();
            // TODO cannot use ID as the entity has not been inserted yet maybe mappingContext.SaveChanged()
            Client autoCenterFromDb = mappingContext.ClientMapper.GetById(autocenter.Id);
            Debug.Assert(autoCenterFromDb.GetHashCode() == autocenter.GetHashCode(), "identity map fails");

            Client firstClientAt3 = mappingContext.ClientMapper.GetById(2);
            Client secondClientAt3 = mappingContext.ClientMapper.GetById(2);
            Debug.Assert(firstClientAt3.GetHashCode() == secondClientAt3.GetHashCode(), "identity map fails");
        }
    }
}
