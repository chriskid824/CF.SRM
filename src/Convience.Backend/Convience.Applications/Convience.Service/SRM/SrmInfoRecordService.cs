using Convience.Entity.Entity.SRM;
using Convience.Model.Models;
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
        public PagingResultModel<ViewSrmInfoRecord> Query(QueryInfoRecordModels query);
        public void UpdateStatus(Status status, SrmInforecord info);
    }
    class SrmInfoRecordService : ISrmInfoRecordService
    {
        private readonly SRMContext _context;

        public SrmInfoRecordService(SRMContext context) {
            _context = context;
        }

        public SrmInforecord[] Get(QueryInfoRecordModels query)
        {
            var infoRecordQurty = _context.SrmInforecords.AsQueryable()
                .AndIfHaveValue(query.QotId, r => r.QotId.Equals(query.QotId))
                .AndIfHaveValue(query.qotIds, r => query.qotIds.Contains(r.QotId.Value));
            return infoRecordQurty.ToArray();
        }

        public PagingResultModel<ViewSrmInfoRecord> Query(QueryInfoRecordModels query)
        {
            int skip = (query.page - 1) * query.size;

            ViewSrmInfoRecord[] view = (from info in _context.SrmInforecords
                                        join v in _context.SrmVendors on info.VendorId equals v.VendorId into vgrouping
                                        from v in vgrouping.DefaultIfEmpty()
                                        join m in _context.SrmMatnrs on info.MatnrId equals m.MatnrId into mgrouping
                                        from m in mgrouping.DefaultIfEmpty()
                                        join c in _context.SrmCurrencies on info.Currency equals c.Currency into cgrouping
                                        from c in cgrouping.DefaultIfEmpty()
                                        join t in _context.SrmTaxcodes on info.Taxcode equals t.Taxcode into tgrouping
                                        from t in tgrouping.DefaultIfEmpty()
                                        join q in _context.SrmQotHs on info.QotId equals q.QotId into qgrouping
                                        from q in qgrouping.DefaultIfEmpty()
                                        join r in _context.SrmRfqHs on q.RfqId equals r.RfqId into rgrouping
                                        from r in rgrouping.DefaultIfEmpty()
                                        select new ViewSrmInfoRecord(info)
                                        {
                                            matnrObject = m,
                                            vendorObject = v,
                                            srmMatnr1 = m.SrmMatnr1,
                                            MatnrName = m.SrmMatnr1,
                                            srmVendor1 = v.SrmVendor1,
                                            VendorName = v.VendorName,
                                            MatnrId = info.MatnrId,
                                            VendorId = info.VendorId,
                                            Status = info.Status,
                                            currencyName = c.CurrencyName,
                                            taxcodeName = t.TaxcodeName,
                                            qotNum = q.QotNum,
                                            rfqNum = r.RfqNum,
                                            rfqId = r.RfqId.ToString(),
                                            viewstatus = ((Status)info.Status).ToString(),
                                            Org = info.Org
                                        }
                                     )
                                     .AndIfHaveValue(query.MatnrId, r => r.MatnrId.Equals(query.MatnrId))
                                     .AndIfHaveValue(query.VendorId, r => r.VendorId.Equals(query.VendorId))
                                     .AndIfHaveValue(query.Status, r => r.Status.Equals(query.Status))
                                     .Where(r=>query.werks.Contains(r.Org.Value))
                                     .ToArray();

            var result = view.Skip(skip).Take(query.size);
            return new PagingResultModel<ViewSrmInfoRecord>
            {
                Data = result.ToArray(),
                Count = view.Count()
            };
        }

        public void UpdateStatus(Status status, SrmInforecord info) {
            var infos = _context.SrmInforecords.AndIfHaveValue(info.Caseid, r => r.Caseid.Value.Equals(info.Caseid)).ToList();
            infos.ForEach(r => r.Status = (int)status);
            _context.SaveChanges();
        }
    }
}
