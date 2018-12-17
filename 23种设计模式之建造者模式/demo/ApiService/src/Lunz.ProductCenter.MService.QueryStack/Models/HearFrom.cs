using System;
using Lunz.Domain.Kernel.Repositories;

namespace Lunz.ProductCenter.ApiService.QueryStack.Models
{
    public class HearFrom : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
