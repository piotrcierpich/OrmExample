using System.Data.SqlClient;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class ClientMapper : BaseMapper<Client>
    {
        public ClientMapper(string connectionString)
            : base(connectionString)
        {
        }

        protected override Client Load(int id, SqlDataReader dataReader)
        {
            Client client = new Client();
            client.Id = id;
            client.Address = (string)dataReader["Address"];
            client.Name = (string)dataReader["Name"];
            return client;
        }

        protected override SqlParameter[] ModifyParameters(Client entity)
        {
            return new[]
                {
                    new SqlParameter("Name", entity.Name),
                    new SqlParameter("Address", entity.Address)
                };
        }

        protected override string TableName
        {
            get { return "Clients"; }
        }

        protected override string[] Columns
        {
            get { return new[] { "Address", "Name" }; }
        }
    }
}
