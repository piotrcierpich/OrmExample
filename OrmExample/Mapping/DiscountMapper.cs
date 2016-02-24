using System.Collections.Generic;
using System.Linq;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    class DiscountMapper : IMapper<Discount>
    {
        private readonly EntityMapper mapper;

        public DiscountMapper(string connectionString)
        {
            mapper = new EntityMapper(connectionString, new DiscountMapping());
            MapperRegistry.RegisterMapper(typeof(Discount), mapper);
        }

        public Discount GetById(int id)
        {
            return (Discount)mapper.GetById(id);
        }

        public IEnumerable<Discount> GetAll()
        {
            return mapper.GetAll().Cast<Discount>();
        }

        public void Insert(Discount entity)
        {
            mapper.Insert(entity);
        }

        public void Update(Discount entity)
        {
            mapper.Update(entity);
        }

        public void DeleteById(int id)
        {
            mapper.DeleteById(id);
        }
    }
}