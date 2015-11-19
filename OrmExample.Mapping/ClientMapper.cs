using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class ClientMapper : BaseMapper<Client>
    {
        public ClientMapper(string connectionString)
            : base(connectionString, new ClientMapping())
        { }
    }
}
