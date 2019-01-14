﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NetSql.Abstractions;
using NetSql.Abstractions.Entities;
using NetSql.Abstractions.SqlQueryable;
using NetSql.Core.Expressions;
using NetSql.Core.Internal;
using NetSql.Core.SqlQueryable;

namespace NetSql.Core
{
    public class DbSet<TEntity> : IDbSet<TEntity> where TEntity : IEntity, new()
    {
        #region ==属性==

        public IDbContext DbContext { get; }

        public IEntityDescriptor EntityDescriptor { get; }

        #endregion

        #region ==字段==

        private readonly ISqlAdapter _sqlAdapter;

        private readonly EntitySql _sql;

        #endregion

        #region ==构造函数==

        public DbSet(IDbContext context)
        {
            DbContext = context;
            EntityDescriptor = DbContext.Options.EntityDescriptorCollection.Get<TEntity>();
            _sqlAdapter = context.Options.SqlAdapter;
            _sql = EntityDescriptor.Sql;
        }

        #endregion

        #region ==Insert==

        public bool Insert(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));

            if (EntityDescriptor.PrimaryKey.IsInt())
            {
                var sql = _sql.Insert + _sqlAdapter.IdentitySql;
                var id = ExecuteScalar<int>(sql, entity);
                if (id > 0)
                {
                    EntityDescriptor.PrimaryKey.PropertyInfo.SetValue(entity, id);
                    return true;
                }
            }
            else if (EntityDescriptor.PrimaryKey.IsLong())
            {
                var sql = _sql.Insert + _sqlAdapter.IdentitySql;
                var id = ExecuteScalar<long>(sql, entity);
                if (id > 0)
                {
                    EntityDescriptor.PrimaryKey.PropertyInfo.SetValue(entity, id);
                    return true;
                }
            }
            else if (EntityDescriptor.PrimaryKey.IsGuid())
            {
                var id = (Guid)EntityDescriptor.PrimaryKey.PropertyInfo.GetValue(entity);
                if (id == Guid.Empty)
                    EntityDescriptor.PrimaryKey.PropertyInfo.SetValue(entity, GuidGenerator.GenerateTimeBasedGuid());

                return Execute(_sql.Insert, entity) > 0;
            }
            else
            {
                return Execute(_sql.Insert, entity) > 0;
            }

