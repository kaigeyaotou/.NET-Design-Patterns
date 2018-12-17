using Lunz.Data.Extensions.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Common.MoveNode
{
    public interface IFilterLikeSearch
    {
        (string errorMsg, (string Sql, dynamic Parameters)) Search(string type, string filedName, QueryGroup filter, int number, string splitSymbol);
    }
}
