using Convience.Entity.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public record QuerySrmProcess : PageQueryModel
    {
        public SrmProcess process { get; set; }
    }
    public record QuerySrmSurface : PageQueryModel
    {
        public SrmSurface surface { get; set; }
    }
}
