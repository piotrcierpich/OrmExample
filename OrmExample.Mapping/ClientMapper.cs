using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OrmExample.Mapping
{
    public class ClientMapper
    {
        private readonly string connectionString;
        private const string QueryString = "SELECT [Id], [Name], [Address] FROM [Clients] WHERE Id = @Id";

        public ClientMapper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Client GetById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(QueryString, connection);
            command.Parameters.Add(new SqlParameter("Id", id));
            using (SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                dataReader.Read();
                Client client = new Client();
                client.Id = dataReader.GetInt32(0);
                client.Name = dataReader.GetString(1);
                client.Address = dataReader.GetString(2);
                return client;
            }
        }

        private const string getAllQuery = "SELECT [Id], [Name], [Address] FROM [Clients]";

        public IEnumerable<Client> GetAll()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(getAllQuery, connection);
            List<Client> clients = new List<Client>();
            using (SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (dataReader.Read())
                {
                    Client client = new Client();
                    client.Id = dataReader.GetInt32(0);
                    client.Name = dataReader.GetString(1);
                    client.Address = dataReader.GetString(2);
                    clients.Add(client);
                }
            }
            return clients;
        }


        private const string insertInto = "INSERT INTO Clients (Name, Address) OUTPUT INSERTED.Id VALUES (@Name, @Address)";

        public void Insert(Client client)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(insertInto, connection);
            command.Parameters.Add(new SqlParameter("Name", client.Name));
            command.Parameters.Add(new SqlParameter("Address", client.Address));
            client.Id = (int)command.ExecuteScalar();
            connection.Close();
        }

        private const string UpdateQuery = "UPDATE Clients SET Name=@Name, Address=@Address WHERE Id=@Id";

        public void Update(Client client)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(UpdateQuery, connection);
            command.Parameters.Add(new SqlParameter("Name", client.Name));
            command.Parameters.Add(new SqlParameter("Address", client.Address));
            command.Parameters.Add(new SqlParameter("Id", client.Id));
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
