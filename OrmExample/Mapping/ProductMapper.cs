using System.Collections.Generic;
using System.Linq;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class ProductMapper : IMapper<Product>
    {
        private readonly EntityMapper mapper;

        public ProductMapper(string connectionString)
        {
            mapper = new EntityMapper(connectionString, new ProductMapping());
            MapperRegistry.RegisterMapper(typeof(Product), mapper);
        }

        public Product GetById(int id)
        {
            return (Product)mapper.GetById(id);
        }

        public IEnumerable<Product> GetAll()
        {
            return mapper.GetAll().Cast<Product>();
        }

        public void Insert(Product entity)
        {
            mapper.Insert(entity);
        }

        public void Update(Product entity)
        {
            mapper.Update(entity);
        }

        public void DeleteById(int id)
        {
            mapper.DeleteById(id);
        }
    }
}
