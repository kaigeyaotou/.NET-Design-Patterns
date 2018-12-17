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
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.PropertyManagement.QueryStack.MySql.Repositories
{
    public class ProductDetailRepository : IProductDetailRepository
    {
        #region  const
        private const string _TABLE = "tb_productdetail";
        private const string _TABLE_FILEDS = "Id, ProductId, MaterialId, Quantity";

        private const string _JOIN_FILEDS = "pd.Id, pd.ProductId, pd.MaterialId, pd.Quantity, mi.MateCode, mi.MateName, mi.PropName";

        private const string _VIEW = "vi_materialinfo";

        private readonly IAmbientDatabaseLocator _databaseLocator;

        public ProductDetailRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }
        #endregion

        #region add | update | delete
        public async Task AddAsync(ProductDetail entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder sql = new StringBuilder();
            sql.Append($@" INSERT INTO {_TABLE}
                               (Id, ProductId, MaterialId, Quantity, CreatedById, CreatedAt) 
                           VALUES 
                               (nextval('PD'), @ProductId, @MaterialId, @Quantity, @CreatedById, NOW()) ");

            var result = await db.ExecuteAsync(
                sql.ToString(),
                new
                {
                    entity.ProductId,
                    entity.MaterialId,
                    entity.Quantity,
                    CreatedById = userId,
                },
                tran);
        }

        public async Task UpdateAsync(ProductDetail entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var sql = $@"UPDATE {_TABLE}
                         SET
                             MaterialId= @MaterialId,
                             Quantity= @Quantity,
                             UpdatedById= @UpdatedById,
                             UpdatedAt= NOW()
                         WHERE Id = @Id";
            await db.ExecuteAsync(
                sql,
                new
                {
                    entity.Id,
                    entity.MaterialId,
                    entity.Quantity,
                    UpdatedById = userId,
                },
                tran);
        }

        public async Task DeleteAsync(string[] ids, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var sql = $@"UPDATE {_TABLE}
                         SET Deleted=1,
                             DeletedById=@DeletedById,
                             DeletedAt=NOW()
                         WHERE Id IN @Id";
            await db.ExecuteAsync(sql, new { Id = ids, DeletedById = userId }, tran);
        }

        public async Task DeleteByProductIdAsync(string productId, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var sql = $@"UPDATE {_TABLE}
                         SET Deleted=1,
                             DeletedById=@DeletedById,
                             DeletedAt=NOW()
                         WHERE productId = @ProductId";
            await db.ExecuteAsync(sql, new { ProductId = productId, DeletedById = userId }, tran);
        }
        #endregion

        #region query
        public async Task<IEnumerable<T>> QueryAsync<T>(string productId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string search_sql = $@" SELECT {_JOIN_FILEDS}
                                    FROM {_TABLE} AS pd 
                                    
                                    LEFT JOIN {_VIEW} AS mi 
                                    ON pd.MaterialId=mi.MaterialId
                                    
                                    WHERE ProductID=@ProductID AND Deleted=0";

            return await db.QueryAsync<T>(search_sql, new { ProductID = productId });
        }
        #endregion

    }
}
