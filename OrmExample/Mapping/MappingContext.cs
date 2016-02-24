using System.Configuration;
using System.Data.SqlClient;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class MappingContext
    {
        private readonly ClientMapper clientMapper;
        private readonly ProductMapper productMapper;
        private readonly DiscountMapper discountMapper;

        public MappingContext(string connectionStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            clientMapper = new ClientMapper(connectionString);
            productMapper = new ProductMapper(connectionString);
            discountMapper = new DiscountMapper(connectionString);
        }

        public IMapper<Client> GetClientMapper()
        {
            return clientMapper;
        }

        public IMapper<Product> GetProductMapper()
        {
            return productMapper;
        }

        public void SaveChanges()
        {
            UnitOfWork.Current.Commit();
        }

        public IMapper<Discount> GetDiscountsMapper()
        {
            return discountMapper;
        }
    }
}
