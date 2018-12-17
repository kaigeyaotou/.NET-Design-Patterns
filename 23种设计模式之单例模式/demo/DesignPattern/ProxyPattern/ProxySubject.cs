using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyPattern
{
    /// <summary>
    /// 代理：只能传达原有逻辑，不能新增业务逻辑
    /// 自建的一层
    /// </summary>
    public class ProxySubject : ISubject
    {
        private static ISubject _iSubject = null;// new RealSubject();
        private void Init()
        {
            _iSubject = new RealSubject();
        }



        private static bool _BooleanResult = false;
        private static bool _IsInit = false;

        private static Dictionary<string, bool> _Cache = new Dictionary<string, bool>();

        /// <summary>
        /// 火车站查询火车票
        /// </summary>
        public bool GetSomething()
        {
            Console.WriteLine("before GetSomething");
            if (!_IsInit)//缓存
            {
                _BooleanResult = _iSubject.GetSomething();
                _IsInit = true;
            }

            Console.WriteLine("after GetSomething");
            return _BooleanResult;
        }

        /// <summary>
        /// 火车站买票
        /// </summary>
        public void DoSomething()
        {
            Console.WriteLine("before DoSomething");

            if (_iSubject == null)
            {
                this.Init();
            }
            _iSubject.DoSomething();
            Console.WriteLine("after DoSomething");
        }
    }
}
