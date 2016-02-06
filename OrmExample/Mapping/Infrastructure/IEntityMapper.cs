using System.Collections;

namespace OrmExample.Mapping
{
    public interface IEntityMapper
    {
        IEntity GetById(int id);
        IEnumerable GetAll();
        void Insert(IEntity entity);
        void Update(IEntity entity);
        void DeleteById(int id);
    }
}