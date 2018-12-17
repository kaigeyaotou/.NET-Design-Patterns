using Lunz.Data.Extensions.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Common.MoveNode
{
    public abstract class GetDatas
    {
        public abstract (string[] datas, string errorMsg) CreateDatas(string filedName, QueryGroup filter, int number, string SplitSymbol, string errorMsg);
    }
}
