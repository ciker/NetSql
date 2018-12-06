#if !NET40

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Td.Fw.Data.Core.Entities;
using Td.Fw.Data.Core.Enums;
using Td.Fw.Data.Core.Expressions;
using Td.Fw.Data.Core.Internal;

namespace Td.Fw.Data.Core
{
    internal partial class DbSet<TEntity> : IDbSet<TEntity> where TEntity : Entity, new()
    {
        public async Task<bool> InsertAsync(TEntity entity, IDbTransaction transaction = null)
        {
            Check.NotNull(entity, nameof(entity));

            if (_descriptor.PrimaryKeyType == PrimaryKeyType.Int)
            {
                var sql = _sqlStatement.Insert + _sqlAdapter.IdentitySql;
                var id = await ExecuteScalarAsync<int>(sql, entity, transaction);
                if (id > 0)
                {
                    _descriptor.PrimaryKey.SetValue(entity, id);
                    return true;
                }
            }
            else if (_descriptor.PrimaryKeyType == PrimaryKeyType.Long)
            {
                var sql = _sqlStatement.Insert + _sqlAdapter.IdentitySql;
                var id = await ExecuteScalarAsync<long>(sql, entity, transaction);
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

                return await ExecuteAsync(_sqlStatement.Insert, entity, transaction) > 0;
            }
            else
            {
                return await ExecuteAsync(_sqlStatement.Insert, entity, transaction) > 0;
            }

            return false;
        }

        public async Task<bool> BatchInsertAsync(List<TEntity> entityList, IDbTransaction transaction = null, int flushSize = 2048)
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
                        var value = col.PropertyInfo.GetValue(entity);
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
                        await ExecuteAsync(sqlBuilder.ToString(), null, transaction);
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

        public async Task<bool> DeleteAsync(dynamic id, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return await ExecuteAsync(_sqlStatement.DeleteSingle, dynParms, transaction) > 0;
        }

        public async Task<bool> SoftDeleteAsync(dynamic id, dynamic deletor, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);
            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);
            dynParms.Add(_sqlAdapter.AppendParameter("DeletedTime"), DateTime.Now);
            dynParms.Add(_sqlAdapter.AppendParameter("Deletor"), deletor);

            return await ExecuteAsync(_sqlStatement.SoftDeleteSingle, dynParms, transaction) > 0;
        }

        public async Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null)
        {
            Check.NotNull(entity, nameof(entity));

            if (_descriptor.PrimaryKeyType == PrimaryKeyType.NoPrimaryKey)
                throw new ArgumentException("没有主键的实体对象无法使用该方法", nameof(entity));

            return await ExecuteAsync(_sqlStatement.UpdateSingle, entity, transaction) > 0;
        }

        public Task<TEntity> GetAsync(dynamic id, IDbTransaction transaction = null)
        {
            PrimaryKeyValidate(id);

            var dynParms = new DynamicParameters();
            dynParms.Add(_sqlAdapter.AppendParameter("Id"), id);

            return QuerySingleOrDefaultAsync<TEntity>(_sqlStatement.Get, dynParms, transaction);
        }

        public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).ExecuteAsync(sql, param, transaction, commandType: commandType);
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).ExecuteScalarAsync<T>(sql, param, transaction, commandType: commandType);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return GetCon(transaction).QueryAsync<T>(sql, param, transaction, commandType: commandType);
        }

    }
}
#endif