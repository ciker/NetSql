using System;
using System.Linq.Expressions;
using Td.Fw.Data.Core.Entities;

namespace Td.Fw.Data.Core.SqlQueryable.Abstract
{
    public interface INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> :
        INetSqlQueryableBase<TEntity, INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>,
            Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, bool>>>
        where TEntity : Entity, new()
        where TEntity2 : Entity, new()
        where TEntity3 : Entity, new()
        where TEntity4 : Entity, new()
        where TEntity5 : Entity, new()
    {

        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> Select<TResult>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TResult>> selectExpression);

        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> OrderBy<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TKey>> expression);

        INetSqlQueryable<TEntity, TEntity2, TEntity3, TEntity4, TEntity5> OrderByDescending<TKey>(Expression<Func<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TKey>> expression);
    }
}
