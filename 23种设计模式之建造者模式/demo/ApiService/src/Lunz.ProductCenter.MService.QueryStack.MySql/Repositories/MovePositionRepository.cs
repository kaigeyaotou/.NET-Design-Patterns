using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Lunz.Data;
using Lunz.ProductCenter.Data;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;

namespace Lunz.ProductCenter.MService.QueryStack.MySql.Repositories
{
    public class MovePositionRepository : IMovePositionRepository
    {
        private readonly IAmbientDatabaseLocator _databaseLocator;

        public MovePositionRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }

        /// <summary>
        /// 同级移动
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="moveAfterPosition">移动之后的位置</param>
        /// <param name="moveId">当前Id</param>
        /// <param name="parentId">移动后的ParentId</param>
        /// <param name="tbName">表名</param>
        /// <returns>返回值</returns>
        public async Task<int> MovePosition<T>(int moveAfterPosition, string moveId, string parentId, string tbName)
            where T : MoveBaseModel
        {
            int result = -1;
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var atInfo = await db.QueryMultipleAsync(
                $"SELECT SortOrder FROM {tbName} " +
                "WHERE Id = @Id AND Deleted = 0",
                new
                {
                    Id = moveId,
                },
                tran);
            int? atSortOrder = atInfo.Read<T>().First().SortOrder;

            result = await db.ExecuteAsync(
                $"UPDATE {tbName} SET " +
                "SortOrder = @SortOrder WHERE Id = @Id AND Deleted = 0",
                new
                {
                    SortOrder = moveAfterPosition,
                    Id = moveId,
                },
                tran);

            string symbol = string.Empty;
            int initSort = -1;
            if (moveAfterPosition < atSortOrder)
            {
                symbol = ">=";
            }
            else
            {
                symbol = "<=";
            }

            string sql = $"SELECT Id,SortOrder FROM {tbName} ";
            if (parentId == null || parentId == string.Empty || parentId == "-1")
            {
                sql += $"WHERE ParentId IS NULL AND Deleted = 0 AND SortOrder {symbol} @SortOrder ";
            }
            else
            {
                sql += $"WHERE ParentId = @ParentId AND Deleted = 0 AND SortOrder {symbol} @SortOrder ";
            }

            var updateInfo = await db.QueryMultipleAsync(
                sql +
                "ORDER BY SortOrder ASC ",
                new
                {
                    ParentId = parentId,
                    SortOrder = moveAfterPosition,
                },
                tran);

            if (symbol == "<=")
            {
                moveAfterPosition = initSort;
            }

            foreach (var item in updateInfo.Read<T>())
            {
                if (item.Id != moveId)
                {
                    moveAfterPosition += 1;
                    var resInfo = await db.ExecuteAsync(
                        $"UPDATE {tbName} SET " +
                        "SortOrder = @SortOrder WHERE Id = @Id AND Deleted = 0",
                        new
                        {
                            SortOrder = moveAfterPosition,
                            Id = item.Id,
                        },
                        tran);
                }
            }

            return result;
        }
    }
}
