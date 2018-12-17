using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Lunz.Data;
using Lunz.ProductCenter.ApiService.QueryStack.Models;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Data;

namespace Lunz.ProductCenter.PropertyManagement.QueryStack.MySql.Repositories
{
    public class ProducTypeRepository : IProducTypeRepository
    {
        #region  const
        private const string _TABLE = "tb_productype";
        private const string _FIELDS = "Id,ParentId,LevelCode,TypeName,TypeCode,SortOrder";
        private readonly IAmbientDatabaseLocator _databaseLocator;

        public ProducTypeRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }
        #endregion

        #region   add/update/del

        public async Task AddAsync(ProducType entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder sql = new StringBuilder();
            sql.Append($" INSERT INTO {_TABLE} ");
            sql.Append("   (Id, ParentId, LevelCode, TypeName, TypeCode, SortOrder, CreatedById, CreatedAt) ");
            sql.Append($" VALUES ");
            sql.Append("   (nextval('PT'), @ParentId, @LevelCode, @TypeName, @TypeCode, @SortOrder, @CreatedById, NOW()) ");

            await db.ExecuteAsync(sql.ToString(), new { entity.ParentId, entity.LevelCode, entity.TypeCode, entity.TypeName, entity.SortOrder, CreatedById = userId }, tran);
        }

        public async Task UpdateAsync(ProducType entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder updatedParams = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(entity.TypeName))
            {
                updatedParams.Append("TypeName = @TypeName,");
            }

            if (entity.Deleted.HasValue)
            {
                updatedParams.Append("Deleted = @Deleted,");
                updatedParams.Append("DeletedAt = NOW(),");
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                updatedParams.Append("DeletedById = @DeletedById,");
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                updatedParams.Append("UpdatedById = @UpdatedById,");
            }

            if (!string.IsNullOrWhiteSpace(updatedParams.ToString()))
            {
                updatedParams.Append("UpdatedAt = NOW()");
                await db.ExecuteAsync(
                    $"UPDATE {_TABLE} SET {updatedParams.ToString()}  WHERE Id=@Id ",
                    new
                    {
                        entity.Id,
                        entity.TypeName,
                        entity.Deleted,
                        DeletedById = userId,
                        UpdatedById = userId,
                    }, tran);
            }
        }
        #endregion

        #region find
        public async Task<ProducType> FindAsync(string id)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            return await db.QueryFirstOrDefaultAsync<ProducType>(
               $" SELECT {_FIELDS} FROM {_TABLE} WHERE  Id=@Id  AND Deleted=0 LIMIT 1 ",
               new { Id = id });
        }

        public async Task<bool> CheckTheTopTypeName(string typeName, string excludeId = "")
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string sql = $@" SELECT COUNT(1) FROM {_TABLE} 
                             WHERE  Id!=@Id  
                                    AND TypeName = @TypeName  
                                    AND LevelCode=1  
                                    AND Deleted=0 ";

            return await db.QueryFirstOrDefaultAsync<bool>(sql, new { Id = excludeId, TypeName = typeName });
        }

        public async Task<bool> CheckTheSameTypeName(string typeName, string parentId, string excludeId = "")
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string sql = $@" SELECT COUNT(1) FROM {_TABLE} 
                             WHERE  Id!=@Id 
                                    AND ParentId=@ParentId 
                                    AND TypeName = @TypeName 
                                    AND Deleted=0 ";

            return await db.QueryFirstOrDefaultAsync<bool>(
                sql, new { Id = excludeId, ParentId = parentId, TypeName = typeName });
        }

        public async Task<int> GetMaxSortOrderValue(string parentId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($" SELECT MAX(SortOrder) FROM {_TABLE} ");

            if (string.IsNullOrWhiteSpace(parentId))
            {// 顶级
                sql.Append(" WHERE (ParentId IS NULL OR ParentId='') AND Deleted=0 ");
            }
            else
            {// 同级或者下级
                sql.Append(" WHERE ParentId=@ParentId AND Deleted=0 ");
            }

            var max = await db.QueryFirstOrDefaultAsync<int?>(sql.ToString(), new { ParentId = parentId });
            return max.HasValue ? max.Value : 0;
        }

        public async Task<string> GetMaxCode(string parentId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($" SELECT MAX(TypeCode) FROM {_TABLE} ");

            if (string.IsNullOrWhiteSpace(parentId))
            {// 顶级
                sql.Append(" WHERE (ParentId IS NULL OR ParentId='') AND Deleted=0 ");
            }
            else
            {// 同级或者下级
                sql.Append(" WHERE ParentId=@ParentId AND Deleted=0 ");
            }

            var max = await db.QueryFirstOrDefaultAsync<string>(sql.ToString(), new { ParentId = parentId });
            return max;
        }

        public async Task<bool> HasChildren(string parentId, string typeName = "")
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            string sql = $" SELECT COUNT(1) FROM {_TABLE} WHERE ParentId=@ParentId AND Deleted=0 ";
            if (typeName != string.Empty)
            {
                sql += " AND TypeName LIKE @TypeName";
            }

            return await db.QueryFirstOrDefaultAsync<bool>(
               sql,
               new { ParentId = parentId, TypeName = typeName });
        }

        #endregion

        #region search

        public async Task<IEnumerable<T>> QueryAsync<T>(string parentId = "")
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string sql = string.Empty;
            if (parentId == "-1")
            {// 全部
                sql = $"SELECT {_FIELDS} FROM {_TABLE} WHERE Deleted=0 ORDER BY SortOrder ASC";
            }
            else if (parentId == "0")
            {// 顶级
                sql = $"SELECT {_FIELDS} FROM {_TABLE} WHERE ParentId IS NULL AND Deleted=0 ORDER BY SortOrder ASC";
            }
            else
            {// 子级
                sql = $"SELECT {_FIELDS} FROM {_TABLE} WHERE ParentId=@ParentId AND Deleted=0 ORDER BY SortOrder ASC";
            }

            return await db.QueryAsync<T>(sql, new { ParentId = parentId });
        }
        #endregion

        public async Task<IEnumerable<T>> SearchByTypeName<T>(string typeName)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            string sql = $@"SELECT {_FIELDS} FROM {_TABLE} WHERE TypeName LIKE '%{typeName}%' 
                            AND Deleted = 0";
            return await db.QueryAsync<T>(sql);
        }
    }
}
