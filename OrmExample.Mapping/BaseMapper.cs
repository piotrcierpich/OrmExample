using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OrmExample.Mapping
{
    public class BaseMapper<T> where T : IEntity
    {
        private const string QueryByIdTemplate = "SELECT {0} FROM {1} WHERE Id = @Id";
        private const string GetAllQueryTemplate = "SELECT {0} FROM {1}";
        private const string InsertIntoTemplate = "INSERT INTO {0} ({1}) OUTPUT INSERTED.Id VALUES {2}";
        private const string UpdateQueryTemplate = "UPDATE {0} SET {1} WHERE Id = @Id";
        private const string DeleteTemplate = "DELETE FROM {0} WHERE Id = @Id";

        private readonly Dictionary<int, T> identityMap = new Dictionary<int, T>();
        private readonly string connectionString;
        private readonly IMapping mapping;

        protected BaseMapper(string connectionString, IMapping mapping)
        {
            this.connectionString = connectionString;
            this.mapping = mapping;
        }

        public T GetById(int id)
        {
            if (identityMap.ContainsKey(id))
                return identityMap[id];

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string queryById = string.Format(QueryByIdTemplate, BuildColumnsWithId(), mapping.TableName);
            SqlCommand command = new SqlCommand(queryById, connection);
            command.Parameters.Add(new SqlParameter("Id", id));
            using (SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                dataReader.Read();
                T entity = (T)mapping.Load(id, dataReader);
                identityMap[id] = entity;
                return entity;
            }
        }

        public IEnumerable<T> GetAll()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string getAllQuery = string.Format(GetAllQueryTemplate, BuildColumnsWithId(), mapping.TableName);
            SqlCommand command = new SqlCommand(getAllQuery, connection);
            List<T> entities = new List<T>();
            using (SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (dataReader.Read())
                {
                    int id = (int)dataReader["Id"];
                    T entity;
                    if (identityMap.ContainsKey(id))
                    {
                        entity = identityMap[id];
                    }
                    else
                    {
                        entity = (T)mapping.Load(id, dataReader);
                        identityMap.Add(id, entity);
                    }
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public void Insert(T entity)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string insertIntoQuery = string.Format(InsertIntoTemplate, mapping.TableName, BuildColumnsNoId(), BuildValues());
            SqlCommand command = new SqlCommand(insertIntoQuery, connection);
            command.Parameters.AddRange(mapping.ModifyParameters(entity));
            entity.Id = (int)command.ExecuteScalar();
            identityMap.Add(entity.Id, entity);
            connection.Close();
        }

        public void Update(T entity)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string updateQuery = string.Format(UpdateQueryTemplate, mapping.TableName, BuildSet());
            SqlCommand command = new SqlCommand(updateQuery, connection);
            command.Parameters.Add(new SqlParameter("Id", entity.Id));
            command.Parameters.AddRange(mapping.ModifyParameters(entity));
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void DeleteById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string deleteQuery = string.Format(DeleteTemplate, mapping.TableName);
            SqlCommand command = new SqlCommand(deleteQuery, connection);
            command.Parameters.Add(new SqlParameter("Id", id));
            command.ExecuteNonQuery();
            identityMap.Remove(id);
            connection.Close();
        }

        private string BuildColumnsWithId()
        {
            return new[] { "Id" }.Concat(mapping.Columns).Aggregate((col1, col2) => col1 + ", " + col2);
        }

        private string BuildColumnsNoId()
        {
            return mapping.Columns.Aggregate((col1, col2) => col1 + ", " + col2);
        }

        private string BuildValues()
        {
            return "(" + mapping.Columns.Select(col => "@" + col).Aggregate((col1, col2) => col1 + ", " + col2) + ")";
        }

        private string BuildSet()
        {
            return mapping.Columns
                          .Select(col => string.Format("{0} = @{0}", col))
                          .Aggregate((col1, col2) => col1 + ", " + col2);
        }
    }
}