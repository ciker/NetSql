using System.Linq;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Core.Internal;

namespace NetSql.Core.Entities
{
    public class EntityDescriptorCollection : CollectionAbstract<IEntityDescriptor>, IEntityDescriptorCollection
    {
        private readonly ISqlAdapter _sqlAdapter;
        private readonly IEntitySqlBuilder _sqlBuilder;

        public EntityDescriptorCollection(ISqlAdapter sqlAdapter, IEntitySqlBuilder sqlBuilder)
        {
            _sqlAdapter = sqlAdapter;
            _sqlBuilder = sqlBuilder;
        }

        public IEntityDescriptor Get<TEntity>() where TEntity : IEntity, new()
        {
            var des = Collection.FirstOrDefault(m => m.EntityType == typeof(TEntity));
            if (des == null)
            {
                des = new EntityDescriptor<TEntity>(_sqlAdapter, _sqlBuilder);

                Add(des);
            }

            return des;
        }

    }
}
