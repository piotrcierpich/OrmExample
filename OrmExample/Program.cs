﻿using System;
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
            IEnumerable<Product> allProducts = mappingContext.GetProductMapper().GetAll();
        }

        private static void TestProductInsert()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Product kryptonite = new Product {Name = "Next generation blast", Price = 999.0m};
            mappingContext.GetProductMapper().Insert(kryptonite);
        }

        private static void TestProductUpdate()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Product product = mappingContext.GetProductMapper().GetById(2);
            product.Name += " improved!";
            mappingContext.GetProductMapper().Update(product);
        }

        private static void TestClientInsert()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Client newClient = new Client {Name = "John malkovic", Address = "NY Brooklyn"};
            mappingContext.GetClientMapper().Insert(newClient);
        }

        private static void TestClientUpdate()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Client clientToUpdate = mappingContext.GetClientMapper().GetById(5);
            clientToUpdate.Name += " what?";
            mappingContext.GetClientMapper().Update(clientToUpdate);
        }

        private static void TestGetClients()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Client client = mappingContext.GetClientMapper().GetById(2);
            IEnumerable<Client> clients = mappingContext.GetClientMapper().GetAll();
        }

        private static void TestUow()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            //mappingContext.SaveChanges();
        }

        private static void TestDelete()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            var productAt3 = mappingContext.GetProductMapper().GetAll().Skip(2).First();
            //mappingContext.GetClientMapper().DeleteById(productAt3.Id);
            mappingContext.GetClientMapper().DeleteById(10);
            var all = mappingContext.GetClientMapper().GetAll();
        }

        private static void TestIdentityMap()
        {
            MappingContext mappingContext = new MappingContext("ormExample");
            Client autocenter = new Client() {Address = "Garncarska 13 Krakow", Name = "Auto center"};
            mappingContext.GetClientMapper().Insert(autocenter);
            Client autoCenterFromDb = mappingContext.GetClientMapper().GetById(autocenter.Id);
            Debug.Assert(autoCenterFromDb.GetHashCode() == autocenter.GetHashCode(), "identity map fails");

            Client firstClientAt3 = mappingContext.GetClientMapper().GetById(2);
            Client secondClientAt3 = mappingContext.GetClientMapper().GetById(2);
            Debug.Assert(firstClientAt3.GetHashCode() == secondClientAt3.GetHashCode(), "identity map fails");
        }
    }
}
