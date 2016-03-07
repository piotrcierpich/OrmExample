using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
            DiscountUntilExpired discountUntilExpired = new DiscountUntilExpired
                {
                    DiscountPercentage = new Percentage(20),
                    FromTo = new DateSpan {StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1)}
                };
            PromoDay promoDay = new PromoDay
                {
                    DiscountPercentage = new Percentage(10),
                    DayDate = DateTime.Today
                };

            mappingContext.DiscountPolicies.Insert(discountUntilExpired);
            mappingContext.DiscountPolicies.Insert(promoDay);
            mappingContext.SaveChanges();

            DiscountUntilExpired discountUntilExpiredReadFromDb = ReadDiscountUntilExpiredFromDatabase(discountUntilExpired.Id);
            Assert.AreEqual(discountUntilExpiredReadFromDb.Id, discountUntilExpired.Id);
            Assert.AreEqual(discountUntilExpiredReadFromDb.DiscountPercentage, discountUntilExpired.DiscountPercentage);
            Assert.AreEqual(discountUntilExpiredReadFromDb.FromTo, discountUntilExpired.FromTo);

            PromoDay promoDayReadFromDb = ReadPromoDayFromDatabase(promoDay.Id);
            Assert.AreEqual(promoDayReadFromDb.Id, promoDay.Id);
            Assert.AreEqual(promoDayReadFromDb.DiscountPercentage, promoDay.DiscountPercentage);
            Assert.AreEqual(promoDayReadFromDb.DayDate, promoDay.DayDate);
        }

        private PromoDay ReadPromoDayFromDatabase(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM DiscountPolicies WHERE ID = " + id, connection))
                {
                    IDataReader dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        PromoDay promoDay = new PromoDay();
                        promoDay.Id = (int)dr["ID"];
                        Percentage percentage = new Percentage((int)dr["DiscountPercentage"]);
                        promoDay.DiscountPercentage = percentage;
                        promoDay.DayDate = (DateTime)dr["DayDate"];
                        return promoDay;
                    }
                    throw new ArgumentException("no id " + id + " found");
                }
            }
        }

        private DiscountUntilExpired ReadDiscountUntilExpiredFromDatabase(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM DiscountPolicies WHERE ID = " + id, connection))
                {
                    IDataReader dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        DiscountUntilExpired discountUntilExpired = new DiscountUntilExpired();
                        discountUntilExpired.Id = (int) dr["ID"];
                        DateTime from = (DateTime) dr["FromTo_Start"];
                        DateTime to = (DateTime) dr["FromTo_End"];
                        discountUntilExpired.FromTo = new DateSpan {StartDate = from, EndDate = to};
                        Percentage percentage = new Percentage((int)dr["DiscountPercentage"]);
                        discountUntilExpired.DiscountPercentage = percentage;
                        return discountUntilExpired;
                    }
                    throw new ArgumentException("no id " + id + " found");
                }
            }
        }
    }
}