            return false;
        }

        public async Task<bool> InsertAsync(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));

            if (EntityDescriptor.PrimaryKey.IsInt())
            {
                var sql = _sql.Insert + _sqlAdapter.IdentitySql;
                var id = await ExecuteScalarAsync<int>(sql, entity);
                if (id > 0)
                {
                    EntityDescriptor.PrimaryKey.PropertyInfo.SetValue(entity, id);
                    return true;
                }
            }
            else if (EntityDescriptor.PrimaryKey.IsLong())
            {
                var sql = _sql.Insert + _sqlAdapter.IdentitySql;
                var id = await ExecuteScalarAsync<long>(sql, entity);
                if (id > 0)
                {
                    EntityDescriptor.PrimaryKey.PropertyInfo.SetValue(entity, id);
                    return true;
                }
            }
            else if (EntityDescriptor.PrimaryKey.IsGuid())
            {
                var id = (Guid)EntityDescriptor.PrimaryKey.PropertyInfo.GetValue(entity);
                if (id == Guid.Empty)
                    EntityDescriptor.PrimaryKey.PropertyInfo.SetValue(entity, GuidGenerator.GenerateTimeBasedGuid());

                return await ExecuteAsync(_sql.Insert, entity) > 0;
            }
            else
            {
                return await ExecuteAsync(_sql.Insert, entity) > 0;
            }

            return false;
        }

        #endregion

        #region ==BatchInsert==

        public bool BatchInsert(List<TEntity> entityList, int flushSize = 2048)
        {
            if (entityList == null || !entityList.Any())
                return false;

            //判断有没有事务
            var hasTran = DbContext.Transaction != null;
            try
            {
                if (!hasTran)
                    DbContext.BeginTransaction();

                var sqlBuilder = new StringBuilder();

                for (var t = 0; t < entityList.Count; t++)
                {
                    var mod = (t + 1) % 1000;
                    if (mod == 1)
                    {
                        sqlBuilder.Clear();
                        sqlBuilder.Append(_sql.BatchInsert);
                    }

                    var entity = entityList[t];
                    sqlBuilder.Append("(");
                    for (var i = 0; i < _sql.BatchInsertColumnList.Count; i++)
                    {
                        var col = _sql.BatchInsertColumnList[i];
                        var value = col.PropertyInfo.GetValue(entity);
                        var type = col.PropertyInfo.PropertyType;

                        if (col.IsPrimaryKey && EntityDescriptor.PrimaryKey.IsGuid())
                        {
                            if ((Guid)value == Guid.Empty)
                            {
                                value = GuidGenerator.GenerateTimeBasedGuid();
                                EntityDescriptor.PrimaryKey.PropertyInfo.SetValue(entity, value);
                            }
                        }

                        ExpressionResolve.AppendValue(sqlBuilder, type, value);

                        if (i < _sql.BatchInsertColumnList.Count - 1)
                            sqlBuilder.Append(",");
                    }

                    sqlBuilder.Append(")");

                    if (mod > 0 && t < entityList.Count - 1)
                        sqlBuilder.Append(",");
                    else if (mod == 0 || t == entityList.Count - 1)
                    {
                        sqlBuilder.Append(";");
                        Execute(sqlBuilder.ToString());
                    }
                }

                if (!hasTran)
                    DbContext.Transaction.Commit();

                return true;
            }
            catch
            {
                if (!hasTran)
                    DbContext.Transaction.Rollback();

                throw;
            }
        }

        public async Task<bool> BatchInsertAsync(List<TEntity> entityList, int flushSize = 2048)
        {
            if (entityList == null || !entityList.Any())
                return false;

            //判断有没有事务
            var hasTran = DbContext.Transaction != null;
            try
            {
                if (!hasTran)
                    DbContext.BeginTransaction();

                var sqlBuilder = new StringBuilder();

                for (var t = 0; t < entityList.Count; t++)
                {
                    var mod = (t + 1) % 1000;
                    if (mod == 1)
                    {
                        sqlBuilder.Clear();
                        sqlBuilder.Append(_sql.BatchInsert);
                    }

                    var entity = entityList[t];
                    sqlBuilder.Append("(");
                    for (var i = 0; i < _sql.BatchInsertColumnList.Count; i++)
                    {
                        var col = _sql.BatchInsertColumnList[i];
                        var value = col.PropertyInfo.GetValue(entity);
                        var type = col.PropertyInfo.PropertyType;

                        if (col.IsPrimaryKey && EntityDescriptor.PrimaryKey.IsGuid())
                        {
                            if ((Guid)value == Guid.Empty)
                            {
                                value = GuidGenerator.GenerateTimeBasedGuid();
                                EntityDescriptor.PrimaryKey.PropertyInfo.SetValue(entity, value);
                            }
                        }

                        ExpressionResolve.AppendValue(sqlBuilder, type, value);

                        if (i < _sql.BatchInsertColumnList.Count - 1)
                            sqlBuilder.Append(",");
                    }

                    sqlBuilder.Append(")");

                    if (mod > 0 && t < entityList.Count - 1)
                        sqlBuilder.Append(",");
                    else if (mod == 0 || t == entityList.Count - 1)
                    {
                        sqlBuilder.Append(";");
                        await ExecuteAsync(sqlBuilder.ToString());
                    }
                }

                if (!hasTran)
                    DbContext.Transaction.Commit();

                return true;
            }
            catch
            {
                if (!hasTran)
                    DbContext.Transaction.Rollback();

                throw;
            }
        }

        #endregion

        #region ==Delete==

        public bool Delete(dynamic id)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return Execute(_sql.DeleteSingle, dynParms) > 0;
        }

        public async Task<bool> DeleteAsync(dynamic id)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return await ExecuteAsync(_sql.DeleteSingle, dynParms) > 0;
        }

        #endregion

        #region ==SoftDelete==

        public bool SoftDelete(dynamic id, dynamic deletor)
        {
            PrimaryKeyValidate(id);
            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);
            dynParms.Add(_sqlAdapter.AppendParameter("DeletedTime"), DateTime.Now);
            dynParms.Add(_sqlAdapter.AppendParameter("Deletor"), deletor);

            return Execute(_sql.SoftDeleteSingle, dynParms) > 0;
        }

        public async Task<bool> SoftDeleteAsync(dynamic id, dynamic deletor)
        {
            PrimaryKeyValidate(id);
            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);
            dynParms.Add(_sqlAdapter.AppendParameter("DeletedTime"), DateTime.Now);
            dynParms.Add(_sqlAdapter.AppendParameter("Deletor"), deletor);

            return await ExecuteAsync(_sql.SoftDeleteSingle, dynParms) > 0;
        }

        #endregion

        #region ==Update==

        public bool Update(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));

            if (EntityDescriptor.PrimaryKey.IsNo())
                throw new ArgumentException("没有主键的实体对象无法使用该方法", nameof(entity));

            return Execute(_sql.UpdateSingle, entity) > 0;
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));

            if (EntityDescriptor.PrimaryKey.IsNo())
                throw new ArgumentException("没有主键的实体对象无法使用该方法", nameof(entity));

            return await ExecuteAsync(_sql.UpdateSingle, entity) > 0;
        }

        #endregion

        #region ==Get==

        public TEntity Get(dynamic id)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return QuerySingleOrDefault<TEntity>(_sql.Get, dynParms);
        }

        public Task<TEntity> GetAsync(dynamic id)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return QuerySingleOrDefaultAsync<TEntity>(_sql.Get, dynParms);
        }

        #endregion

        #region ==Execute==

        public int Execute(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().Execute(sql, param, DbContext.Transaction, commandType: commandType);
        }

        public Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().ExecuteAsync(sql, param, DbContext.Transaction, commandType: commandType);
        }

        #endregion

        #region ==ExecuteScalar==

        public T ExecuteScalar<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().ExecuteScalar<T>(sql, param, DbContext.Transaction, commandType: commandType);
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().ExecuteScalarAsync<T>(sql, param, DbContext.Transaction, commandType: commandType);
        }

        #endregion

        #region ==QueryFirstOrDefault==

        public T QueryFirstOrDefault<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().QueryFirstOrDefault<T>(sql, param, DbContext.Transaction, commandType: commandType);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().QueryFirstOrDefaultAsync<T>(sql, param, DbContext.Transaction, commandType: commandType);
        }

        #endregion

        #region ==QuerySingleOrDefault==

        public T QuerySingleOrDefault<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().QuerySingleOrDefault<T>(sql, param, DbContext.Transaction, commandType: commandType);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().QuerySingleOrDefaultAsync<T>(sql, param, DbContext.Transaction, commandType: commandType);
        }

        #endregion

        #region ==Query==

        public IEnumerable<T> Query<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().Query<T>(sql, param, DbContext.Transaction, commandType: commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return DbContext.Open().QueryAsync<T>(sql, param, DbContext.Transaction, commandType: commandType);
        }

        public INetSqlQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression = null)
        {
            return new NetSqlQueryable<TEntity>(this, expression);
        }

        #endregion

        #region ==Pagination==

        public INetSqlQueryable<TEntity, TEntity2> LeftJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2>(this, onExpression);
        }

        public INetSqlQueryable<TEntity, TEntity2> InnerJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : IEntity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2>(this, onExpression, JoinType.Inner);
        }

        #endregion

        #region ==私有方法==

        /// <summary>
        /// 主键验证
        /// </summary>
        /// <param name="id"></param>
        private void PrimaryKeyValidate(dynamic id)
        {
            //没有主键的表无法删除单条记录
            if (EntityDescriptor.PrimaryKey.IsNo())
                throw new ArgumentException("该实体没有主键，无法使用该方法~");

            //验证id有效性
            if (EntityDescriptor.PrimaryKey.IsInt() || EntityDescriptor.PrimaryKey.IsLong())
            {
                if (id < 1)
                    throw new ArgumentException("主键不能小于1~");
            }
            else
            {
                if (id == null)
                    throw new ArgumentException("主键不能为空~");
            }
        }

        #endregion
    }
}
