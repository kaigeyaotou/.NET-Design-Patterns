using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.Health.Interface
{
    public interface IHealthChecker
    {
        string Message { get; set; }

        bool DoCheck();
    }
}
