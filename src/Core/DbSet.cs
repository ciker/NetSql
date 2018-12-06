using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Td.Fw.Data.Core.Entities;
using Td.Fw.Data.Core.Entities.@internal;
using Td.Fw.Data.Core.Enums;
using Td.Fw.Data.Core.Expressions;
using Td.Fw.Data.Core.Internal;
using Td.Fw.Data.Core.SqlAdapter;
using Td.Fw.Data.Core.SqlQueryable;
using Td.Fw.Data.Core.SqlQueryable.Abstract;
using Td.Fw.Data.Core.SqlQueryable.Impl;

namespace Td.Fw.Data.Core
{
    internal partial class DbSet<TEntity> : IDbSet<TEntity> where TEntity : Entity, new()
    {
        #region ==属性==

        private readonly IDbContext _context;

        private readonly IEntityDescriptor _descriptor;

        private readonly EntitySqlStatement _sqlStatement;

        private readonly ISqlAdapter _sqlAdapter;

        #endregion

        #region ==构造函数==

        public DbSet(ISqlAdapter sqlAdapter, IDbContext context)
        {
            _sqlAdapter = sqlAdapter;
            _context = context;
            _descriptor = EntityCollection.TryGet<TEntity>();
            _sqlStatement = new EntitySqlStatement(_descriptor, sqlAdapter);
        }

        #endregion

        public bool Insert(TEntity entity, IDbTransaction transaction = null)
        {
            Check.NotNull(entity, nameof(entity));

            if (_descriptor.PrimaryKeyType == PrimaryKeyType.Int)
            {
                var sql = _sqlStatement.Insert + _sqlAdapter.IdentitySql;
                var id = ExecuteScalar<int>(sql, entity, transaction);
                if (id > 0)
                {
                    _descriptor.PrimaryKey.SetValue(entity, id);
                    return true;
                }
            }
            else if (_descriptor.PrimaryKeyType == PrimaryKeyType.Long)
            {
                var sql = _sqlStatement.Insert + _sqlAdapter.IdentitySql;
                var id = ExecuteScalar<long>(sql, entity, transaction);
                if (id > 0)
                {
                    _descriptor.PrimaryKey.SetValue(entity, id);
                    return true;
                }
            }
            else if (_descriptor.PrimaryKeyType == PrimaryKeyType.Guid)
            {
                var id = (Guid)_descriptor.PrimaryKey.GetValue(entity);
                if (id == Guid.Empty)
                    _descriptor.PrimaryKey.SetValue(entity, GuidHelper.GenerateGuid());

                return Execute(_sqlStatement.Insert, entity, transaction) > 0;
            }
            else
            {
                return Execute(_sqlStatement.Insert, entity, transaction) > 0;
            }

            return false;
        }

        public bool BatchInsert(List<TEntity> entityList, IDbTransaction transaction = null, int flushSize = 2048)
        {
            if (entityList == null || !entityList.Any())
                return false;

            try
            {
                if (transaction == null)
                    transaction = _context.BeginTransaction();

                var sqlBuilder = new StringBuilder();

                for (var t = 0; t < entityList.Count; t++)
                {
                    var mod = (t + 1) % 1000;
                    if (mod == 1)
                    {
                        sqlBuilder.Clear();
                        sqlBuilder.Append(_sqlStatement.BatchInsert);
                    }

                    var entity = entityList[t];
                    sqlBuilder.Append("(");
                    for (var i = 0; i < _sqlStatement.BatchInsertColumnList.Count; i++)
                    {
                        var col = _sqlStatement.BatchInsertColumnList[i];
#if !NET40
                        var value = col.PropertyInfo.GetValue(entity);
#else
                        var value = col.PropertyInfo.GetValue(entity, new object[] { 0 });
#endif
                        var type = col.PropertyType;

                        ExpressionResolve.AppendValue(sqlBuilder, type, value);

                        if (i < _sqlStatement.BatchInsertColumnList.Count - 1)
                            sqlBuilder.Append(",");
                    }

                    sqlBuilder.Append(")");

                    if (mod > 0 && t < entityList.Count - 1)
                        sqlBuilder.Append(",");
                    else if (mod == 0 || t == entityList.Count - 1)
                    {
                        sqlBuilder.Append(";");
                        Execute(sqlBuilder.ToString(), null, transaction);
                    }
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction?.Rollback();
                throw;
            }
            finally
            {
                transaction?.Connection?.Close();
            }
        }

        public bool Delete(dynamic id, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return Execute(_sqlStatement.DeleteSingle, dynParms, transaction) > 0;
        }

        public bool SoftDelete(dynamic id, dynamic deletor, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);
            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);
            dynParms.Add(_sqlAdapter.AppendParameter("DeletedTime"), DateTime.Now);
            dynParms.Add(_sqlAdapter.AppendParameter("Deletor"), deletor);

            return Execute(_sqlStatement.SoftDeleteSingle, dynParms, transaction) > 0;
        }

        public bool Update(TEntity entity, IDbTransaction transaction = null)
        {
            Check.NotNull(entity, nameof(entity));

            if (_descriptor.PrimaryKeyType == PrimaryKeyType.NoPrimaryKey)
                throw new ArgumentException("没有主键的实体对象无法使用该方法", nameof(entity));

            return Execute(_sqlStatement.UpdateSingle, entity, transaction) > 0;
        }

        public TEntity Get(dynamic id, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return QuerySingleOrDefault<TEntity>(_sqlStatement.Get, dynParms, transaction);
        }

        public int Execute(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).Execute(sql, param, transaction, commandType: commandType);
        }

        public T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).ExecuteScalar<T>(sql, param, transaction, commandType: commandType);
        }

        public T QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).QueryFirstOrDefault<T>(sql, param, transaction, commandType: commandType);
        }

        public T QuerySingleOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).QuerySingleOrDefault<T>(sql, param, transaction, commandType: commandType);
        }

        public IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).Query<T>(sql, param, transaction, commandType: commandType);
        }

        #region ==查询==

        public INetSqlQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression = null, IDbTransaction transaction = null)
        {
            return new NetSqlQueryable<TEntity>(this, _sqlAdapter, _sqlStatement, expression);
        }

        public INetSqlQueryable<TEntity, TEntity2> LeftJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : Entity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2>(this, _sqlAdapter, onExpression);
        }

        public INetSqlQueryable<TEntity, TEntity2> InnerJoin<TEntity2>(Expression<Func<TEntity, TEntity2, bool>> onExpression) where TEntity2 : Entity, new()
        {
            return new NetSqlQueryable<TEntity, TEntity2>(this, _sqlAdapter, onExpression, JoinType.Inner);
        }

        #endregion

        #region ==私有方法==

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private IDbConnection GetCon(IDbTransaction transaction)
        {
            return transaction == null ? _context.DbConnection : transaction.Connection;
        }

        /// <summary>
        /// 主键验证
        /// </summary>
        /// <param name="id"></param>
        private void PrimaryKeyValidate(dynamic id)
        {
            //没有主键的表无法删除单条记录
            if (_descriptor.PrimaryKeyType == PrimaryKeyType.NoPrimaryKey)
                throw new ArgumentException("该实体没有主键，无法使用该方法~");

            //验证id有效性
            if (_descriptor.PrimaryKeyType == PrimaryKeyType.Int || _descriptor.PrimaryKeyType == PrimaryKeyType.Long)
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
