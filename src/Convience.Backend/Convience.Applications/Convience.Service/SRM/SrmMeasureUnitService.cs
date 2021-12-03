using Convience.Entity.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmMeasureUnitService
    {
        public Dictionary<int, string> GetMeasureUnit();
    }
    class SrmMeasureUnitService: ISrmMeasureUnitService
    {
        private readonly SRMContext _context;
        public SrmMeasureUnitService(SRMContext context)
        {
            _context = context;
        }
        public Dictionary<int, string> GetMeasureUnit() {
            List<SrmMeasureUnit> units = _context.SrmMeasureUnits.ToList();
            Dictionary<int, string> r = new Dictionary<int, string>();
            foreach (var unit in units) {
                r.Add(unit.MeasureId, unit.MeasureDesc);
            }
            return r;
        }
    }
}
