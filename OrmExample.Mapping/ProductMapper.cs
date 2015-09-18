﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmExample.Mapping
{
    public class ProductMapper
    {
        private readonly DsRetriever dsRetriever;
        private readonly string connectionString;
        private const string queryById = "SELECT Id, Name, Price FROM Products WHERE Id = @Id";

        public ProductMapper(DsRetriever dsRetriever, string connectionString)
        {
            this.dsRetriever = dsRetriever;
            this.connectionString = connectionString;
        }

        public Product GetById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(queryById, connection);
            command.Parameters.Add(new SqlParameter("Id", id));
            using (SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                dataReader.Read();
                Product product = new Product();
                product.Id = dataReader.GetInt32(0);
                product.Name = dataReader.GetString(1);
                product.Price = dataReader.GetDecimal(2);
                return product;
            }
        }

        private const string findByNameQuery = "SELECT Id, Name, Price FROM Products WHERE Name LIKE @Name";

        public IEnumerable<Product> GetByName(string nameAlike)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(findByNameQuery, connection);
            command.Parameters.Add(new SqlParameter("Name", "%" + nameAlike + "%"));
            List<Product> products = new List<Product>();
            using (SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (dataReader.Read())
                {
                    Product product = new Product();
                    product.Id = dataReader.GetInt32(0);
                    product.Name = dataReader.GetString(1);
                    product.Price = dataReader.GetDecimal(2);
                    products.Add(product);
                }
            }
            return products;
        }

        private const string insertInto = "INSERT INTO Products (Name, Price) VALUES (@Name, @Price)";

        public void Insert(Product product)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(insertInto, connection);
            command.Parameters.Add(new SqlParameter("Name", product.Name));
            command.Parameters.Add(new SqlParameter("Price", product.Price));
            command.ExecuteNonQuery();
            connection.Close();
        }

        private const string getAllQuery = "SELECT Id, Name, Price FROM Products";

        public IEnumerable<Product> GetAll()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(getAllQuery, connection);
            List<Product> products = new List<Product>();
            using (SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (dataReader.Read())
                {
                    Product product = new Product();
                    product.Id = dataReader.GetInt32(0);
                    product.Name = dataReader.GetString(1);
                    product.Price = dataReader.GetDecimal(2);
                    products.Add(product);
                }
            }
            return products;
        }
    }
}
