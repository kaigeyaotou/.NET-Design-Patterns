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

namespace Lunz.ProductCenter.PropertyManagement.QueryStack.MySql.Repositories
{
    public class PropertiesOptionRepository : IPropertiesOptionResponsitory
    {
        #region  const
        private const string _TABLE = "tb_propertiesoption";
        private const string _FIELDS = "Id,PropId,OptionName,IsDisable,SortOrder";
        private readonly IAmbientDatabaseLocator _databaseLocator;

        public PropertiesOptionRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }
        #endregion

        #region   add/update/del

        public async Task AddAsync(PropertiesOption entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder sql = new StringBuilder();
            sql.Append($" INSERT INTO {_TABLE} ");
            sql.Append("   (Id,PropId,OptionName,IsDisable,SortOrder, CreatedAt,CreatedById) ");
            sql.Append($" VALUES ");
            sql.Append("   (nextval('PO'),@PropId,@OptionName,@IsDisable,@SortOrder,NOW(),@CreatedById) ");

            await db.ExecuteAsync(
                sql.ToString(),
                new
                {
                    entity.PropId,
                    entity.OptionName,
                    entity.IsDisable,
                    entity.SortOrder,
                    CreatedById = userId,
                },
                tran);
        }

        public async Task UpdateAsync(PropertiesOption entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder updatedParams = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(entity.OptionName))
            {
                updatedParams.Append("OptionName = @OptionName,");
            }

            if (entity.IsDisable.HasValue)
            {
                updatedParams.Append("IsDisable = @IsDisable,");
            }

            if (entity.SortOrder.HasValue)
            {
                updatedParams.Append("SortOrder = @SortOrder,");
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                updatedParams.Append("UpdatedById = @UpdatedById,");
            }

            if (!string.IsNullOrWhiteSpace(updatedParams.ToString()))
            {
                updatedParams.Append("UpdatedAt=NOW()");
                var result = await db.ExecuteAsync(
                    $"UPDATE {_TABLE} SET {updatedParams.ToString()}  WHERE Id=@Id",
                    new
                    {
                        entity.Id,
                        entity.OptionName,
                        entity.IsDisable,
                        entity.SortOrder,
                        UpdatedById = userId,
                    },
                    tran);
            }
        }
        #endregion

        #region enable / disable / up down

        public async Task EnableAsync(string id, string userId)
        {
            PropertiesOption option = new PropertiesOption() { Id = id, IsDisable = false };

            await UpdateAsync(option, userId);
        }

        public async Task DisableAsync(string id, string userId)
        {
            PropertiesOption option = new PropertiesOption() { Id = id, IsDisable = true };

            await UpdateAsync(option, userId);
        }

        public async Task UpOrDownAsync(string id, int sortValue, string userId)
        {
            PropertiesOption up = new PropertiesOption() { Id = id, SortOrder = sortValue };

            await UpdateAsync(up, userId);
        }
        #endregion

        #region find

        public async Task<PropertiesOption> FindByOptionNameAsync(string propId, string optionName)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            return await db.QueryFirstOrDefaultAsync<PropertiesOption>(
               $" SELECT {_FIELDS} FROM {_TABLE} WHERE PropId=@PropId AND OptionName=@OptionName LIMIT 1 ",
               new { PropId = propId, OptionName = optionName });
        }

        public async Task<PropertiesOption> FindAsync(string id)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            return await db.QueryFirstOrDefaultAsync<PropertiesOption>(
               $" SELECT {_FIELDS} FROM {_TABLE} WHERE Id=@Id AND Deleted = 0 AND IsDisable = 0 LIMIT 1 ",
               new { Id = id });
        }

        public async Task<int> MaxSortOrderValue(string propId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var result = await db.QueryFirstOrDefaultAsync<int?>(
              $" SELECT MAX(SortOrder) FROM {_TABLE} WHERE PropId=@PropId ",
              new { PropId = propId });

            return !result.HasValue ? 0 : result.Value;
        }

        public async Task<bool> AllowDisabledOfThisOptionAsync(string propertyId, string thisId = "")
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($@" SELECT COUNT(1) 
                           FROM {_TABLE} 
                           WHERE Id != @Id 
                                 AND PropId=@PropertyId 
                                 AND IsDisable=0 
                                 AND Deleted=0 ");

            var count = await db.QueryFirstOrDefaultAsync<int>(sql.ToString(), new { PropertyId = propertyId, Id = thisId });
            return count > 0;
        }
        #endregion

        #region search
        public async Task<IEnumerable<PropertiesOption>> QueryAsync(
            string propId)
        {
            return await QueryAsync<PropertiesOption>(propId);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(
            string propId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder where_order = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(propId))
            {
                where_order.Append(" WHERE PropId=@PropId ");
            }

            where_order.Append(" ORDER BY SortOrder DESC");

            string sql = $" SELECT {_FIELDS} FROM {_TABLE} {where_order.ToString()} ";

            var result = await db.QueryAsync<T>(sql, new { PropId = propId });
            return result;
        }
        #endregion
    }
}
