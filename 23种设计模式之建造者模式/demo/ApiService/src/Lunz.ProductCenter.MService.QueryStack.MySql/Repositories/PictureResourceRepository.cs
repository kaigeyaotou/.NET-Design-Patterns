using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Lunz.Data;
using Lunz.ProductCenter.Data;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;

namespace Lunz.ProductCenter.MService.QueryStack.MySql.Repositories
{
    public class PictureResourceRepository : IPictureResourceRepository
    {
        private readonly IAmbientDatabaseLocator _databaseLocator;

        public PictureResourceRepository(IAmbientDatabaseLocator databaseLocator)
        {
            _databaseLocator = databaseLocator;
        }

        public async Task AddPictureResourcesAsync(string productId, List<ProductPicture> pictures, string userId)
        {
            if (pictures != null)
            {
                var db = _databaseLocator.GetProductCenterManagementDatabase();
                var tran = _databaseLocator.GetDbTransaction(db);

                await AddAsync(productId, pictures, userId, db, tran);
            }
        }

        public async Task UpdatePictureResourcesAsync(string productId, List<ProductPicture> pictures, string userId)
        {
            if (pictures?.Any() ?? false)
            {
                var db = _databaseLocator.GetProductCenterManagementDatabase();
                var tran = _databaseLocator.GetDbTransaction(db);

                var updates = pictures.Where(p => !string.IsNullOrEmpty(p.Id)).Select(p => new
                {
                    p.Id,
                    p.Remark,
                    UpdatedById = userId,
                    UpdatedAt = DateTime.Now,
                }).ToList();

                await db.ExecuteAsync(
                   @"UPDATE tb_productresource SET Remark = @Remark, UpdatedById = @UpdatedById, UpdatedAt = @UpdatedAt WHERE ResourceId = @Id",
                   updates,
                   tran);

                var adds = pictures.Where(p => string.IsNullOrEmpty(p.Id)).ToList();
                await AddAsync(productId, adds, userId, db, tran);
            }
        }

        public async Task DeletePictureResourceAsync(string resourceId, string userId)
        {
            var db = _databaseLocator.GetProductCenterManagementDatabase();
            var tran = _databaseLocator.GetDbTransaction(db);

            await db.ExecuteAsync(
                @"UPDATE basic_resourceitem SET Deleted = 1, DeletedById = @DeletedById, DeletedAt = @DeletedAt WHERE Id = @Id",
                new
                {
                    Id = resourceId,
                    DeletedById = userId,
                    DeletedAt = DateTime.Now,
                },
                tran);

            await db.ExecuteAsync(
                @"UPDATE tb_productresource SET Deleted = 1, DeletedById = @DeletedById, DeletedAt = @DeletedAt WHERE ResourceId = @Id",
                new
                {
                    Id = resourceId,
                    DeletedById = userId,
                    DeletedAt = DateTime.Now,
                },
                tran);
        }

        public async Task<IEnumerable<T>> FindPictureResourcesAsync<T>(string productId)
        {
            var sql = @"SELECT productresource.ProductId, productresource.Remark, 
                        resource.Id, resource.FileName, resource.FileType, resource.FileLength, resource.FileURL 
                        FROM tb_productresource productresource 
                        JOIN basic_resourceitem resource ON resource.Id = productresource.ResourceId AND FileURL IS NOT NULL AND FileURL != '' AND resource.Deleted = 0 
                        WHERE productresource.Deleted = 0 
                        AND productresource.ProductId = @ProductId
                        ORDER BY productresource.ProductId DESC, resource.Id DESC";

            using (var db = _databaseLocator.GetProductCenterManagementDatabase())
            {
                return await db.QueryAsync<T>(sql, new { ProductId = productId });
            }
        }

        private async Task AddAsync(string productId, List<ProductPicture> pictures, string userId, DbConnection db, DbTransaction tran)
        {
            var sql = @"SELECT nextval('RI');";
            foreach (ProductPicture picture in pictures)
            {
                var resourceId = await db.ExecuteScalarAsync(sql);
                if (!string.IsNullOrEmpty(resourceId?.ToString().Trim()))
                {
                    picture.Id = resourceId.ToString();
                    await db.ExecuteAsync(
                        @"INSERT INTO basic_resourceitem (Id, FileName, FileType, FileLength, FileURL, CreatedById, CreatedAt)
                        VALUES(@Id, @FileName, @FileType, @FileLength, @FileURL, @CreatedById, @CreatedAt);",
                        new
                        {
                            picture.Id,
                            picture.FileName,
                            picture.FileType,
                            picture.FileLength,
                            picture.FileURL,
                            CreatedById = userId,
                            CreatedAt = DateTime.Now,
                        },
                        tran);

                    await db.ExecuteAsync(
                        @"INSERT INTO tb_productresource (ProductId, ResourceId, Remark, CreatedById, CreatedAt)
                            VALUES(@ProductId, @ResourceId, @Remark, @CreatedById, @CreatedAt);",
                        new
                        {
                            ProductId = productId,
                            ResourceId = resourceId,
                            picture.Remark,
                            CreatedById = userId,
                            CreatedAt = DateTime.Now,
                        },
                        tran);
                }
            }
        }
    }
}
