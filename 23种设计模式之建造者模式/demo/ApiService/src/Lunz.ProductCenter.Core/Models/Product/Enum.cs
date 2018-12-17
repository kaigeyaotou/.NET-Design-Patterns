using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Lunz.ProductCenter.Core.Models.Product
{
    public enum AuditState
    {
        /// <summary>
        /// 未提交
        /// </summary>
        Unsubmitted = 0,

        /// <summary>
        /// 审核通过=发布
        /// </summary>
        Passed = 1,

        /// <summary>
        /// 拒绝
        /// </summary>
        Rejected = 2,
    }
}
