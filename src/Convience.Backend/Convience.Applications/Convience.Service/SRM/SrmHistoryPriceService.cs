using Convience.Entity.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convience.Util.Extension;
using Convience.Model.Models.SRM;

namespace Convience.Service.SRM
{
    public interface ISrmHistoryPriceService
    {
        public SrmHistoryPrice GetHistoryPrice(QuerySrmHistoryPrice query);
    }
    public class SrmHistoryPriceService: ISrmHistoryPriceService
    {
        private readonly SRMContext _context;
        public SrmHistoryPriceService(SRMContext context)
        {
            _context = context;
        }
        public SrmHistoryPrice GetHistoryPrice(QuerySrmHistoryPrice query) {
            var result = _context.SrmHistoryPrices
                 .Where(r=>r.Ekorg.Equals(query.Ekorg))
                 .AndIfHaveValue(query.Matnr, r => r.Matnr.Equals(query.Matnr))
                 .AndIfHaveValue(query.Essay, r => r.Essay.Equals(query.Essay))
                 .AndIfHaveValue(query.year, r => r.OrderDate.Value.Year.Equals(query.year.Value));
            return query.orderASC ? result.OrderBy(r => r.OrderDate).FirstOrDefault() : result.OrderByDescending(r => r.OrderDate).FirstOrDefault();
        }
    }
}
