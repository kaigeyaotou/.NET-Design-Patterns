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
    public class PropertyRepository : IPropertyRespository
    {
        #region  const
        private const string _TABLE = "tb_properties";
        private const string _VIEW = "vi_properties";
        private const string _VIEW_Fields = " Id,PropName,DisplayName ";
        private const string _Fields = " Id,PropName,DisplayName,IsDisable ";
        private readonly IAmbientDatabaseLocator _databaseLocator;

        public PropertyRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }
        #endregion

        #region   add/update/del

        public async Task AddAsync(Property entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder sql = new StringBuilder();
            sql.Append($" INSERT INTO {_TABLE} ");
            sql.Append("   ( Id, PropName, DisplayName, IsDisable, CreatedById, CreatedAt) ");
            sql.Append($" VALUES ");
            sql.Append("   ( nextval('PR'), @PropName, @DisplayName, @IsDisable, @CreatedById, NOW()) ");

            await db.ExecuteAsync(
                sql.ToString(),
                new
                {
                    entity.PropName,
                    entity.DisplayName,
                    entity.IsDisable,
                    CreatedById = userId,
                }, tran);
        }

        public async Task UpdateAsync(Property entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder updated_params = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(entity.PropName))
            {
                updated_params.Append(" PropName=@PropName, ");
                updated_params.Append(" PropName=@PropName, ");
            }

            if (!string.IsNullOrWhiteSpace(entity.DisplayName))
            {
                updated_params.Append(" DisplayName=@DisplayName, ");
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                updated_params.Append(" UpdatedById=@UpdatedById, ");
            }

            if (entity.IsDisable.HasValue)
            {
                updated_params.Append(" IsDisable=@IsDisable, ");
            }

            if (!string.IsNullOrWhiteSpace(updated_params.ToString()))
            {
                updated_params.Append(" UpdatedAt=NOW() ");
                await db.ExecuteAsync(
                    $" UPDATE {_TABLE} SET {updated_params.ToString()}  WHERE Id=@Id ",
                    new
                    {
                        entity.Id,
                        entity.PropName,
                        entity.DisplayName,
                        entity.IsDisable,
                        UpdatedById = userId,
                    });
            }
        }
        #endregion

        #region enable / disable

        public async Task EnableAsync(string id, string userId)
        {
            Property option = new Property() { Id = id, IsDisable = false };

            await UpdateAsync(option, userId);
        }

        public async Task DisableAsync(string id, string userId)
        {
            Property option = new Property() { Id = id, IsDisable = true, };

            await UpdateAsync(option, userId);
        }
        #endregion

        #region find
        public async Task<Property> FindAsync(string key)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            return await db.QueryFirstOrDefaultAsync<Property>(
               $" SELECT {_Fields} FROM {_TABLE} WHERE Id=@Id AND Deleted = 0 AND IsDisable = 0 LIMIT 1 ",
               new { Id = key });
        }

        public async Task<Property> FindPropertyAsync(string key)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            return await db.QueryFirstOrDefaultAsync<Property>(
               $" SELECT {_Fields} FROM {_TABLE} WHERE Id=@Id AND Deleted = 0 LIMIT 1 ",
               new { Id = key });
        }

        public async Task<bool> CheckdPropNameAsync(string propname, string excludeId = "")
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($" SELECT COUNT(1) FROM {_TABLE} WHERE Id != @Id AND PropName = @PropName ");

            return await db.QueryFirstOrDefaultAsync<bool>(
                sql.ToString(),
                new { PropName = propname, Id = excludeId });
        }
        #endregion

        #region search

        public async Task<(long Count, IEnumerable<Property> Data)> QueryAsync(Func<(string Sql, dynamic Parameters)> filter = null, int? pageIndex = null, int? pageSize = null, string orderBy = null, bool hasCountResult = false)
        {
            return await QueryAsync<Property>(filter, pageIndex, pageSize, orderBy, hasCountResult);
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

            string count_sql = $" SELECT COUNT(1) FROM {_TABLE} {where_order_limit.ToString()}; ";

            if (orderBy != null && orderBy.Count() > 0)
            {
                string order_by = string.Join(" , ", orderBy);
                where_order_limit.Append($" ORDER BY {order_by}");
            }
            else
            {
                where_order_limit.Append($" ORDER BY Id DESC ");
            }

            if (pageIndex.HasValue && pageSize.HasValue
                && pageIndex > 0 && pageSize > 0)
            {
                int take = (pageIndex.Value - 1) * pageSize.Value;
                int limit = pageSize.Value;
                where_order_limit.Append($" LIMIT {take.ToString()} , {limit.ToString()}");
            }

            string search_sql = $" SELECT {_Fields} FROM {_TABLE} {where_order_limit.ToString()}; ";

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

        public async Task<(long Count, IEnumerable<T> Data)> QueryRealityAsync<T>(
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
                    where_order_limit.Append($"WHERE {filterValue.Sql}");
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

            string search_sql = $" SELECT {_VIEW_Fields} FROM {_VIEW} {where_order_limit.ToString()}; ";

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
