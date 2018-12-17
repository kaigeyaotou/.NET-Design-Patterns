using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Lunz.Data;
using Lunz.ProductCenter.Data;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;

namespace Lunz.ProductCenter.MService.QueryStack.MySql.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        #region
        private const string _tradCode = "02";
        #endregion
        private readonly IAmbientDatabaseLocator _databaseLocator;

        public MaterialRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }

        public IQueryable<Material> Items => throw new NotImplementedException();

        public async Task<Material> FindAsync(string id)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            Material material = null;

            var sql = @"SELECT m.*,p.*,t.* FROM tb_materialinfo m
                        LEFT JOIN tb_materialproperties p ON p.MaterialId = m.Id
                        LEFT JOIN tb_tradeinfo t ON t.ProdMaterId = m.Id
                        WHERE m.Id =@Id AND m.Deleted = 0";

            await db.QueryAsync<Material, MaterialProperty, Trade, Material>(
                sql,
                (m, p, t) =>
                {
                    if (material == null)
                    {
                        material = m;
                    }

                    if (p != null && (!material.Properties.Any(x => x.Id == p.Id)))
                    {
                        material.Properties.Add(p);
                    }

                    if (t != null && (!material.Trades.Any(x => x.Id == t.Id)))
                    {
                        material.Trades.Add(t);
                    }

                    return m;
                },
                new
                {
                    Id = id,
                });
            return material;
        }

        public async Task<IEnumerable<Material>> BasicExistAsync<T>(T entity, string id = null)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var lookup = new Dictionary<string, Material>();

            var sql = @"SELECT m.*,p.*,t.* FROM tb_materialinfo m
                        LEFT JOIN tb_materialproperties p ON p.MaterialId=m.Id
                        LEFT JOIN tb_tradeinfo t ON t.ProdMaterId=m.Id
                        WHERE m.Deleted=0 AND MateName=@MateName AND MateTypeId=@MateTypeId AND 
                                    MateTypeCode=@MateTypeCode AND MaterialSpecId=@MaterialSpecId AND 
                                    MaterialUnitId=@MaterialUnitId AND 
                                    IdCodeSingle=@IdCodeSingle AND IsSelfBuild=@IsSelfBuild";

            if (!string.IsNullOrWhiteSpace(id))
            {
                sql = $"{sql} AND m.Id<>@Id";
            }

            await db.QueryAsync<Material, MaterialProperty, Trade, Material>(
                sql,
                (m, p, t) =>
                {
                    Material tmp;
                    if (!lookup.TryGetValue(m.Id, out tmp))
                    {
                        tmp = m;
                        lookup.Add(m.Id, tmp);
                    }

                    if (p != null && (!tmp.Properties.Any(x => x.Id == p.Id)))
                    {
                        tmp.Properties.Add(p);
                    }

                    if (t != null && (!tmp.Trades.Any(x => x.Id == t.Id)))
                    {
                        tmp.Trades.Add(t);
                    }

                    return m;
                }, entity);
            return lookup.Values.ToList();
        }

        public async Task<(long Count, IEnumerable<T> Data)> QueryAsync<T>(
            string materialTypeId = null,
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var sql1 = "SELECT MaterialId,MateCode,MateName,MateTypeName,MateTypeCode,MaterialSpec,IsSelfBuild,IsDisable," +
                       "AuditState,PropName,TradeName,MaterialPrice FROM vi_materialinfo_new ";
            var sql2 = "SELECT COUNT(0) FROM vi_materialinfo_new";

            if (!string.IsNullOrWhiteSpace(materialTypeId))
            {
                var ids = await db.QuerySingleAsync<string>(
                "SELECT (REPLACE(TRIM(LEADING ',' FROM fn_getchildtypebyid_deleted(@MaterialTypeId)),',',\"','\")) AS ids",
                new { MaterialTypeId = materialTypeId });

                sql1 = $"{sql1} WHERE MateTypeId IN(SELECT Id FROM tb_materialtype WHERE Id IN('{ids}'))";
                sql2 = $"{sql2} WHERE MateTypeId IN(SELECT Id FROM tb_materialtype WHERE Id IN('{ids}'))";
            }

            object parameters = null;
            if (filter != null)
            {
                var filterValue = filter();
                if (!string.IsNullOrWhiteSpace(filterValue.Sql))
                {
                    if (!string.IsNullOrWhiteSpace(materialTypeId))
                    {
                        sql1 = $"{sql1} AND ({filterValue.Sql})";
                        sql2 = $"{sql2} AND ({filterValue.Sql})";
                    }
                    else
                    {
                        sql1 = $"{sql1} WHERE {filterValue.Sql}";
                        sql2 = $"{sql2} WHERE {filterValue.Sql}";
                    }

                    parameters = filterValue.Parameters;
                }
            }

            // if (!string.IsNullOrWhiteSpace(orderBy))
            // {
            sql1 = $"{sql1} ORDER BY  mateCode DESC ";

            // }
            if (pageIndex.HasValue && pageSize.HasValue && pageIndex > 0 && pageSize > 0)
            {
                sql1 = $"{sql1} LIMIT {(pageIndex - 1) * pageSize}, {pageSize}";
            }

            long count = 0;
            IEnumerable<T> data = null;
            var sql = hasCountResult ? $"{sql1};{sql2}" : sql1;
            using (var query = await db.QueryMultipleAsync(sql, parameters))
            {
                data = await query.ReadAsync<T>();
                if (hasCountResult)
                {
                    count = await query.ReadSingleAsync<long>();
                }
            }

            return (count, data);
        }

        public async Task AddAsync(Material entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            await db.ExecuteAsync(
               @"INSERT INTO tb_materialinfo (Id, MateCode, MateName, MateTypeId, MateTypeCode, MateTypeName,
                   MaterialSpecId, MaterialSpec, MaterialUnitId, MaterialUnits, MaterialPrice, IdCodeSingle,
                   IsSelfBuild, AuditState, CreatedById, CreatedAt)
                 VALUES(@Id, @MateCode, @MateName, @MateTypeId, @MateTypeCode, @MateTypeName,
                   @MaterialSpecId, @MaterialSpec, @MaterialUnitId, @MaterialUnits, @MaterialPrice,
                   @IdCodeSingle, @IsSelfBuild,0, @CreatedById, NOW())",
               new
               {
                   entity.Id,
                   entity.MateCode,
                   entity.MateName,
                   entity.MateTypeId,
                   entity.MateTypeCode,
                   entity.MateTypeName,
                   entity.MaterialSpecId,
                   entity.MaterialSpec,
                   entity.MaterialUnitId,
                   entity.MaterialUnits,
                   entity.MaterialPrice,
                   entity.IdCodeSingle,
                   entity.IsSelfBuild,
                   CreatedById = userId,
               },
               tran);
        }

        public async Task UpdateAsync(Material entity, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            await db.ExecuteAsync(
               @"UPDATE tb_materialinfo SET MateName=@MateName, MateTypeId=@MateTypeId, MateTypeCode=@MateTypeCode,
                 MateTypeName=@MateTypeName, MaterialSpecId=@MaterialSpecId, MaterialSpec=@MaterialSpec, 
                 MaterialUnitId=@MaterialUnitId, MaterialUnits= @MaterialUnits, MaterialPrice= @MaterialPrice,
                 IdCodeSingle=@IdCodeSingle, IsSelfBuild=@IsSelfBuild, UpdatedById=@UpdatedById, UpdatedAt=NOW() WHERE Id=@Id",
               new
               {
                   entity.Id,
                   entity.MateName,
                   entity.MateTypeId,
                   entity.MateTypeCode,
                   entity.MateTypeName,
                   entity.MaterialSpecId,
                   entity.MaterialSpec,
                   entity.MaterialUnitId,
                   entity.MaterialUnits,
                   entity.MaterialPrice,
                   entity.IdCodeSingle,
                   entity.IsSelfBuild,
                   UpdatedById = userId,
               },
               tran);
        }

        public async Task UpdateAsync(string id, decimal materialPrice, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            await db.ExecuteAsync(
                "UPDATE tb_materialinfo SET MaterialPrice=@MaterialPrice,UpdatedById=@UpdatedById,UpdatedAt=@UpdatedAt WHERE Id=@Id",
                new
                {
                    Id = id,
                    MaterialPrice = materialPrice,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.Now,
                }, tran);
        }

        public async Task DeleteAsync(string id, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            await db.ExecuteAsync(
                "UPDATE tb_materialinfo SET Deleted=1,DeletedById=@DeletedById,DeletedAt=@DeletedAt WHERE Id=@Id",
                new
                {
                    Id = id,
                    DeletedById = userId,
                    DeletedAt = DateTime.Now,
                }, tran);
        }

        public async Task<(string Id, Material Data)> GetIdAndMaxCodeAsync()
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var sql = @"SELECT nextval('MI');
                        SELECT Id, MateCode, MateName, MateTypeId, MateTypeCode, MateTypeName,MaterialSpecId, MaterialSpec,
                          MaterialUnitId, MaterialUnits, MaterialPrice, IdCodeSingle, IsSelfBuild, IsDisable 
                        FROM tb_materialinfo ORDER BY MateCode DESC LIMIT 1";

            string id = string.Empty;
            Material data = null;
            using (var query = await db.QueryMultipleAsync(sql))
            {
                id = await query.ReadSingleAsync<string>();
                data = await query.ReadSingleAsync<Material>();
            }

            return (id, data);
        }

        public async Task AddPropertiesAsync(List<MaterialProperty> entities, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var models = entities.Select(x => new
            {
                x.MaterialId,
                x.PropId,
                x.PropName,
                x.OptionId,
                CreatedById = userId,
                CreatedAt = DateTime.Now,
            }).ToList();

            await db.ExecuteAsync(
               @"INSERT INTO tb_materialproperties (Id, MaterialId, PropId, PropName, OptionId, SortOrder,CreatedById,CreatedAt)
                 VALUES(nextval('MP'), @MaterialId, @PropId, @PropName, @OptionId,
                 (SELECT V.S FROM (SELECT IFNULL(MAX(SortOrder),0)+1 AS S FROM tb_materialproperties)AS V), @CreatedById, @CreatedAt)",
               models,
               tran);
        }

        public async Task UpdatePropertiesAsync(List<MaterialProperty> entities, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var models = entities.Select(x => new
            {
                x.Id,
                x.OptionId,
                UpdatedById = userId,
                UpdatedAt = DateTime.Now,
            }).ToList();

            await db.ExecuteAsync(
               @"UPDATE tb_materialproperties SET OptionId=@OptionId,UpdatedById=@UpdatedById,UpdatedAt=@UpdatedAt WHERE Id=@Id",
               models,
               tran);
        }

        public async Task DeletePropertiesAsync(string[] ids)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var idstr = string.Join("','", ids);

            var sql = $"DELETE FROM tb_materialproperties WHERE Id IN ('{idstr}')";
            await db.ExecuteAsync(sql, tran);
        }

        public async Task AddTradeAsync(List<Trade> entities, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var models = entities.Select(x => new
            {
                x.ProdMaterId,
                x.TradeId,
                x.TradeName,
                CreatedById = userId,
                CreatedAt = DateTime.Now,
            }).ToList();

            await db.ExecuteAsync(
               @"INSERT INTO tb_tradeinfo(Id,ProdMaterId,TradeId,TradeName,CreatedById,CreatedAt)
                 VALUES(nextval('TI'), @ProdMaterId, @TradeId, @TradeName, @CreatedById, @CreatedAt)",
               models,
               tran);
        }

        public async Task DeleteTradeAsync(string[] ids)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var idstr = string.Join("','", ids);

            var sql = $"DELETE FROM tb_tradeinfo WHERE Id IN ('{idstr}')";
            await db.ExecuteAsync(sql, tran);
        }

        public async Task SetStatusAsync(string[] ids, bool isDisable, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var idstr = string.Join("','", ids);

            var sql = $"UPDATE tb_materialinfo SET IsDisable=@IsDisable,UpdatedById=@UpdatedById,UpdatedAt=@UpdatedAt WHERE Id IN ('{idstr}')";
            await db.ExecuteAsync(
                sql,
                new
                {
                    IsDisable = isDisable ? 1 : 0,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.Now,
                }, tran);
        }

        public async Task PublishedAsync(string[] ids, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);
            var sql = @"
                UPDATE tb_materialinfo SET AuditState=1,AuditTime=@AuditTime,UpdatedById=@UpdatedById,UpdatedAt=@UpdatedAt WHERE Id IN @Ids;
                UPDATE tb_materialproperties SET Deleted=1,DeletedAt=@DeletedAt,DeletedById=@DeletedById WHERE MaterialId IN @Ids AND PropId IN (
                  SELECT PropId FROM tb_materialtypeproperties WHERE MateTypeId IN(SELECT MateTypeId FROM tb_materialinfo WHERE Id IN @Ids) AND IsDisable=1)";

            await db.ExecuteAsync(
                sql,
                new
                {
                    Ids = ids,
                    AuditTime = DateTime.Now,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.Now,
                    DeletedById = userId,
                    DeletedAt = DateTime.Now,
                }, tran);
        }

        public async Task<IEnumerable<DataDictionary>> DataDicAsync(bool needAll)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var lookup = new Dictionary<string, DataDictionary>();
            string sql = @"SELECT b.*,v.* FROM basic_datadictionary  AS b
                           LEFT JOIN basic_datadictionary AS v ON v.ParentId=b.Id AND v.Deleted=0
                           WHERE b.ParentId IS NULL AND b.Deleted=0 AND b.Code IN('01','02','03')";

            await db.QueryAsync<DataDictionary, DataDictionary, DataDictionary>(sql, (b, v) =>
            {
                DataDictionary tmp;
                if (!lookup.TryGetValue(b.Id, out tmp))
                {
                    tmp = b;
                    lookup.Add(b.Id, tmp);
                }

                if (needAll)
                {
                    if (v != null && (!tmp.Children.Any(x => x.Id == v.Id)))
                    {
                        tmp.Children.Add(v);
                    }
                }
                else
                {
                    if (v != null && v.Enabled && (!tmp.Children.Any(x => x.Id == v.Id)))
                    {
                        tmp.Children.Add(v);
                    }
                }

                tmp.Children = tmp.Children.OrderBy(x => x.SortOrder).ToList();

                return b;
            });

            return lookup.Values.ToList();
        }

        public async Task<IEnumerable<MaterialTypeProperty>> GetPropertiesAsync(string materialId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            var lookup = new Dictionary<string, MaterialTypeProperty>();
            var sql = @"SELECT  b.`DisplayName` AS PropDisplayName, m.*,t.* FROM tb_materialtypeproperties m  LEFT JOIN tb_properties b ON m.PropId = b.Id LEFT JOIN tb_propertiesoption t ON t.PropId = m.PropId  WHERE m.MateTypeId IN (SELECT MateTypeId FROM tb_materialinfo WHERE Id=@MaterialId) AND 
                              m.PropId IN(SELECT PropId FROM tb_materialproperties WHERE MaterialId=@MaterialId AND deleted =0)";

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

                    if (t != null && (!tmp.Options.Any(x => x.Id == t.Id)))
                    {
                        tmp.Options.Add(t);
                    }

                    tmp.Options = tmp.Options.OrderByDescending(x => x.SortOrder).ToList();

                    return m;
                },
                new
                {
                    MaterialId = materialId,
                });

            return lookup.Values.ToList();
        }

        #region export
        public async Task<IEnumerable<T>> FindMaterialsWithAllPropertiesByTypeAsync<T>(string typeId)
        {
            if (typeId?.Trim().Length > 0)
            {
                var materialDictionary = new Dictionary<string, MaterialExport>();

                using (var db = _databaseLocator.GetProductCenterManagementDatabase())
                {
                    var query = await db.QueryAsync<MaterialExport, MaterialPropertyExport, MaterialExport>(
                        _materialWithAllPropertiesSql,
                        (material, property) =>
                        {
                            MaterialExport entity;
                            if (!materialDictionary.TryGetValue(material.Id, out entity))
                            {
                                materialDictionary.Add(material.Id, entity = material);
                            }

                            if (entity.Properties == null)
                            {
                                entity.Properties = new List<MaterialPropertyExport>();
                            }

                            if (property != null)
                            {
                                if (!entity.Properties.Any(x => x.PropId == property.PropId))
                                {
                                    entity.Properties.Add(property);
                                }
                            }

                            return entity;
                        },
                        new { TypeId = typeId.Trim() },
                        splitOn: "PropId");
                }

                if (materialDictionary?.Values?.Any() ?? false)
                {
                    return materialDictionary.Values.Cast<T>();
                }
            }

            return default(IEnumerable<T>);
        }

        public async Task<IEnumerable<T>> FindMaterialsWithBasicPropertiesByTypeAsync<T>(string typeId)
        {
            if (typeId?.Trim().Length > 0)
            {
                var typeIdsSql = "SELECT fn_getchildtypebyid_deleted(@TypeId) AS ids";
                using (var db = _databaseLocator.GetProductCenterManagementDatabase())
                {
                    var ids = await db.QueryFirstOrDefaultAsync<string>(typeIdsSql, new { TypeId = typeId });

                    if (ids?.Trim().Length > 1)
                    {
                        var idList = ids.Substring(1).Split(',');
                        return await db.QueryAsync<T>(_materialWithBasicPropertiesSql, new { TypeIds = idList });
                    }
                }
            }

            return default(IEnumerable<T>);
        }

        public async Task<MaterialTypeExport> FindCustomPropertiesByTypeAsync<T>(string typeId)
        {
            if (typeId?.Trim().Length > 0)
            {
                var result = new MaterialTypeExport();

                using (var db = _databaseLocator.GetProductCenterManagementDatabase())
                {
                    var query = await db.QueryAsync<MaterialTypeExport, MaterialPropertyExport, PropertyOptionExport, MaterialTypeExport>(
                        _materialTypePropertiesSql,
                        (type, property, option) =>
                        {
                            if (type != null && !type.Id.Equals(result.Id))
                            {
                                result = type;
                            }

                            if (result.Properties == null)
                            {
                                result.Properties = new List<MaterialPropertyExport>();
                            }

                            if (property != null)
                            {
                                if (!result.Properties.Any(x => x.PropId == property.PropId))
                                {
                                    result.Properties.Add(property);
                                }

                                if (option != null)
                                {
                                    var item = result.Properties.Where(i => i.PropId == property.PropId).FirstOrDefault();

                                    if (item != null)
                                    {
                                        if (item.Options == null)
                                        {
                                            item.Options = new List<PropertyOptionExport>();
                                        }

                                        if (!item.Options.Any(o => o.OptionId == option.OptionId))
                                        {
                                            item.Options.Add(option);
                                        }
                                    }
                                }
                            }

                            return result;
                        },
                        new { TypeId = typeId.Trim() },
                        splitOn: "PropId, OptionId");
                }

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public async Task<IEnumerable<T>> FindBasicPropertiesAsync<T>()
        {
            // DT0000000015: 物料单位, DT0000000031: 物料规格
            var sql = @"SELECT ParentId AS PropId, CASE ParentId WHEN 'DT0000000015' THEN '物料单位' ELSE '物料规格' END AS DisplayName, Id AS OptionId, NAME AS OptionName 
                        FROM basic_datadictionary 
                        WHERE Enabled = 1 AND Deleted = 0 AND ParentId IN('DT0000000015', 'DT0000000031') 
                        ORDER BY ParentId DESC, SortOrder";

            var properties = new Dictionary<string, MaterialPropertyExport>();

            using (var db = _databaseLocator.GetProductCenterManagementDatabase())
            {
                var query = await db.QueryAsync<MaterialPropertyExport, PropertyOptionExport, MaterialPropertyExport>(
                        sql,
                        (property, option) =>
                        {
                            MaterialPropertyExport entity;
                            if (!properties.TryGetValue(property.PropId, out entity))
                            {
                                properties.Add(property.PropId, entity = property);
                            }

                            if (entity.Options == null)
                            {
                                entity.Options = new List<PropertyOptionExport>();
                            }

                            if (option != null)
                            {
                                if (!entity.Options.Any(x => x.OptionId == option.OptionId))
                                {
                                    entity.Options.Add(option);
                                }
                            }

                            return entity;
                        },
                        splitOn: "OptionId");
            }

            if (properties?.Values?.Any() ?? false)
            {
                return properties.Values.Cast<T>();
            }

            return default(IEnumerable<T>);
        }

        public async Task<IEnumerable<T>> FindAllTradePropertiesAsync<T>()
        {
            // DT0000000006: 采购主体
            var sql = @"SELECT Id, NAME AS TradeName FROM basic_datadictionary WHERE ParentId = 'DT0000000006' AND Enabled = 1 AND Deleted = 0 ORDER BY SortOrder";

            using (var db = _databaseLocator.GetProductCenterManagementDatabase())
            {
                return await db.QueryAsync<T>(sql);
            }
        }

        public async Task<bool> IsUploadDateTimeLatestAsync(string typeId, string uploadDateTime)
        {
            var sql1 = @"SELECT CreatedAt, UpdatedAt FROM tb_materialtypeproperties 
                        WHERE MateTypeId = @TypeId AND Deleted = 0";

            var sql2 = @"SELECT CreatedAt, UpdatedAt FROM basic_datadictionary 
                        WHERE ParentId IN('DT0000000006', 'DT0000000015', 'DT0000000031') AND Deleted = 0";

            var sql = $"{sql1};{sql2};";
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            using (var query = await db.QueryMultipleAsync(sql, new { TypeId = typeId }))
            {
                var properties = query.Read<MaterialTypeProperty>();
                if (properties?.Any() ?? false)
                {
                    var result = properties.Where(p => (p.CreatedAt != null && p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss").CompareTo(uploadDateTime) > 0)
                                                    || (p.UpdatedAt != null && p.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss").CompareTo(uploadDateTime) > 0));

                    if (result?.Any() ?? false)
                    {
                        return false;
                    }
                }

                var basicdatas = query.Read<Trade>();
                if (basicdatas?.Any() ?? false)
                {
                    var result = basicdatas.Where(p => (p.CreatedAt != null && p.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss").CompareTo(uploadDateTime) > 0)
                                                    || (p.UpdatedAt != null && p.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss").CompareTo(uploadDateTime) > 0));

                    if (result?.Any() ?? false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task AddResourceAsync(ResourceItem resource, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            var sql = @"SELECT nextval('RI');";

            var resourceId = await db.ExecuteScalarAsync(sql);
            if (!string.IsNullOrEmpty(resourceId?.ToString().Trim()))
            {
                resource.Id = resourceId.ToString();
                await db.ExecuteAsync(
                    @"INSERT INTO basic_resourceitem (Id, FileName, FileType, FileLength, FileURL, CreatedById, CreatedAt)
                    VALUES(@Id, @FileName, @FileType, @FileLength, @FileURL, @CreatedById, @CreatedAt);",
                    new
                    {
                        resource.Id,
                        resource.FileName,
                        resource.FileType,
                        resource.FileLength,
                        resource.FileURL,
                        CreatedById = userId,
                        CreatedAt = DateTime.Now,
                    },
                    tran);

                await db.ExecuteAsync(
                    @"INSERT INTO sys_exportlog (ResourceId, FileName, CreatedById, CreatedAt)
                        VALUES(@ResourceId, @FileName, @CreatedById, @CreatedAt);",
                    new
                    {
                        ResourceId = resourceId,
                        resource.FileName,
                        CreatedById = userId,
                        CreatedAt = DateTime.Now,
                    },
                    tran);
            }
        }

        public async Task<(long Count, IEnumerable<T> Data)> FindExcelResourcesAsync<T>(
            Func<(string Sql, dynamic Parameters)> filter = null, int? pageIndex = null, int? pageSize = null, string orderBy = null, bool hasCountResult = false)
        {
            var sql1 = @"SELECT SQL_CALC_FOUND_ROWS Id, FileName, FileType, FileLength, FileURL FROM basic_resourceitem WHERE FileType = '.xlsx' AND FileURL IS NOT NULL AND FileURL != '' AND Deleted = 0";
            var sql2 = "SELECT FOUND_ROWS()";

            object parameters = null;
            if (filter != null)
            {
                var filterValue = filter();
                if (!string.IsNullOrWhiteSpace(filterValue.Sql))
                {
                    sql1 = $"{sql1} AND {filterValue.Sql}";
                    parameters = filterValue.Parameters;
                }
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                sql1 = $"{sql1} ORDER BY {orderBy}";
            }
            else
            {
                sql1 = $"{sql1} ORDER BY Id DESC";
            }

            if (pageIndex.HasValue && pageSize.HasValue && pageIndex > 0 && pageSize > 0)
            {
                sql1 = $"{sql1} LIMIT {(pageIndex - 1) * pageSize}, {pageSize}";
            }

            long count = 0;
            IEnumerable<T> data = null;

            var sql = hasCountResult ? $"{sql1};{sql2};" : sql1;
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            using (var query = await db.QueryMultipleAsync(sql))
            {
                data = await query.ReadAsync<T>();

                if (hasCountResult)
                {
                    count = await query.ReadSingleAsync<long>();
                }
            }

            return (count, data);
        }

        public async Task<(long Count, IEnumerable<T> Data)> QueryForProductAsync<T>(
            string materialTypeId,
            List<string> tradeNames,
            Func<(string Sql, dynamic Parameters)> filter = null,
            int? pageIndex = null,
            int? pageSize = null,
            string orderBy = null,
            bool hasCountResult = false)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();

            // where
            List<StringBuilder> wheres = new List<StringBuilder>();
            object parameters = null;
            if (filter != null)
            {
                var filterValue = filter();
                if (!string.IsNullOrWhiteSpace(filterValue.Sql))
                {
                    wheres.Add(new StringBuilder(filterValue.Sql));
                    parameters = filterValue.Parameters;
                }
            }

            StringBuilder where_tradeNames = new StringBuilder();
            where_tradeNames.Append($" ( ");
            where_tradeNames.Append($"   TradeName ='' OR  TradeName IS NULL ");
            if (tradeNames != null)
            {
                where_tradeNames.Append($"OR TradeName LIKE '%{string.Join("%' OR TradeName LIKE '%", tradeNames)}%'");
            }

            where_tradeNames.Append($" ) ");
            wheres.Add(where_tradeNames);

            if (!string.IsNullOrEmpty(materialTypeId))
            {
                StringBuilder where_materialTypeId = new StringBuilder();
                where_materialTypeId.Append($"MateTypeId = '{materialTypeId}'");
                wheres.Add(where_materialTypeId);
            }

            // like
            StringBuilder where_materialTypeCode = new StringBuilder();
            where_materialTypeCode.Append($"MateTypeCode NOT LIKE '{_tradCode}%'");
            wheres.Add(where_materialTypeCode);

            // count & search
            StringBuilder total = new StringBuilder();
            total.Append("SELECT COUNT(0) FROM vi_materialinfo_new ");

            StringBuilder search = new StringBuilder();
            search.Append(@"SELECT MaterialId,
                                MateCode,
                                MateName,
                                MateTypeName,
                                MaterialSpec,
                                IsSelfBuild,
                                IsDisable,
                                AuditState,
                                PropName,
                                TradeName,
                                MaterialPrice
                         FROM vi_materialinfo_new ");

            string where_sql = string.Join(" AND ", wheres);
            if (!string.IsNullOrEmpty(where_sql))
            {
                total.Append($" WHERE {where_sql} ");
                search.Append($" WHERE {where_sql}");
            }

            search.Append(" ORDER BY  mateCode DESC ");

            // limit
            if (pageIndex.HasValue && pageSize.HasValue && pageIndex > 0 && pageSize > 0)
            {
                search.Append($" LIMIT {(pageIndex - 1) * pageSize}, {pageSize} ");
            }

            // sql
            StringBuilder sql = search.Append(";");
            if (hasCountResult)
            {
                sql.Append(total);
            }

            // query
            long count = 0;
            IEnumerable<T> data = null;
            using (var query = await db.QueryMultipleAsync(sql.ToString(), parameters))
            {
                data = await query.ReadAsync<T>();
                if (hasCountResult)
                {
                    count = await query.ReadSingleAsync<long>();
                }
            }

            return (count, data);
        }

        public async Task<IEnumerable<Material>> ExistNameAsync(string name, string mateTypeId, string materialId)
        {
            string sql = $@"SELECT Id,MateCode,MateName,MateTypeId,MateTypeCode,MateTypeName
                            FROM tb_materialinfo WHERE Deleted = 0 
                            AND MateTypeId = @MateTypeId AND MateName = @MateName
                            ";
            if (materialId != string.Empty)
            {
                sql += "AND Id != @Id";
            }

            using (var db = _databaseLocator.GetProductCenterManagementDatabase())
            {
                return await db.QueryAsync<Material>(
                    sql,
                    new { MateTypeId = mateTypeId, MateName = name, Id = materialId });
            }
        }

        private readonly string _materialWithAllPropertiesSql = @"SELECT material.Id AS Id, material.MateCode AS MateCode, material.MateName AS MateName, 
                                    mtype.TypeName AS TypeName, 
                                    material.MaterialSpec AS MaterialSpec, material.MaterialUnits AS MaterialUnits, material.MaterialPrice AS MaterialPrice, 
                                    material.IdCodeSingle AS IdCodeSingle, material.IsSelfBuild AS IsSelfBuild, 
                                    material.AuditState AS AuditState, material.AuditTime AS AuditTime, 
                                    GROUP_CONCAT(DISTINCT datadictionary.Name ORDER BY datadictionary.SortOrder) AS TradeName, 
                                    material.IsDisable, 
                                    property.Id AS PropId, property.DisplayName AS DisplayName, typepropertymap.IsNecessary AS IsNecessary, 
                                    GROUP_CONCAT(DISTINCT propertyoption.OptionName ORDER BY propertyoption.SortOrder DESC) AS OptionName 
                                    FROM tb_materialinfo material 
                                    LEFT JOIN tb_materialtype mtype ON mtype.Id = material.MateTypeId 
                                    LEFT JOIN tb_materialtypeproperties typepropertymap ON typepropertymap.MateTypeId = material.MateTypeId AND typepropertymap.Deleted = 0 
                                    LEFT JOIN tb_materialproperties materialoptionmap ON materialoptionmap.MaterialId = material.Id AND materialoptionmap.PropId = typepropertymap.PropId AND materialoptionmap.Deleted = 0 
                                    LEFT JOIN tb_properties property ON property.Id = typepropertymap.PropId AND property.Deleted = 0 
                                    LEFT JOIN tb_propertiesoption propertyoption ON propertyoption.Id = materialoptionmap.OptionId AND propertyoption.Deleted = 0 
                                    LEFT JOIN tb_tradeinfo trade ON trade.ProdMaterId = material.Id AND trade.Deleted = 0 
                                    LEFT JOIN basic_datadictionary datadictionary ON datadictionary.Id = trade.TradeId AND datadictionary.Deleted = 0 
                                    WHERE material.Deleted = 0 AND mtype.Deleted = 0 
                                    AND mtype.Id = @TypeId 
                                    GROUP BY material.Id, typepropertymap.PropId 
                                    ORDER BY Id DESC, typepropertymap.PropId DESC;";

        private readonly string _materialWithBasicPropertiesSql = @"SELECT material.Id AS Id, material.MateCode AS MateCode, material.MateName AS MateName, 
                                    mtype.TypeName AS TypeName, 
                                    material.MaterialSpec AS MaterialSpec, material.MaterialUnits AS MaterialUnits, material.MaterialPrice AS MaterialPrice, 
                                    material.IdCodeSingle AS IdCodeSingle, material.IsSelfBuild AS IsSelfBuild, 
                                    material.AuditState AS AuditState, material.AuditTime AS AuditTime, 
                                    GROUP_CONCAT(DISTINCT datadictionary.Name ORDER BY datadictionary.SortOrder) AS TradeName, 
                                    material.IsDisable 
                                    FROM tb_materialinfo material 
                                    LEFT JOIN tb_materialtype mtype ON mtype.Id = material.MateTypeId 
                                    LEFT JOIN tb_tradeinfo trade ON trade.ProdMaterId = material.Id AND trade.Deleted = 0 
                                    LEFT JOIN basic_datadictionary datadictionary ON datadictionary.Id = trade.TradeId AND datadictionary.Deleted = 0 
                                    WHERE material.Deleted = 0 AND mtype.Deleted = 0 
                                    AND mtype.Id IN @TypeIds 
                                    GROUP BY material.Id 
                                    ORDER BY Id DESC;";

        private readonly string _materialTypePropertiesSql = @"SELECT mtype.Id AS Id, mtype.TypeName AS TypeName,
                                    property.Id AS PropId, property.DisplayName AS DisplayName, typepropertymap.IsNecessary AS IsNecessary,
                                    propertyoption.Id AS OptionId, propertyoption.OptionName AS OptionName
                                    FROM tb_materialtype mtype
                                    LEFT JOIN tb_materialtypeproperties typepropertymap ON typepropertymap.MateTypeId = mtype.Id AND typepropertymap.Deleted = 0 AND typepropertymap.IsDisable = 0 
                                    LEFT JOIN tb_properties property ON property.Id = typepropertymap.PropId AND property.Deleted = 0 
                                    LEFT JOIN tb_propertiesoption propertyoption ON propertyoption.PropId = property.Id AND propertyoption.Deleted = 0 AND propertyoption.IsDisable = 0 
                                    WHERE mtype.Deleted = 0
                                    AND mtype.Id = @TypeId 
                                    ORDER BY property.Id DESC, propertyoption.SortOrder DESC";
        #endregion
    }
}
