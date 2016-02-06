using System;
using System.Collections.Generic;

namespace OrmExample.Mapping
{
    static class MapperRegistry
    {
        [ThreadStatic]
        private static readonly Dictionary<Type, IEntityMapper> Registry;

        static MapperRegistry()
        {
            Registry = new Dictionary<Type, IEntityMapper>();
        }

        public static IEntityMapper GetMapper(Type t)
        {
            return Registry[t];
        }

        public static void RegisterMapper(Type t, IEntityMapper modifications)
        {
            Registry[t] = modifications;
        }
    }
}