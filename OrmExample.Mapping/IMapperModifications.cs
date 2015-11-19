namespace OrmExample.Mapping
{


    public interface IMapperModifications<in T> where T : IEntity
    {
        void Insert(T entity);
        void Update(T entity);
        void DeleteById(int id);
    }
}