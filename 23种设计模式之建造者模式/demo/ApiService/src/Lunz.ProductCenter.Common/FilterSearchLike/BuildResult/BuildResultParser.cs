using System;
using System.Collections.Generic;
using System.Text;
using Lunz.Data.Extensions.Query;

namespace Lunz.ProductCenter.Common.FilterSearchLike.BuildResult
{
    public class BuildResultParser : BuildSql
    {
        public override (string likeSql, int field, string errorMsg) CreateSql(string[] datas, IEnumerable<Rule> filters, string filedName, string errorMsg)
        {
            if (errorMsg != string.Empty && errorMsg != null)
            {
                return (null, 0, errorMsg);
            }

            int para = 0;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("(");
            int field = -1;
            try
            {
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
            }
            catch (Exception e)
            {
                errorMsg = e.ToString();
            }

            string likeSql = strSql.ToString();
            return (likeSql, field, errorMsg);
        }
    }
}
