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
    public class MaterialTypeRepository : IMaterialTypeRepository
    {
        private readonly IAmbientDatabaseLocator _databaseLocator;

        public MaterialTypeRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }

        public async Task<MaterialType> FindAsync(string id)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            return await db.QueryFirstOrDefaultAsync<MaterialType>(
                "SELECT Id, ParentId, TypeCode, TypeName, LevelCode, SortOrder, Deleted FROM tb_materialtype WHERE Deleted=0 AND Id=@Id",
                new { Id = id });
        }

        public async Task<IEnumerable<MaterialType>> QueryAsync()
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            return await db.QueryAsync<MaterialType>(
                "SELECT Id, ParentId, TypeCode, TypeName, LevelCode, SortOrder, Deleted FROM tb_materialtype WHERE Deleted=0");
        }

        public async Task<IEnumerable<MaterialType>> AllAsync()
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            return await db.QueryAsync<MaterialType>(
                "SELECT Id, ParentId, TypeCode, TypeName, LevelCode, SortOrder, Deleted FROM tb_materialtype");
        }

        public async Task<IEnumerable<T>> RootTypesAsync<T>()
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var sql = @"SELECT m.Id,m.ParentId,m.TypeCode,m.TypeName,m.LevelCode,
                               CASE WHEN (SELECT COUNT(c.Id) FROM tb_materialtype AS c WHERE c.Deleted=0 AND c.ParentId=m.Id)>0
		                              THEN 1 ELSE 0
                                    END
                               AS Children
                       FROM tb_materialtype AS m WHERE m.ParentId IS NULL AND m.Deleted=0 ORDER BY m.SortOrder";
            return await db.QueryAsync<T>(sql);
        }

        public async Task<IEnumerable<T>> ChildTypesAsync<T>(string id)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var sql = @"SELECT m.Id,m.ParentId,m.TypeCode,m.TypeName,m.LevelCode,
                         CASE WHEN (SELECT COUNT(c.Id) FROM tb_materialtype AS c WHERE c.Deleted=0 AND c.ParentId=m.Id)>0
		                        THEN 1 ELSE 0
                              END
                         AS Children
                       FROM tb_materialtype AS m WHERE m.ParentId=@Id AND m.Deleted=0 ORDER BY m.SortOrder";
            return await db.QueryAsync<T>(sql, new { Id = id });
        }

        public async Task<IEnumerable<T>> GetPropertiesAsync<T>(string typeId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var sql = @"SELECT a.Id, a.MateTypeId, a.PropId, b.PropName, b.DisplayName AS PropDisplayName, a.PropCreatedAt,
                               a.IsNecessary, a.IsDisable 
                        FROM tb_materialtypeproperties AS a
                        LEFT JOIN tb_properties AS b ON a.PropId=b.Id
                        WHERE a.Deleted=0 AND b.Deleted=0 AND a.MateTypeId=@MateTypeId ORDER BY a.PropCreatedAt DESC";

            return await db.QueryAsync<T>(sql, new { MateTypeId = typeId });
        }

        public async Task<IEnumerable<MaterialTypeProperty>> GetPropertiesAsync(string typeId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var lookup = new Dictionary<string, MaterialTypeProperty>();
            var sql = @"SELECT  b.DisplayName AS PropDisplayName,m.*,t.* FROM tb_materialtypeproperties m LEFT JOIN tb_properties b ON m.PropId= b.id LEFT JOIN tb_propertiesoption t ON t.PropId = m.PropId
                        WHERE m.MateTypeId =@MateTypeId AND m.Deleted = 0 AND m.IsDisable=0";

            await db.QueryAsync<MaterialTypeProperty, PropertyOption, MaterialTypeProperty>(
                sql,
                (m, t) =>
                {
                    MaterialTypeProperty tmp;
                    if (!lookup.TryGetValue(m.Id, out tmp))
                    {
                        tmp = m;
                        lookup.Add(m.Id, tmp);
                    }

                    if (t != null && (!tmp.Options.Any(x => x.Id == t.Id)) && (!t.IsDisable))
                    {
                        tmp.Options.Add(t);
                    }

                    tmp.Options = tmp.Options.OrderByDescending(x => x.SortOrder).ToList();

                    return m;
                },
                new
                {
                    MateTypeId = typeId,
                });

            return lookup.Values.ToList();
        }

        public async Task AddAsync(MaterialType entity)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            await db.ExecuteAsync(
               @"INSERT INTO tb_materialtype (Id, ParentId, TypeCode, TypeName, LevelCode, SortOrder, CreatedById, CreatedAt)
                 VALUES(nextval('MT'), @ParentId, @TypeCode, @TypeName,  @LevelCode, @SortOrder, @CreatedById, NOW())",
               entity,
               tran);
        }

        public async Task AddPropertiesAsync(string typeId, string[] propIds, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var pidstr = string.Join("','", propIds);
            var sql = string.Format(
                @"INSERT INTO tb_materialtypeproperties (Id,MateTypeId,IsNecessary,PropId,PropName,PropCreatedAt,SortOrder,CreatedById,CreatedAt) 
                  SELECT nextval('TM') AS Id,'{0}' AS MateTypeId,0 AS IsNecessary,
                         a.Id AS PropId, a.PropName AS PropName,a.CreatedAt AS PropCreatedAt,
                         (SELECT IFNULL(MAX(SortOrder),0) AS SortOrder FROM tb_materialtypeproperties)+count(*) AS SortOrder,
                         '{2}' AS CreatedById, NOW() AS CreatedAt
                  FROM (SELECT * FROM tb_properties WHERE Id IN ('{1}') 
                          AND Id NOT IN(SELECT PropId FROM tb_materialtypeproperties WHERE Deleted=0 AND MateTypeId='{0}')) AS a,
                       (SELECT * FROM tb_properties WHERE Id IN ('{1}') 
                          AND Id NOT IN(SELECT PropId FROM tb_materialtypeproperties WHERE Deleted=0 AND MateTypeId='{0}')) AS b
                  WHERE a.Id>=b.Id 
                  GROUP BY a.Id",
                typeId,
                pidstr,
                userId);

            await db.ExecuteAsync(sql, tran);
        }

        public async Task SetPropertyStatus(string id, bool isDisable, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            await db.ExecuteAsync(
                "UPDATE tb_materialtypeproperties SET IsDisable=@IsDisable,UpdatedById=@UpdatedById,UpdatedAt=@UpdatedAt WHERE Id=@Id",
                new
                {
                    Id = id,
                    IsDisable = isDisable ? 1 : 0,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.Now,
                }, tran);
        }

        public async Task SetPropertyNecessary(string id, bool isNecessary, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            await db.ExecuteAsync(
                "UPDATE tb_materialtypeproperties SET IsNecessary=@IsNecessary,UpdatedById=@UpdatedById,UpdatedAt=@UpdatedAt WHERE Id=@Id",
                new
                {
                    Id = id,
                    IsNecessary = isNecessary ? 1 : 0,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.Now,
                }, tran);
        }

        public async Task<IEnumerable<T>> SearchByTypeName<T>(string typeName)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            string sql = $@"SELECT Id, ParentId, TypeCode, TypeName, LevelCode, SortOrder
                            FROM tb_materialtype WHERE TypeName LIKE '%{typeName}%' 
                            AND Deleted = 0";
            return await db.QueryAsync<T>(sql);
        }
    }
}
