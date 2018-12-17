using System;
using System.Collections.Generic;
using System.Text;

namespace Lunz.ProductCenter.MService.QueryStack.Models
{
    public class MoveBaseModel
    {
        public string Id { get; set; }

        public int? SortOrder { get; set; }

        public string ParentId { get; set; }

        public int? LevelCode { get; set; }

        public bool HasChildren { get; set; }
    }
}
