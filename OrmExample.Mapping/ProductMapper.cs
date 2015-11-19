using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class ProductMapper : BaseMapper<Product>
    {
        public ProductMapper(string connectionString)
            : base(connectionString, new ProductMapping())
        { }
    }
}
