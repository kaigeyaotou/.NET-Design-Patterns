using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunz.ProductCenter.Common.MoveNode.Select
{
    public class BuildDatasParser : GetDatas
    {
        public override (string[] datas, string errorMsg) CreateDatas(string filedName, QueryGroup filter, int number, string SplitSymbol, string errorMsg)
        {
            if (errorMsg != string.Empty && errorMsg != null)
            {
                return (null, errorMsg);
            }

            string[] datas = { };
            var prodNameInfo = filter.Rules.FirstOrDefault(x => x.Field == filedName && x.Data != null);

            // var filSql = filter.ToSql<T>().ToTuble();
            string errMsg = string.Empty;
            try
            {
                if (prodNameInfo != null)
                {
                    datas = prodNameInfo.Data.Trim().Split(SplitSymbol.ToCharArray());
                    datas = (from m in datas
                             where m != null && m != string.Empty
                             select m).ToArray();
                }
                else
                {
                    errMsg = "prodNameInfo为空";
                }
            }
            catch (Exception)
            {
                errMsg = $"发生异常，请联系管理员。";
            }

            return (datas, errMsg);
        }
    }
}