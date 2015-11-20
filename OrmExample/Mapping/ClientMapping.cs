using System.Data.SqlClient;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    class ClientMapping : IMapping
    {
        public IEntity Load(int id, SqlDataReader dataReader)
        {
            Client client = new Client();
            client.Id = id;
            client.Address = (string)dataReader["Address"];
            client.Name = (string)dataReader["Name"];
            return client;
        }

        public SqlParameter[] ModifyParameters(IEntity entity)
        {
            Client clientEntity = (Client) entity;
            return new[]
                {
                    new SqlParameter("Name", clientEntity.Name),
                    new SqlParameter("Address", clientEntity.Address)
                };
        }

        public string[] Columns
        {
            get { return new[] { "Address", "Name" }; }
        }

        public string TableName
        {
            get { return "Clients"; }
        }
    }
}