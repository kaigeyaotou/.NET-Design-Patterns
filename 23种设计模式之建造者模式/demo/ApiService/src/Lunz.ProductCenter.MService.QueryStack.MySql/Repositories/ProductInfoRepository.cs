using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Lunz.Data;
using Lunz.ProductCenter.ApiService.QueryStack.Models;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Data;
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.PropertyManagement.QueryStack.MySql.Repositories
{
    public class ProductInfoRepository : IProductInfoRepository
    {
        #region  const
        private const string _TABLE = "tb_productinfo";
        private const string _TABLE_FILEDS = "Id, ProdCode, ProdName, ProdFullName, ProdTypeId, ProdTypeName,AuditState, ProdTypeCode, IsDisable";
        private const string _VIEW = "vi_productinfo";
        private const string _VIEW_FILEDS = "Id, ProdCode, ProdName, ProdFullName, ProdTypeId, ProdTypeName, AuditState, TradeName,TradeId, IsDisable";

        private readonly IAmbientDatabaseLocator _databaseLocator;

        public ProductInfoRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }
        #endregion

        #region add | update | delete
        public async Task AddAsync(ProductInfo entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder sql = new StringBuilder();
            sql.Append($" INSERT INTO {_TABLE} ");
            sql.Append("   ( Id, ProdCode, ProdName, ProdFullName, ProdTypeId,");
            sql.Append("     ProdTypeCode, ProdTypeName, AuditState, CreatedById, CreatedAt ) ");
            sql.Append(" VALUES ");
            sql.Append("   ( nextval('PI'), @ProdCode, @ProdName, @ProdFullName, @ProdTypeId,");
            sql.Append("     @ProdTypeCode, @ProdTypeName, 0, @CreatedById, NOW() ); ");
            var result = await db.ExecuteAsync(
                sql.ToString(),
                new
                {
                    entity.ProdCode,
                    entity.ProdName,
                    entity.ProdFullName,
                    entity.ProdTypeId,
                    entity.ProdTypeCode,
                    entity.ProdTypeName,
                    entity.AuditState,
                    CreatedById = userId,
                },
                tran);
        }

        public async Task UpdateAsync(ProductInfo entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder updateParams = new StringBuilder();
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(entity.ProdName))
            {
                updateParams.Append(" ProdName=@ProdName, ");
            }

            if (!string.IsNullOrWhiteSpace(entity.ProdFullName))
            {
                updateParams.Append(" ProdFullName=@ProdFullName, ");
            }

            if (!string.IsNullOrWhiteSpace(entity.ProdTypeId))
            {
                updateParams.Append(" ProdTypeId=@ProdTypeId, ");
            }

            if (!string.IsNullOrWhiteSpace(entity.ProdTypeCode))
            {
                updateParams.Append(" ProdTypeCode=@ProdTypeCode, ");
            }

            if (!string.IsNullOrWhiteSpace(entity.ProdTypeName))
            {
                updateParams.Append(" ProdTypeName=@ProdTypeName, ");
            }

            if (entity.IsDisable.HasValue)
            {
                updateParams.Append(" IsDisable=@IsDisable, ");
            }

            if (entity.Deleted.HasValue)
            {
                updateParams.Append(" Deleted=@Deleted, ");
                updateParams.Append(" DeletedAt=NOW(), ");
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                updateParams.Append(" UpdatedById=@UpdatedById, ");
            }

            if (!string.IsNullOrWhiteSpace(updateParams.ToString()))
            {
                updateParams.Append(" UpdatedAt=NOW() ");
                string sql = $" UPDATE {_TABLE}  SET {updateParams} WHERE Id = @Id ";
                await db.ExecuteAsync(
                    sql,
                    new
                    {
                        entity.Id,
                        entity.ProdName,
                        entity.ProdFullName,
                        entity.ProdTypeId,
                        entity.ProdTypeCode,
                        entity.ProdTypeName,
                        entity.IsDisable,
                        entity.Deleted,
                        UpdatedById = userId,
                    }, tran);
            }
        }

        public async Task DeleteAsync(string id, string userId)
        {
            ProductInfo entity = new ProductInfo() { Id = id, Deleted = true };
            await UpdateAsync(entity, userId);
        }
        #endregion

        #region  enable | disable | release
        public async Task EnableAsync(List<string> ids, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            string sql = $" UPDATE {_TABLE}  SET IsDisable=0,UpdatedById=@UpdatedById,UpdatedAt=NOW() WHERE Id IN @Ids ";

            await db.ExecuteAsync(sql, new { Ids = ids, UpdatedById = userId }, tran);
        }

        public async Task DisableAsync(List<string> ids, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);
            string sql = $" UPDATE {_TABLE}  SET IsDisable=1,UpdatedById=@UpdatedById,UpdatedAt=NOW() WHERE Id IN @Ids ";

            await db.ExecuteAsync(sql, new { Ids = ids, UpdatedById = userId }, tran);
        }

        public async Task ReleaseAsync(string[] ids, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            string sql = $" UPDATE {_TABLE}  SET AuditState=1,UpdatedById=@UpdatedById,UpdatedAt=NOW() WHERE Id IN @Ids ";
            await db.ExecuteAsync(sql, new { Ids = ids, UpdatedById = userId }, tran);
        }
        #endregion

        #region find

        public async Task<ProductInfo> DetailAsync(string id)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($" SELECT {_VIEW_FILEDS} FROM {_VIEW} WHERE Id = @Id LIMIT 1 ");

            var result = await db.QueryFirstOrDefaultAsync<ProductInfo>(sql.ToString(), new { Id = id });
            return result;
        }

        public async Task<ProductInfo> FindAsync(string id)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($" SELECT {_TABLE_FILEDS} FROM {_TABLE} WHERE Id = @Id AND Deleted=0 LIMIT 1 ");

            var result = await db.QueryFirstOrDefaultAsync<ProductInfo>(sql.ToString(), new { Id = id });
            return result;
        }

        public async Task<ProductInfo> FindByProductFullNameAsync(string productFullName)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($" SELECT {_TABLE_FILEDS} FROM {_TABLE} WHERE ProdFullName = @ProdFullName AND Deleted=0 LIMIT 1 ");

            var result = await db.QueryFirstOrDefaultAsync<ProductInfo>(sql.ToString(), new { ProdFullName = productFullName });
            return result;
        }

        public async Task<bool> HasProductFullNameAsync(string productFullName, string excludeId = "")
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string sql = $" SELECT COUNT(1) FROM {_TABLE} WHERE Id != @Id AND ProdFullName = @ProdFullName AND Deleted = 0 ";

            var has = await db.QueryFirstOrDefaultAsync<bool>(sql, new { Id = excludeId, ProdFullName = productFullName });
            return has;
        }

        public async Task<bool> HasProductOfThisTypeAsync(string productTypeId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string sql = $" SELECT COUNT(1) FROM {_TABLE} WHERE ProdTypeId = @ProdTypeId AND Deleted = 0 ";

            var has = await db.QueryFirstOrDefaultAsync<bool>(sql, new { ProdTypeId = productTypeId });
            return has;
        }

        public async Task<string> GetMaxProductCodeAsync()
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($" SELECT MAX(ProdCode) FROM {_TABLE} WHERE Deleted = 0 ");

            return await db.QueryFirstOrDefaultAsync<string>(sql.ToString());
        }

        public async Task<bool> IsReleaseAsync(string id)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            int approvePassed = (int)Core.Models.Product.AuditState.Passed;
            string sql = $" SELECT COUNT(1) FROM {_TABLE} WHERE Id = @Id AND AuditState={approvePassed} AND Deleted=0";
            int result = await db.QueryFirstOrDefaultAsync<int>(sql, new { Id = id });
            return result > 0;
        }
        #endregion

        #region search

        public async Task<(long Count, IEnumerable<ProductInfo> Data)> QueryAsync(
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false)
        {
            return await QueryAsync<ProductInfo>(filter, pageIndex, pageSize, orderBy, hasCountResult);
        }

        public async Task<(long Count, IEnumerable<T> Data)> QueryAsync<T>(
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder where_order_limit = new StringBuilder();
            object parameters = null;

            if (filter != null)
            {
                var filterValue = filter();
                if (!string.IsNullOrWhiteSpace(filterValue.Sql))
                {
                    where_order_limit.Append($" WHERE {filterValue.Sql}");
                    parameters = filterValue.Parameters;
                }
            }

            string count_sql = $" SELECT COUNT(1) FROM {_VIEW} {where_order_limit.ToString()}; ";

            if (orderBy.Trim().ToLower() == "desc"
                || orderBy.Trim().ToLower() == "asc"
                || string.IsNullOrWhiteSpace(orderBy))
            {
                where_order_limit.Append($" ORDER BY Id DESC ");
            }
            else
            {
                string order_by = string.Join(" , ", orderBy);
                where_order_limit.Append($" ORDER BY {order_by}");
            }

            if (pageIndex.HasValue && pageSize.HasValue
                && pageIndex > 0 && pageSize > 0)
            {
                int take = (pageIndex.Value - 1) * pageSize.Value;
                int limit = pageSize.Value;
                where_order_limit.Append($" LIMIT {take.ToString()} , {limit.ToString()}");
            }

            string search_sql = $" SELECT {_VIEW_FILEDS} FROM {_VIEW} {where_order_limit.ToString()}; ";

            long result_count = 0;
            IEnumerable<T> result_data = null;
            string query_sql = hasCountResult ? $"{search_sql}{count_sql}" : search_sql;

            using (var query = await db.QueryMultipleAsync(query_sql, parameters))
            {
                result_data = await query.ReadAsync<T>();
                if (hasCountResult)
                {
                    result_count = await query.ReadSingleAsync<long>();
                }
            }

            return (result_count, result_data);
        }
        #endregion
    }
}
