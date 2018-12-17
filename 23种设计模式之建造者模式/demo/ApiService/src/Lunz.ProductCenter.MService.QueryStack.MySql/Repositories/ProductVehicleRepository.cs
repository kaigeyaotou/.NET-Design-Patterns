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
    public class ProductVehicleRepository : IProductVehicleRepository
    {
        #region  const
        private const string _TABLE = "tb_productvehicle";
        private const string _Fields = @" Id, ProductId, BrandId, BrandName, ChildBrandId, ChildBrandName,
                                          SeriesId, SeriesName, ModelId, ModelName, CreatedById, CreatedAt,
                                          UpdatedById, UpdatedAt ";

        private readonly IAmbientDatabaseLocator _databaseLocator;

        public ProductVehicleRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }
        #endregion

        #region   add/update/del

        public async Task AddAsync(List<ProductVehicle> entities)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder sql = new StringBuilder();
            sql.Append($" INSERT INTO {_TABLE} ");
            sql.Append(@"   ( Id, VehicleId, ProductId, BrandId, BrandName, ChildBrandId, ChildBrandName,
                              SeriesId, SeriesName, ModelId, ModelName, Year, Displacement, DisplacementText,
                              Horsepower, IntakeType, GearboxName, CreatedAt) ");
            sql.Append($" VALUES ");
            sql.Append(@"   ( nextval('PV'), @VehicleId, @ProductId, @BrandId, @BrandName, @ChildBrandId, @ChildBrandName,
                              @SeriesId, @SeriesName, @ModelId, @ModelName, @Year, @Displacement, @DisplacementText, 
                              @Horsepower, @IntakeType, @GearboxName, NOW()) ");

            await db.ExecuteAsync(sql.ToString(), entities, tran);
        }

        public async Task UpdateAsync(ProductVehicle entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder updated_params = new StringBuilder();

            if (entity.DeletedAt.HasValue)
            {
                updated_params.Append(" Deleted=@Deleted, ");
            }

            if (!string.IsNullOrEmpty(userId))
            {
                updated_params.Append(" DeletedById=@DeletedById, ");
            }

            if (entity.DeletedAt.HasValue)
            {
                updated_params.Append(" DeletedAt=@DeletedAt, ");
            }

            if (!string.IsNullOrWhiteSpace(updated_params.ToString()))
            {
                updated_params.Append(" UpdatedAt=NOW(), ");
                updated_params.Append(" UpdatedById=@UpdatedById ");
                await db.ExecuteAsync(
                    $" UPDATE {_TABLE} SET {updated_params.ToString()}  WHERE Id=@Id ",
                    new
                    {
                        entity.Id,
                        entity.Deleted,
                        entity.DeletedAt,
                        DeletedById = userId,
                        UpdatedById = userId,
                    });
            }
        }

        public async Task DeleteAsync(string id, string userId)
        {
            ProductVehicle productVehicle = new ProductVehicle()
            {
                Id = id,
                Deleted = true,
                DeletedAt = DateTime.Now,
            };
            await UpdateAsync(productVehicle, userId);
        }
        #endregion

        #region search
        public async Task<bool> HasSameModel(string productId, string vehicleId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string sql = $" SELECT COUNT(1) FROM {_TABLE} WHERE VehicleId=@VehicleId AND ProductId=@ProductId AND Deleted=0; ";

            var count = await db.QueryFirstOrDefaultAsync<int>(sql, new { ProductId = productId, VehicleId = vehicleId });
            return count > 0;
        }

        public async Task<bool> HasSameSeries(string productId, string seriesId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string sql = $" SELECT COUNT(1) FROM {_TABLE} WHERE SeriesId=@SeriesId AND ModelName='全车型' AND ProductId=@ProductId AND Deleted=0; ";

            var count = await db.QueryFirstOrDefaultAsync<int>(sql, new { ProductId = productId, SeriesId = seriesId });
            return count > 0;
        }

        public async Task<(long Count, IEnumerable<ProductVehicle> Data)> QueryAsync(Func<(string Sql, dynamic Parameters)> filter = null, int? pageIndex = null, int? pageSize = null, string orderBy = null, bool hasCountResult = false)
        {
            return await QueryAsync<ProductVehicle>(filter, pageIndex, pageSize, orderBy, hasCountResult);
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
                    where_order_limit.Append($"WHERE {filterValue.Sql}");
                    where_order_limit.Append(" AND DEleted =0 ");
                    parameters = filterValue.Parameters;
                }
            }

            if (string.IsNullOrEmpty(where_order_limit.ToString()))
            {
                where_order_limit.Append("WHERE Deleted =0");
            }

            string count_sql = $" SELECT COUNT(1) FROM {_TABLE} {where_order_limit.ToString()}; ";

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

        public async Task<IEnumerable<T>> QueryByProductId<T>(string productId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string sql = $" SELECT {_Fields} FROM {_TABLE} WHERE ProductId=@ProductId AND Deleted=0; ";

            return await db.QueryAsync<T>(sql, new { ProductId = productId });
        }
        #endregion
    }
}
