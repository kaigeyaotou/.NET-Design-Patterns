using Lunz.Data.Extensions.Query;
using Lunz.ProductCenter.Common.FilterSearchLike;

namespace Lunz.ProductCenter.Common.MoveNode
{
    public class Director
    {
        public void Construct<T>(Builder builder)
        {
            builder.BuildDatas<T>();
            builder.BuildFilters();
            builder.BuildSql();
            builder.BuildHandleSql<T>();
        }
    }
}
