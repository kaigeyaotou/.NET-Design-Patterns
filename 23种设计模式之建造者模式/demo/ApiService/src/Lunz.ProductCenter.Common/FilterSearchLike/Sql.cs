using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Common.FilterSearchLike
{
    public class Sql
    {
        public (string Sql, dynamic Parameters) Sql1 { get; set; }

        public string ErrorMsg { get; set; }
    }
}
