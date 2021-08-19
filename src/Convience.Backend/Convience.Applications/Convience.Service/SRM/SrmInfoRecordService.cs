using Convience.Entity.Entity.SRM;
using Convience.Model.Models.SRM;
using Convience.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmInfoRecordService
    {
        public SrmInforecord[] Get(QueryInfoRecordModels query);
    }
    class SrmInfoRecordService : ISrmInfoRecordService
    {
        private readonly SRMContext _context;

        public SrmInfoRecordService(SRMContext context) {
            _context = context;
        }

        public SrmInforecord[] Get(QueryInfoRecordModels query)
        {
            //using (SRMContext db = new SRMContext())
            //{
            var infoRecordQurty = _context.SrmInforecords.AsQueryable()
                .AndIfHaveValue(query.QotId, r => r.QotId.Equals(query.QotId))
                .AndIfHaveValue(query.qotIds, r => query.qotIds.Contains(r.QotId.Value));
            return infoRecordQurty.ToArray();
            //}
        }
    }
}
