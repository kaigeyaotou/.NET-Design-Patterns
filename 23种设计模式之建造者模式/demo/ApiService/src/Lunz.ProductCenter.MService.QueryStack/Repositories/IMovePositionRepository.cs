using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.Domain.Kernel.Repositories;
using Lunz.ProductCenter.ApiService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Models;

namespace Lunz.ProductCenter.MService.QueryStack.Repositories
{
    public interface IMovePositionRepository
    {
        Task<int> MovePosition<T>(int moveAfterPosition, string moveId, string parentId, string tbName)
            where T : MoveBaseModel;
    }
}
