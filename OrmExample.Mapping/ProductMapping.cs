using System.Data.SqlClient;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    class ProductMapping : IMapping
    {
        public IEntity Load(int id, SqlDataReader dataReader)
        {
            Product product = new Product();
            product.Id = id;
            product.Name = (string)dataReader["Name"];
            product.Price = (decimal)dataReader["Price"];
            return product;
        }

        public SqlParameter[] ModifyParameters(IEntity entity)
        {
            Product productEntity = (Product)entity;
            return new[]
                {
                    new SqlParameter("@Name", productEntity.Name), 
                    new SqlParameter("@Price", productEntity.Price) 
                };
        }

        public string[] Columns
        {
            get { return new[] { "Name", "Price" }; }
        }

        public string TableName { get { return "Products"; } }
    }
}