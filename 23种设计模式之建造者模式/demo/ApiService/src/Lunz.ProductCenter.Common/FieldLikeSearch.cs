using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;

namespace Lunz.ProductCenter.Common
{
    public class FieldLikeSearch
    {
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="filedName">字段名</param>
        /// <param name="filter">filters</param>
        /// <param name="number">最多关键字数量</param>
        /// <returns>返回值</returns>
        public static (string errorMsg, (string Sql, dynamic Parameters)) GetLikeSearchByField<T>(string filedName, QueryGroup filter, int number)
        {
            var prodNameInfo = filter.Rules.FirstOrDefault(x => x.Field == filedName && x.Data != null);
            var filSql = filter.ToSql<T>().ToTuble();
            string errMsg = string.Empty;
            try
            {
                if (prodNameInfo != null)
                {
                    string[] datas = prodNameInfo.Data.Trim().Split(' ');
                    datas = (from m in datas
                             where m != null && m != string.Empty
                             select m).ToArray();
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
                        int para = 0;
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("(");
                        int field = -1;
                        foreach (var item in filters)
                        {
                            if (item.Field == filedName)
                            {
                                if (field == -1)
                                {
                                    field = para;
                                }

                                int i = para;
                                foreach (var tag in datas)
                                {
                                    strSql.Append($"{filedName} LIKE  @Param{i} OR ");
                                    i++;
                                }
                            }

                            para++;
                        }

                        string likeSql = strSql.ToString();
                        likeSql = likeSql.Remove(likeSql.LastIndexOf("OR "), 2) + ")";
                        var queryFilter = filter.ToSql<T>().ToTuble();
                        queryFilter.Sql = queryFilter.Sql.Replace($"{filedName} LIKE @Param{field}", likeSql);
                        filSql = queryFilter;
                    }
                    else
                    {
                        errMsg = $"最多只能 {number} 个关键字";
                    }
                }
            }
            catch (Exception e)
            {
                errMsg = $"发生异常，请联系管理员。";
            }

            return (errMsg, filSql);
        }
    }
}
