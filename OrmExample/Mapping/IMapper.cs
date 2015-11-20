using System.Collections.Generic;

namespace OrmExample.Mapping
{
    public interface IMapper<T> where T : IEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Insert(T entity);
        void Update(T entity);
        void DeleteById(int id);
    }
}