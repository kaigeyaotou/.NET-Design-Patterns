using Lunz.Data.Extensions.Query;
using Lunz.ProductCenter.Common.FilterSearchLike;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Common.MoveNode
{
    public class FilterLikeSearch
    {
        public static (string errorMsg, (string Sql, dynamic Parameters)) Search<T>(string type, string filedName, QueryGroup filter, int number, string splitSymbol)
        {
            Director director = new Director();
            Builder builder = new ConCreteBuilder(type, filedName, filter, number, splitSymbol);
            director.Construct<T>(builder);
            Sql sql = builder.BuildProduct();

            return (sql.ErrorMsg, sql.Sql1);
        }
    }
}
