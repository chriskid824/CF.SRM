using Convience.Entity.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convience.Util.Extension;

namespace Convience.Service.SRM
{
    public interface ISrmHistoryPriceService
    {
        public SrmHistoryPrice GetHistoryPrice(SrmHistoryPrice query);
    }
    public class SrmHistoryPriceService: ISrmHistoryPriceService
    {
        private readonly SRMContext _context;
        public SrmHistoryPriceService(SRMContext context)
        {
            _context = context;
        }
        public SrmHistoryPrice GetHistoryPrice(SrmHistoryPrice query) {
           return _context.SrmHistoryPrices.Where(r => r.Matnr.Equals(query.Matnr) || r.Essay.Equals(query.Essay)).OrderByDescending(r => r.OrderDate).FirstOrDefault();
        }
    }
}
