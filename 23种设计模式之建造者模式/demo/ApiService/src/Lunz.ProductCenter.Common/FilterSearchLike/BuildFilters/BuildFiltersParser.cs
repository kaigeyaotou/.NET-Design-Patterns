using Lunz.Data.Extensions.Query;
using System.Collections.Generic;
using System.Linq;

namespace Lunz.ProductCenter.Common.MoveNode.Update
{
    public class BuildFiltersParser : MoveNode.GetFilters
    {
        public override (IEnumerable<Rule>, string errorMsg) CreateFilters(string filedName, QueryGroup filter, string[] datas, int number, string errorMsg)
        {
            if (errorMsg != string.Empty && errorMsg != null)
            {
                return (null, errorMsg);
            }

            if (datas.Length <= number)
            {
                int j = filter.Rules.FindIndex(x => x.Field == filedName);
                filter.Rules[j] = new Rule()
                {
                    Op = "cn",
                    Data = string.Empty,
                    Datas = datas.ToList(),
                    Field = filedName,
                };
                var filters = from n in filter.Rules
                              where n.Data != null || n.Datas.Count() != 0
                              select n;
                return (filters, errorMsg);
            }
            else
            {
                errorMsg = $"最多只能 {number} 个关键字";
                return (null, errorMsg);
            }
        }
    }
}
