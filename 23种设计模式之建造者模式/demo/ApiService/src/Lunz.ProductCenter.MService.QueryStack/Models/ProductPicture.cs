using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    public class ProductPicture : IEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 附件大小(KB)
        /// </summary>
        public int FileLength { get; set; }

        /// <summary>
        /// 附件地址
        /// </summary>
        public string FileURL { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
    }
}
