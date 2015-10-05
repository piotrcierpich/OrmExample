using System.Configuration;

namespace OrmExample.Mapping
{
    public class MappingContext
    {
        private readonly ClientMapper clientMapper;
        private readonly ProductMapper productMapper;

        public MappingContext(string connectionStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            clientMapper = new ClientMapper(connectionString);
            productMapper = new ProductMapper(connectionString);
        }

        public ClientMapper GetClientMapper()
        {
            return clientMapper;
        }

        public ProductMapper GetProductMapper()
        {
            return productMapper;
        }
    }
}
