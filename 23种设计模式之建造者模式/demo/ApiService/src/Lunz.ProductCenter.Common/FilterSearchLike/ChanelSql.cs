using Lunz.Data.Extensions.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Common.FilterSearchLike
{
    public abstract class ChanelSql
    {
        public abstract (string errorMsg, (string Sql, dynamic Parameters)) HandleSql<T>(string likeSql, QueryGroup filter, string filedName, int field, string errorMsg);
    }
}
