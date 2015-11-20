using System.Collections.Generic;
using System.Linq;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class ClientMapper : IMapper<Client>
    {
        private readonly EntityMapper mapper;

        public ClientMapper(string connectionString)
        {
            mapper = new EntityMapper(connectionString, new ClientMapping());
        }

        public Client GetById(int id)
        {
            return (Client)mapper.GetById(id);
        }

        public IEnumerable<Client> GetAll()
        {
            return mapper.GetAll().Cast<Client>();
        }

        public void Insert(Client entity)
        {
            mapper.Insert(entity);
        }

        public void Update(Client entity)
        {
            mapper.Update(entity);
        }

        public void DeleteById(int id)
        {
            mapper.DeleteById(id);
        }
    }
}
