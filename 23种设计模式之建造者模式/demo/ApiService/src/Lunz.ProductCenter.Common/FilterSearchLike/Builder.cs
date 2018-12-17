namespace Lunz.ProductCenter.Common.FilterSearchLike
{
    public abstract class Builder
    {
        public abstract void BuildDatas<T>();

        public abstract void BuildFilters();

        public abstract void BuildSql();

        public abstract void BuildHandleSql<T>();

        public abstract Sql BuildProduct();
    }
}
