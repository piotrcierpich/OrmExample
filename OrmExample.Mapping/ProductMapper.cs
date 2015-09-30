using System.Data.SqlClient;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class ProductMapperConcrete : BaseMapper<Product>
    {
        public ProductMapperConcrete(string connectionString)
            : base(connectionString)
        { }

        protected override Product Load(int id, SqlDataReader dataReader)
        {
            Product product = new Product();
            product.Id = id;
            product.Name = (string)dataReader["Name"];
            product.Price = (decimal)dataReader["Price"];
            return product;
        }

        protected override SqlParameter[] ModifyParameters(Product product)
        {
            return new[]
                {
                    new SqlParameter("@Name", product.Name), 
                    new SqlParameter("@Price", product.Price) 
                };
        }

        protected override string TableName
        {
            get { return "Products"; }
        }

        protected override string[] Columns
        {
            get { return new[] { "Name", "Price" }; }
        }
    }
}
