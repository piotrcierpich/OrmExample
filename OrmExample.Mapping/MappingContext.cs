using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmExample.Mapping
{
    public class MappingContext
    {
        private readonly string connectionString;

        public MappingContext(string connectionStringName)
        {
            connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public ClientMapper GetClientMapper()
        {
            return new ClientMapper(new DsRetriever(connectionString), connectionString);
        }

        public ProductMapper GetProductMapper()
        {
            return new ProductMapper(new DsRetriever(connectionString), connectionString);
        }
    }
}
