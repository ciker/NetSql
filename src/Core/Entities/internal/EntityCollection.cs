﻿using System;
using System.Collections.Concurrent;

namespace NetSql.Core.Entities.@internal
{
    internal class EntityCollection
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEntityDescriptor> Descriptors = new ConcurrentDictionary<RuntimeTypeHandle, IEntityDescriptor>();

        public static IEntityDescriptor TryGet<TEntity>() where TEntity : Entity, new()
        {
            var type = typeof(TEntity);
            if (!Descriptors.TryGetValue(type.TypeHandle, out IEntityDescriptor descriptor))
            {
                descriptor = new EntityDescriptor<TEntity>();
                Descriptors[type.TypeHandle] = descriptor;
            }

            return descriptor;
        }

    }
}
