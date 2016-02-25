using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OrmExample.Entities;
using OrmExample.Mapping;

namespace Orm.Tests
{
    [TestFixture]
    public class DiscountPolicyTests
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
                SqlCommand deleteDiscountsSqlCommand = new SqlCommand("DELETE FROM DiscountPolicies", sqlConnection);
                deleteDiscountsSqlCommand.ExecuteNonQuery();
            }
        }

        [Test]
        public void DiscountPoliciesShouldBeStoredInTheSameTable()
        {
            DiscountPolicyBase discountUntilExpired = new DiscountUntilExpired
                {
                    DiscountPercentage = new Percentage(20),
                    FromTo = new DateSpan {StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1)}
                };
            DiscountPolicyBase promoDay = new PromoDay
                {
                    DiscountPercentage = new Percentage(10),
                    DayDate = DateTime.Today
                };
        }
    }
}
