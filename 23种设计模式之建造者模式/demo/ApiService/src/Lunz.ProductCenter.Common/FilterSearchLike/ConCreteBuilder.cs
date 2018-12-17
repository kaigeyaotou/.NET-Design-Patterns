using System;
using System.Collections.Generic;
using System.Text;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sql;
using Lunz.ProductCenter.Common.FilterSearchLike.BuildResult;
using Lunz.ProductCenter.Common.FilterSearchLike.chanelSql;
using Lunz.ProductCenter.Common.MoveNode;
using Lunz.ProductCenter.Common.MoveNode.Select;
using Lunz.ProductCenter.Common.MoveNode.Update;

namespace Lunz.ProductCenter.Common.FilterSearchLike
{
    public class ConCreteBuilder : Builder
    {
        #region
        private string _type;

        private string _filedName;

        private QueryGroup _filter;

        private int _number;

        private IEnumerable<Rule> _filters;

        private string _splitSymbol;

        private string _likeSql;

        private int _field;

        private string[] _datas;
        #endregion

        private Sql _sql = new Sql();

        public ConCreteBuilder(string type, string filedName, QueryGroup filter, int number, string splitSymbol)
        {
            this._type = type;
            this._filedName = filedName;
            this._filter = filter;
            this._number = number;
            this._splitSymbol = splitSymbol;
        }

        /// <summary>
        /// 处理SQL
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        public override void BuildHandleSql<T>()
        {
            ChanelSql chanelSql = new BuildHandleSqlParser();
            var res = chanelSql.HandleSql<T>(_likeSql, _filter, _filedName, _field, _sql.ErrorMsg);
            if (res.errorMsg != string.Empty)
            {
                _sql.ErrorMsg = res.errorMsg;
                BuildProduct();
            }

            _sql.Sql1 = res.Item2;
            _sql.ErrorMsg = res.errorMsg;
        }

        /// <summary>
        /// 构建有用数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        public override void BuildDatas<T>()
        {
            GetDatas getDatas = new BuildDatasParser();
            var res = getDatas.CreateDatas(_filedName, _filter, _number, _splitSymbol, _sql.ErrorMsg);
            _datas = res.datas;
            if (res.errorMsg == "prodNameInfo为空")
            {
                _sql.Sql1 = _filter.ToSql<T>().ToTuble();
                BuildProduct();
            }
            else if (res.errorMsg != string.Empty)
            {
                _sql.ErrorMsg = res.errorMsg;
                BuildProduct();
            }
        }

        /// <summary>
        /// 重构过滤信息
        /// </summary>
        public override void BuildFilters()
        {
            GetFilters getFilters = new BuildFiltersParser();
            var res = getFilters.CreateFilters(_filedName, _filter, _datas, _number, _sql.ErrorMsg);
            _filters = res.Item1;
            if (res.errorMsg != string.Empty)
            {
                _sql.ErrorMsg = res.errorMsg;
                BuildProduct();
            }
        }

        /// <summary>
        /// 生成产品SQL
        /// </summary>
        /// <returns></returns>
        public override Sql BuildProduct()
        {
            return _sql;
        }

        /// <summary>
        /// 生产SQL
        /// </summary>
        public override void BuildSql()
        {
            BuildSql getSql = new BuildResultParser();
            var res = getSql.CreateSql(_datas, _filters, _filedName, _sql.ErrorMsg);
            _likeSql = res.likeSql;
            _field = res.field;
            if (res.errorMsg != string.Empty)
            {
                _sql.ErrorMsg = res.errorMsg;
                BuildProduct();
            }
        }
    }
}
