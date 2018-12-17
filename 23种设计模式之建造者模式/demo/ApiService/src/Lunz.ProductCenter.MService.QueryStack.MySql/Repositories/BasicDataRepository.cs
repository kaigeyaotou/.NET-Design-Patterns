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
    public class BasicDataRepository : IBasicDataRepository
    {
        #region  const
        private const string _TABLE = "basic_datadictionary";
        private const string _Fields = "Id,Code,Name,ParentId,SortOrder,Enabled,CreatedAt";
        private readonly IAmbientDatabaseLocator _databaseLocator;

        public BasicDataRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }
        #endregion

        #region add | update |delete
        public async Task AddAsync(BasicData entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder sql = new StringBuilder();
            sql.Append($" INSERT INTO {_TABLE} ");
            sql.Append("   (Id, Code, Name, ParentId, SortOrder, Enabled, CreatedAt, CreatedById) ");
            sql.Append($" VALUES ");
            sql.Append("   (nextval('DT'), @Code, @Name, @ParentId, @SortOrder, @Enabled, @CreatedAt, @CreatedById) ");

            await db.ExecuteAsync(
                sql.ToString(),
                new
                {
                    entity.Code,
                    entity.Name,
                    entity.ParentId,
                    entity.SortOrder,
                    entity.Enabled,
                    entity.CreatedAt,
                    CreatedById = userId,
                },
                tran);
        }

        public async Task UpdateAsync(BasicData entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder updated_params = new StringBuilder();

            if (entity.Enabled.HasValue)
            {
                updated_params.Append(" Enabled=@Enabled, ");
            }

            if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                updated_params.Append(" Name=@Name, ");
            }

            if (entity.SortOrder.HasValue)
            {
                updated_params.Append(" SortOrder=@SortOrder, ");
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                updated_params.Append(" UpdatedById=@UpdatedById, ");
            }

            if (!string.IsNullOrWhiteSpace(updated_params.ToString()))
            {
                updated_params.Append(" UpdatedAt=NOW() ");
                await db.ExecuteAsync(
                    $" UPDATE {_TABLE} SET {updated_params.ToString()}  WHERE Id=@Id ",
                    new
                    {
                        entity.Id,
                        entity.Name,
                        entity.Enabled,
                        entity.SortOrder,
                        UpdatedById = userId,
                    }, tran);
            }
        }

        public async Task UpdateOptionAsync(BasicData entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder updated_params = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                updated_params.Append(" Name=@Name, ");
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                updated_params.Append(" UpdatedById=@UpdatedById, ");
            }

            if (!string.IsNullOrWhiteSpace(updated_params.ToString()))
            {
                updated_params.Append(" UpdatedAt=NOW() ");

                StringBuilder where = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(entity.Id))
                {
                    where.Append(" Id=@Id AND");
                }

                if (string.IsNullOrEmpty(entity.ParentId))
                {
                    where.Append(" (ParentId IS NULL OR ParentId='') AND");
                }
                else
                {
                    where.Append(" ParentId=@ParentId AND ");
                }

                where.Append(" Deleted = 0");

                await db.ExecuteAsync(
                    $" UPDATE {_TABLE} SET {updated_params.ToString()}  WHERE {where.ToString()} ",
                    new
                    {
                        entity.Id,
                        entity.ParentId,
                        entity.Name,
                        UpdatedById = userId,
                    }, tran);
            }
        }
        #endregion

        #region enable | disable | updown

        public async Task EnableAsync(string id, string userId)
        {
            BasicData option = new BasicData()
            {
                Id = id,
                Enabled = true,
            };

            await UpdateAsync(option, userId);
        }

        public async Task DisableAsync(string id, string userId)
        {
            BasicData option = new BasicData()
            {
                Id = id,
                Enabled = false,
            };

            await UpdateAsync(option, userId);
        }

        public async Task UpOrDownAsync(BasicData entity, string userId)
        {
            await UpdateAsync(entity, userId);
        }
        #endregion

        #region find
        public async Task<BasicData> FindAsync(string id)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            return await db.QueryFirstOrDefaultAsync<BasicData>(
               $" SELECT {_Fields} FROM {_TABLE} WHERE Id=@Id AND Enabled = 1 AND Deleted = 0 LIMIT 1 ",
               new { Id = id });
        }

        public async Task<bool> HasSameChildrenNameAsync(string parentId, string name, string thisId = "")
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            bool has = false;

            StringBuilder where = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(thisId))
            {
                where.Append(" Id != @Id AND");
            }

            if (string.IsNullOrEmpty(parentId))
            {
                where.Append(" (ParentId IS NULL OR ParentId='') AND");
            }
            else
            {
                where.Append(" ParentId=@ParentId AND ");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                where.Append(" Name = @Name AND");
            }

            if (!string.IsNullOrWhiteSpace(where.ToString()))
            {
                string sql = $"SELECT COUNT(1) FROM {_TABLE} WHERE {where.ToString()}  Deleted =0";
                has = await db.QueryFirstOrDefaultAsync<bool>(
                    sql,
                    new { ParentId = parentId, Name = name, Id = thisId });
            }

            return has;
        }

        public async Task<bool> AllowDisabledOfThisOptionAsync(string parentId, string thisId = "")
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($@" SELECT COUNT(1) 
                           FROM {_TABLE} 
                           WHERE Id != @Id 
                                 AND ParentId=@ParentId 
                                 AND Enabled=1 
                                 AND Deleted=0 ");

            var count = await db.QueryFirstOrDefaultAsync<int>(sql.ToString(), new { ParentId = parentId, Id = thisId });
            return count > 0;
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

        public async Task<string> GetMaxCodeValue(string parentId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder sql = new StringBuilder();
            sql.Append($" SELECT MAX(Code) FROM {_TABLE} ");

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
        #endregion

        public async Task<IEnumerable<T>> QueryByParentIdAsync<T>(string parentId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            StringBuilder where_order = new StringBuilder();
            if (parentId == "0")
            {// 顶级
                where_order.Append(" (ParentId IS NULL OR ParentId='') AND");
            }
            else if (!string.IsNullOrWhiteSpace(parentId))
            {// 下级
                where_order.Append(" ParentId = @ParentId AND");
            }

            where_order.Append(" Deleted = 0 ");

            IEnumerable<T> result = null;
            if (!string.IsNullOrWhiteSpace(where_order.ToString()))
            {
                string sql = $"SELECT {_Fields} FROM {_TABLE} WHERE {where_order} ORDER BY SortOrder";
                result = await db.QueryAsync<T>(sql.ToString(), new { ParentId = parentId });
            }

            return result;
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

            where_order_limit.Append("WHERE (ParentId IS NULL OR ParentId ='') AND Deleted = 0 ");

            if (filter != null)
            {
                var filterValue = filter();
                if (!string.IsNullOrWhiteSpace(filterValue.Sql))
                {
                    where_order_limit.Append($" AND {filterValue.Sql}");
                    parameters = filterValue.Parameters;
                }
            }

            string count_sql = $" SELECT COUNT(1) FROM {_TABLE} {where_order_limit.ToString()}; ";

            if (orderBy.Trim().ToLower() == "desc"
                || orderBy.Trim().ToLower() == "asc"
                || string.IsNullOrWhiteSpace(orderBy))
            {
                where_order_limit.Append($" ORDER BY SortOrder DESC ");
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
    }
}
