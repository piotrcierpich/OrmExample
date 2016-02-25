using System.Configuration;
using System.Data.SqlClient;
using NUnit.Framework;
using OrmExample.Entities;
using OrmExample.Mapping;

namespace Orm.Tests
{
    [TestFixture]
    public class DiscountTests
    {
        private const string ConnectionStringName = "testConnectionString";
        private MappingContext mappingContext;
        private string connectionString;

        [TestFixtureSetUp]
        public void TestsSetup()
        {
            connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            CleanDataInTable();
        }

        [SetUp]
        public void Setup()
        {
            mappingContext = new MappingContext(ConnectionStringName);
        }


        [TearDown]
        public void Cleanup()
        {
            CleanDataInTable();

            // this is a bug, entities are not removed from new objects 

            //UnitOfWork.Reset();
        }


        private static void CleanDataInTable()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand deleteDiscountsSqlCommand = new SqlCommand("DELETE FROM Discounts", sqlConnection);
                deleteDiscountsSqlCommand.ExecuteNonQuery();
                SqlCommand deleteProductsSqlCommand = new SqlCommand("DELETE FROM Products", sqlConnection);
                deleteProductsSqlCommand.ExecuteNonQuery();
            }
        }

        [Test]
        public void DiscountShouldBeSavedWithIdOfRelatedProduct()
        {
            Product p = new Product { Name = "Oil", Price = 89.0m };
            mappingContext.ProductMapper.Insert(p);
            mappingContext.SaveChanges();
            int expectedProductId = p.Id;
            Discount d = new Discount { Product = p };
            mappingContext.DiscountsMapper.Insert(d);
            mappingContext.SaveChanges();
            int discountId = d.Id;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlcommand = new SqlCommand("SELECT Product_Id FROM Discounts WHERE Id = " + discountId, sqlConnection))
                {
                    int actualProductId = (int)sqlcommand.ExecuteScalar();
                    Assert.AreEqual(expectedProductId, actualProductId);
                }
            }
        }

        [Test]
        [Ignore("For time being to make the mapper simplier, skip insertion of related entity")]
        public void DiscountShouldSaveRelatedProductIfNotSaved()
        {
            Product relatedProduct = new Product { Name = "Oil", Price = 89.0m };
            Discount discount = new Discount { Product = relatedProduct };
            mappingContext.DiscountsMapper.Insert(discount);
            mappingContext.SaveChanges();
            int expectedRelatedProductId = relatedProduct.Id;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlcommand = new SqlCommand("SELECT Product_Id FROM Discounts WHERE Id = " + discount.Id, sqlConnection))
                {
                    int actualProductId = (int)sqlcommand.ExecuteScalar();
                    Assert.AreEqual(expectedRelatedProductId, actualProductId);
                }
            }
        }

        [Test]
        public void DiscountShouldGetRelatedProduct()
        {
            Product discountedProduct = new Product { Name = "Oil", Price = 89.0m };
            mappingContext.ProductMapper.Insert(discountedProduct);
            mappingContext.SaveChanges();
            Discount discount = new Discount { Product = discountedProduct };
            mappingContext.DiscountsMapper.Insert(discount);
            mappingContext.SaveChanges();
            int idToGet = discount.Id;
            Discount sut = new MappingContext(ConnectionStringName).DiscountsMapper.GetById(idToGet);
            Assert.AreEqual(discountedProduct.Id, sut.Product.Id);
        }
    }
}
