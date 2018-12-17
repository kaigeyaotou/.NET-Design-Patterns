using System.Collections.Generic;
using System.Linq;
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.Common
{
    public class TreeLikeSearch
    {
        /// <summary>
        /// 树形查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="infos">查询出的全量数据</param>
        /// <param name="allInfos">全量数据</param>
        /// <param name="level">级别</param>
        /// <returns>返回与级别相符的数据</returns>
        public static IEnumerable<T> GetTreesByName<T>(IEnumerable<T> infos, IEnumerable<T> allInfos, int level)
            where T : MoveBaseModel
        {
            List<T> models = new List<T>();
            foreach (var item in infos)
            {
                if (item.ParentId == null && !models.Contains<T>(item))
                {
                    models.Add(item);
                    continue;
                }

                T term = item;
                string pId = string.Empty;
                for (int i = item.LevelCode.Value; i > 0; i--)
                {
                    if (term.LevelCode == level)
                    {
                        break;
                    }

                    pId = term.ParentId;
                    if (pId == string.Empty || pId == null)
                    {
                        continue;
                    }

                    term = (from m in allInfos
                            where m.Id == pId
                            select m).First();
                }

                if (!models.Contains<T>(term))
                {
                    models.Add(term);
                }
            }

            return models;
        }
    }
}
