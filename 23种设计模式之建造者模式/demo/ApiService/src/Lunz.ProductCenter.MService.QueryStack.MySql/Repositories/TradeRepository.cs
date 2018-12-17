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
    public class TradeRepository : ITradeRepository
    {
        #region  const
        private const string _TABLE = "tb_tradeinfo";
        private const string _FILEDS = "Id, ProdMaterId, TradeId, TradeName";

        private readonly IAmbientDatabaseLocator _databaseLocator;

        public TradeRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }
        #endregion

        #region add | update | delete
        public async Task AddAsync(List<Trade> tradeinfos)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            StringBuilder sql = new StringBuilder();
            sql.Append($@" INSERT INTO {_TABLE}   
                                (Id, ProdMaterId, TradeId, TradeName, CreatedAt)  
                           VALUES 
                                (nextval('TI'), @ProdMaterId, @TradeId, @TradeName ,NOW())");

            var result = await db.ExecuteAsync(sql.ToString(), tradeinfos, tran);
        }

        public async Task DeleteByProductIdAsync(string productId, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            string sql = $" UPDATE {_TABLE} SET Deleted = 1, DeletedAt = NOW(),DeletedById = @DeletedById  WHERE ProdMaterId = @ProdMaterId ";
            await db.ExecuteAsync(sql, new { ProdMaterId = productId, DeletedById = userId }, tran);
        }

        public async Task DeleteTradeAsync(string[] ids, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var sql = $"UPDATE {_TABLE}  SET  Deleted=1, DeletedAt=Now()   WHERE Id IN @ids";
            await db.ExecuteAsync(sql, new { Ids = ids }, tran);
        }
        #endregion

        #region query
        public async Task<IEnumerable<T>> QueryAsync<T>(string productId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            string search_sql = $" SELECT {_FILEDS} FROM {_TABLE} WHERE ProdMaterId=@ProductID AND Deleted=0;";

            return await db.QueryAsync<T>(search_sql, new { ProductID = productId });
        }
        #endregion

    }
}
