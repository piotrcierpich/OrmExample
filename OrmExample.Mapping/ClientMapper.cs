using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmExample.Mapping
{
    public class ClientMapper
    {
        private readonly DsRetriever dsRetriever;
        private const string QueryString = "SELECT [Id], [Name], [Address] FROM [Clients] WHERE Id = {0}";

        public ClientMapper(DsRetriever dsRetriever)
        {
            this.dsRetriever = dsRetriever;
        }

        public Client GetById(int id)
        {
            string queryCmd = string.Format(QueryString, id);
            DataTable dt = dsRetriever.GetDataForQuery(queryCmd);

            Client client = new Client();
            client.Id = (int) dt.Rows[0][0];
            client.Name = (string) dt.Rows[0][1];
            client.Address = (string) dt.Rows[0][2];
            return client;
        }
    }
}
