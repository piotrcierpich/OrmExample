using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrmExample;

namespace EntityFrameworkExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Product rocketFuel = new Product { Name = "Rocket fuel", Price = 25.0m };
            Product doubleBlast = new Product { Name = "Double blast", Price = 40.0m };
            Product tripleBlast = new Product { Name = "Triple blast", Price = 40.0m };
            PromoContext dbContext = new PromoContext();
            //dbContext.Products.Add(rocketFuel);
            //dbContext.Products.Add(doubleBlast);
            dbContext.Products.Add(tripleBlast);

            Discount rocketFuelDiscount = new Discount { Product = rocketFuel, DiscountPolicy = new PromoDay { DayDate = DateTime.Today, DiscountPercentage = new Percentage(20) } };
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

            dbContext.Clients.Add(regularClient);
            dbContext.Clients.Add(josephDeer);
            dbContext.SaveChanges();
        }
    }

    class PromoContext : DbContext
    {
        public PromoContext()
            : base("ormExampleEf")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Client>().HasKey(c => c.Id);
            modelBuilder.Entity<Discount>().HasKey(d => d.Id);
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
