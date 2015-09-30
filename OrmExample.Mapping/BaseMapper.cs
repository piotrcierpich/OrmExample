using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OrmExample.Mapping
{
    public abstract class BaseMapper<T> where T : IEntity
    {
        private const string QueryByIdTemplate = "SELECT {0} FROM {1} WHERE Id = @Id";
        private const string GetAllQueryTemplate = "SELECT {0} FROM {1}";
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
            string queryById = string.Format(QueryByIdTemplate, BuildColumnsWithId(), TableName);
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
            string getAllQuery = string.Format(GetAllQueryTemplate, BuildColumnsWithId(), TableName);
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
            command.Parameters.AddRange(ModifyParameters(entity));
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
            command.Parameters.AddRange(ModifyParameters(entity));
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
        protected abstract SqlParameter[] ModifyParameters(T entity);
        protected abstract string TableName { get; }
        protected abstract string[] Columns { get; }
    }
}