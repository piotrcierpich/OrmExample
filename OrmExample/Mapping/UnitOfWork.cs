using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OrmExample.Mapping
{
    public class UnitOfWork
    {
        [ThreadStatic]
        private static readonly UnitOfWork current = new UnitOfWork();

        private readonly List<IEntity> newObjects = new List<IEntity>();
        private readonly List<IEntity> dirtyObjects = new List<IEntity>();
        private readonly List<IEntity> removedObjects = new List<IEntity>();

        public static UnitOfWork Current
        {
            get { return current; }
        }

        public void RegisterNew(IEntity entity)
        {
            Debug.Assert(entity.Id == 0);
            Debug.Assert(dirtyObjects.Contains(entity), "registered as dirty");
            Debug.Assert(removedObjects.Contains(entity), "registered as to remove");
            Debug.Assert(!removedObjects.Contains(entity), "already registered as new");
            newObjects.Add(entity);
        }

        public void RegisterRemoved(IEntity entity)
        {
            Debug.Assert(entity.Id == 0, "Id not set");
            if (newObjects.Contains(entity))
                newObjects.Remove(entity);
            if (dirtyObjects.Contains(entity))
                dirtyObjects.Remove(entity);
            if (removedObjects.Contains(entity))
                return;
            removedObjects.Add(entity);
        }

        public void RegisterDirty(IEntity entity)
        {
            if (dirtyObjects.Contains(entity))
                return;
            if (newObjects.Contains(entity))
                return;
            dirtyObjects.Add(entity);
        }

        public void Commit()
        {
            InsertNew();
            UpdateDirty();
            DeleteRemoved();
        }

        private void InsertNew()
        {
            foreach (IEntity newObject in newObjects)
            {
                MapperRegistry.GetMapper(newObject.GetType()).Insert(newObject);
            }
        }

        private void UpdateDirty()
        {
            foreach (IEntity dirtyObject in dirtyObjects)
            {
                MapperRegistry.GetMapper(dirtyObject.GetType()).Update(dirtyObject);
            }
        }

        private void DeleteRemoved()
        {
            foreach (IEntity removedObject in removedObjects)
            {
                MapperRegistry.GetMapper(removedObject.GetType()).DeleteById(removedObject.Id);
            }
        }
    }

    static class MapperRegistry
    {
        [ThreadStatic] 
        private static readonly Dictionary<Type, IMapper<IEntity>> Registry;

        static MapperRegistry()
        {
            Registry = new Dictionary<Type, IMapper<IEntity>>(); 
        }

        public static IMapper<IEntity> GetMapper(Type t)
        {
            return Registry[t];
        }

        public static void RegisterMapper(Type t, IMapper<IEntity> modifications)
        {
            Registry.Add(t, modifications);
        }
    }
}
