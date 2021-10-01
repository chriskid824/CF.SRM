using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.Extension
{
    public static class Extensions
    {
        public static decimal Normalize(this decimal value)
        {
            return value / 1.000000000000000000000000000000000m;
        }
        public static string NormalizeTwoDigits(this decimal value)
        {
            return string.Format("{0:N2}", value).Replace(".00", "");
        }
    }
}
