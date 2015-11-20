namespace OrmExample.Mapping
{
    public abstract class EntityInUow : IEntity
    {   
        protected void MarkNew()
        {
            UnitOfWork.Current.RegisterNew(this);
        }

        protected void MarkDirty()
        {
            UnitOfWork.Current.RegisterDirty(this);
        }

        protected void MarkRemoved()
        {
            UnitOfWork.Current.RegisterRemoved(this);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public int Id { get; set; }
    }
}