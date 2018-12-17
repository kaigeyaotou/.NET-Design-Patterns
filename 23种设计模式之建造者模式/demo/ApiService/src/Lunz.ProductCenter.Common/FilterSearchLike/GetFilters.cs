using Lunz.Data.Extensions.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Common.MoveNode
{
    public abstract class GetFilters
    {
        public abstract (IEnumerable<Rule>, string errorMsg) CreateFilters(string filedName, QueryGroup filter, string[] datas, int number, string errorMsg);
    }
}
