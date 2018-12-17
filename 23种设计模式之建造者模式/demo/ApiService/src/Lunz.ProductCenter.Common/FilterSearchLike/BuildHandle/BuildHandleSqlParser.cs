using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Common.FilterSearchLike.chanelSql
{
    public class BuildHandleSqlParser : ChanelSql
    {
        public override (string errorMsg, (string Sql, dynamic Parameters)) HandleSql<T>(string likeSql, QueryGroup filter, string filedName, int field, string errorMsg)
        {
            if (errorMsg != string.Empty && errorMsg != null)
            {
                return (errorMsg, (null, 0));
            }

            string errMsg = string.Empty;
            var filSql = filter.ToSql<T>().ToTuble();

            try
            {
                likeSql = likeSql.Remove(likeSql.LastIndexOf("OR "), 2) + ")";
                var queryFilter = filter.ToSql<T>().ToTuble();
                queryFilter.Sql = queryFilter.Sql.Replace($"{filedName} LIKE @Param{field}", likeSql);
                filSql = queryFilter;
            }
            catch (Exception e)
            {
                errMsg = e.ToString();
            }

            return (errMsg, filSql);
        }
    }
}
