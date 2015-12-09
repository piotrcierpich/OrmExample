using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using OrmExample.Entities;
using OrmExample.Mapping;
using System.Diagnostics;

namespace Orm.Tests
{
    [TestFixture]
    public class ClientMapperTests
    {
        private const string ConnectionStringName = "testConnectionString";//"Data Source=(localdb)\v11.0;Initial Catalog=OrmExampleEf;Integrated Security=true;";
        private MappingContext sut;
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
            sut = new MappingContext(ConnectionStringName);
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
                SqlCommand sqlCommand = new SqlCommand("DELETE FROM Clients", sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        [Test]
        public void SavedInsertShouldSetId()
        {
            Client newClient = new Client
                {
                    Name = "Johnny Bravo",
                    Address = "Czerwone Maki 84, Krakow"
                };
            sut.GetClientMapper().Insert(newClient);
            Assert.AreEqual(0, newClient.Id);
            sut.SaveChanges();
            Assert.AreNotEqual(0, newClient.Id);
        }

        [Test]
        public void SaveShouldInsertOnlyOnce()
        {
            Client newClient = new Client
                {
                Name = "Johnny Bravo",
                Address = "Czerwone Maki 84, Krakow"
            };
            sut.GetClientMapper().Insert(newClient);
            sut.SaveChanges();
            sut.SaveChanges();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM CLIENTS", connection);
                int rowsCount = (int)command.ExecuteScalar();
                Assert.AreEqual(1, rowsCount);
            }
        }

        [Test]
        public void GetAllShouldGetAndMapFromDb()
        {
            InsertClient();
            Client[] allClients = sut.GetClientMapper().GetAll().ToArray();
            Assert.AreEqual("John Doe", allClients[0].Name);
            Assert.AreEqual("Czerwone Maki 84 Krakow", allClients[0].Address);
        }

        [Test]
        public void GetShouldGetAndMapFromDb()
        {
            int insertedId = InsertClient();
            Client insertedClient = sut.GetClientMapper().GetById(insertedId);
            Assert.AreEqual("John Doe", insertedClient.Name);
            Assert.AreEqual("Czerwone Maki 84 Krakow", insertedClient.Address);
        }

        [Test]
        public void ShouldNotQueryTwiceForTheSameClientId()
        {
            int clientId = InsertClient();
            Client client1 = sut.GetClientMapper().GetById(clientId);
            Client client2 = sut.GetClientMapper().GetById(clientId);
            Assert.IsTrue(ReferenceEquals(client1, client2));
        }

        [Test]
        public void ShouldInsertDataOnSaveChanges()
        {
            //given
            Client client = new Client
                {
                    Name = "Jack Daniels",
                    Address = "Rynek 5, Krakow"
                };
            //when
            sut.GetClientMapper().Insert(client);
            sut.SaveChanges();
            //expect
            Client clientInDb = ReadClientFromDb();
            Assert.AreEqual("Jack Daniels", clientInDb.Name);
            Assert.AreEqual("Rynek 5, Krakow", clientInDb.Address);
        }

        private Client ReadClientFromDb()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Name, Address FROM Clients", connection);
                IDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    string name = dr.GetString(0);
                    string address = dr.GetString(1);
                    return new Client
                        {
                            Address = address,
                            Name = name
                        };
                }
                else
                {
                    Assert.Fail("No data found");
                    return null;
                }
            }
        }

        [Test]
        public void ShouldSaveModifications()
        {
            int clientId = InsertClient();
            Client client =  sut.GetClientMapper().GetById(clientId);
            client.Name += " II";
            sut.SaveChanges();
            Client clientInDb = ReadClientFromDb();
            StringAssert.EndsWith(" II", clientInDb.Name); 
        }

        private int InsertClient()
        {
            int insertedId;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand insertCommand = new SqlCommand("INSERT INTO Clients (Name, Address) OUTPUT INSERTED.Id VALUES ('John Doe', 'Czerwone Maki 84 Krakow')",
                                                          sqlConnection);
                sqlConnection.Open();
                insertedId = (int)insertCommand.ExecuteScalar();
            }
            return insertedId;
        }
    }
}
