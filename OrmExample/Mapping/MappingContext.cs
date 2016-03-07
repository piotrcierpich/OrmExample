using System.Configuration;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class MappingContext
    {
        private readonly ClientMapper clientMapper;
        private readonly ProductMapper productMapper;
        private readonly DiscountMapper discountMapper;
        private readonly DiscountPoliciesMapper discountPoliciesMapper;

        public MappingContext(string connectionStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            clientMapper = new ClientMapper(connectionString);
            productMapper = new ProductMapper(connectionString);
            discountMapper = new DiscountMapper(connectionString);
            discountPoliciesMapper = new DiscountPoliciesMapper(connectionString);
        }

        public IMapper<Client> ClientMapper
        {
            get { return clientMapper; }
        }

        public IMapper<Product> ProductMapper
        {
            get { return productMapper; }
        }

        public IMapper<Discount> DiscountsMapper
        {
            get { return discountMapper; }
        }

        public IMapper<DiscountPolicyBase> DiscountPolicies
        {
            get { return discountPoliciesMapper; }
        }

        public void SaveChanges()
        {
            UnitOfWork.Current.Commit();
        }
    }
}
