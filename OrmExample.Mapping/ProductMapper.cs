using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OrmExample.Mapping
{
    public abstract class BaseMapper<T> where T : IEntity
    {
        private const string QueryByIdTemplate = "SELECT {0} FROM Products WHERE Id = @Id";
        private const string GetAllQueryTemplate = "SELECT {0} FROM Products";
        private const string InsertIntoTemplate = "INSERT INTO {0} ({1}) OUTPUT INSERTED.Id VALUES {2}";
        private const string UpdateQueryTemplate = "UPDATE {0} SET {1} WHERE Id = @Id";

        private readonly string connectionString;

        public BaseMapper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public T GetById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string queryById = string.Format(QueryByIdTemplate, BuildColumnsWithId());
            SqlCommand command = new SqlCommand(queryById, connection);
            command.Parameters.Add(new SqlParameter("Id", id));
            using (SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                dataReader.Read();
                return Load(id, dataReader);
            }
        }

        public IEnumerable<T> GetAll()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string getAllQuery = string.Format(GetAllQueryTemplate, BuildColumnsWithId());
            SqlCommand command = new SqlCommand(getAllQuery, connection);
            List<T> entities = new List<T>();
            using (SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (dataReader.Read())
                {
                    T entity = Load((int)dataReader["Id"], dataReader);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public void Insert(T entity)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string insertIntoQuery = string.Format(InsertIntoTemplate, TableName, BuildColumnsNoId(), BuildValues());
            SqlCommand command = new SqlCommand(insertIntoQuery, connection);
            command.Parameters.AddRange(InsertData(entity));
            entity.Id = (int)command.ExecuteScalar();
            connection.Close();
        }

        public void Update(T entity)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string updateQuery = string.Format(UpdateQueryTemplate, TableName, BuildSet());
            SqlCommand command = new SqlCommand(updateQuery, connection);
            command.Parameters.Add(new SqlParameter("Id", entity.Id));
            command.Parameters.AddRange(InsertData(entity));
            command.ExecuteNonQuery();
            connection.Close();
        }

        private string BuildColumnsWithId()
        {
            return new[] { "Id" }.Concat(Columns).Aggregate((col1, col2) => col1 + ", " + col2);
        }

        private string BuildColumnsNoId()
        {
            return Columns.Aggregate((col1, col2) => col1 + ", " + col2);
        }

        private string BuildValues()
        {
            return "(" + Columns.Select(col => "@" + col).Aggregate((col1, col2) => col1 + ", " + col2) + ")";
        }

        private string BuildSet()
        {
            return Columns.Select(col => string.Format("{0} = @{0}", col))
                          .Aggregate((col1, col2) => col1 + ", " + col2);
        }

        protected abstract T Load(int id, SqlDataReader dataReader);
        protected abstract SqlParameter[] InsertData(T entity);
        protected abstract string TableName { get; }
        protected abstract string[] Columns { get; }
    }

    public class ProductMapperConcrete : BaseMapper<Product>
    {
        public ProductMapperConcrete(string connectionString)
            : base(connectionString)
        { }

        protected override Product Load(int id, SqlDataReader dataReader)
        {
            Product product = new Product();
            product.Id = id;
            product.Name = (string)dataReader["Name"];
            product.Price = (decimal)dataReader["Price"];
            return product;
        }

        protected override SqlParameter[] InsertData(Product product)
        {
            return new[]
                {
                    new SqlParameter("@Name", product.Name), 
                    new SqlParameter("@Price", product.Price) 
                };
        }

        protected override string TableName
        {
            get { return "Products"; }
        }

        protected override string[] Columns
        {
            get { return new[] { "Name", "Price" }; }
        }
    }

    public class ProductMapper 
    {
        private readonly string connectionString;
        private const string queryById = "SELECT Id, Name, Price FROM Products WHERE Id = @Id";

        public ProductMapper(string connectionString)
        {
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

        private const string insertInto = "INSERT INTO Products (Name, Price) OUTPUT INSERTED.Id VALUES (@Name, @Price)";

        public void Insert(Product product)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(insertInto, connection);
            command.Parameters.Add(new SqlParameter("Name", product.Name));
            command.Parameters.Add(new SqlParameter("Price", product.Price));
            product.Id = (int)command.ExecuteScalar();
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

        private const string UpdateQuery = "UPDATE Products SET Name = @Name, Price = @Price WHERE Id = @Id";

        public void Update(Product product)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(UpdateQuery, connection);
            command.Parameters.Add(new SqlParameter("Name", product.Name));
            command.Parameters.Add(new SqlParameter("Price", product.Price));
            command.Parameters.Add(new SqlParameter("Id", product.Id));
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
