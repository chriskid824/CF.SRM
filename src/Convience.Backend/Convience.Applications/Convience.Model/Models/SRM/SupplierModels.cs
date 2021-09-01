using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public class QuerySupplierList
    {
        public string code { get; set; }
        public string name { get; set; }
        public int[] statuses { get; set; }
        public int[] Werks { get; set; }
        public bool end { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
