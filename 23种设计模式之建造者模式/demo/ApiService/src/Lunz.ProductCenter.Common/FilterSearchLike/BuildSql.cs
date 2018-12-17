using System;
using System.Collections.Generic;
using System.Text;
using Lunz.Data.Extensions.Query;

namespace Lunz.ProductCenter.Common.FilterSearchLike
{
    public abstract class BuildSql
    {
        public abstract (string likeSql, int field, string errorMsg) CreateSql(string[] datas, IEnumerable<Rule> filters, string filedName, string errorMsg);
    }
}
