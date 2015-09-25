using System.Configuration;

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
            return new ClientMapper(connectionString);
        }

        public ProductMapper GetProductMapper()
        {
            return new ProductMapper(connectionString);
        }
    }
}
