using System.Data.SqlClient;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    internal class DiscountMapping : IMapping
    {
        public IEntity Load(int id, SqlDataReader dataReader)
        {
            Discount discount = new Discount();
            discount.Id = id;
            int productId = (int)dataReader["Product_Id"];
            discount.Product = (Product)MapperRegistry.GetMapper(typeof(Product)).GetById(productId);
            return discount;
        }

        public SqlParameter[] ModifyParameters(IEntity entity)
        {
            Discount discount = (Discount) entity;
            return new[]
                {
                    new SqlParameter("@Product_Id", discount.Product.Id)
                };
        }

        public string[] Columns { get { return new[] { "Product_Id" }; } }
        public string TableName { get { return "Discounts"; } }
    }
}