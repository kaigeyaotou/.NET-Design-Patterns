using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.ApiService.QueryStack.Models
{
    public class ProductInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 产品Code
        /// </summary>
        public string ProdCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProdName { get; set; }

        /// <summary>
        /// 产品全称
        /// </summary>
        public string ProdFullName { get; set; }

        /// <summary>
        /// 产品类型Id
        /// </summary>
        public string ProdTypeId { get; set; }

        /// <summary>
        /// 产品Code
        /// </summary>
        public string ProdTypeCode { get; set; }

        /// <summary>
        /// 产品类型名称
        /// </summary>
        public string ProdTypeName { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int? AuditState { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 销售渠道
        /// </summary>
        public string TradeName { get; set; }

        /// <summary>
        /// 销售渠道[]
        /// </summary>
        public string[] TradeNames
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(TradeName))
                {
                    return TradeName.Split(',');
                }

                return new string[] { };
            }
        }

        /// <summary>
        /// 销售渠道Id
        /// </summary>
        public string TradeId { get; set; }

        /// <summary>
        /// 销售渠道id[]
        /// </summary>
        public string[] TradeIds
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(TradeId))
                {
                    return TradeId.Split(',');
                }

                return new string[] { };
            }
        }

        public string[] OriginalTrades
        {
            get
            {
                if (TradeIds != null)
                {
                    return Trades;
                }
                return new string[] { };
            }
        }

        public string[] Trades
        {
            get
            {
                string[] id2NameArr = new string[] { };
                try
                {
                    id2NameArr = new string[TradeIds.Count()];
                    if (TradeIds.Count() == TradeNames.Count())
                    {
                        for (int i = 0; i < TradeIds.Count(); i++)
                        {
                            string id2Name = $"{TradeIds[i]}|||{TradeNames[i]}";
                            id2NameArr[i] = id2Name;
                        }
                    }

                    return id2NameArr;
                }
                catch
                {
                    return id2NameArr;
                }
            }
        }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool? IsDisable { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
