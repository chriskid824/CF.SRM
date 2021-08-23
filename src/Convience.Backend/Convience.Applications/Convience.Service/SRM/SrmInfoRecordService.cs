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
        public PagingResultModel<viewSrmInfoRecord> Query(QueryInfoRecordModels query);
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

        public PagingResultModel<viewSrmInfoRecord> Query(QueryInfoRecordModels query)
        {
            int skip = (query.page - 1) * query.size;

            //var infoRecordQuery = _context.SrmInforecords.AsQueryable()
            //    .AndIfHaveValue(query.MatnrId, r => r.MatnrId.Equals(query.MatnrId))
            //    .AndIfHaveValue(query.VendorId, r => r.VendorId.Equals(query.VendorId))
            //    .AndIfHaveValue(query.Status, r => r.Status.Equals(query.Status))
            //    .AndIfHaveValue(query.QotId, r => r.QotId.Equals(query.QotId))
            //    .AndIfHaveValue(query.qotIds, r => query.qotIds.Contains(r.QotId.Value));
            //var r = infoRecordQuery.Skip(skip).Take(query.size);

            //var test2 = (from info in _context.SrmInforecords
            //             select info).Where(r=>r.MatnrId==1).ToArray();

            //var test = (from info in _context.SrmInforecords
            //            join v in _context.SrmVendors on info.VendorId equals v.VendorId
            //            join m in _context.SrmMatnrs on info.MatnrId equals m.MatnrId
            //            select new viewSrmInfoRecord(info)
            //            {
            //                VendorName = v.VendorName,
            //                MatnrName = m.SrmMatnr1,
            //            }
            //   )
            //   .AndIfHaveValue(query.MatnrId, r => r.MatnrId.Equals(query.MatnrId))
            //    .AndIfHaveValue(query.VendorId, r => r.VendorId.Equals(query.VendorId))
            //    .Where(r=>r.Ekgry == "777")
            //    .Where(r => r.MatnrName == "777")
            //   //.AndIfHaveValue(query.Status, r => r.InfoId ==1)
            //   //.Where(r => r.MatnrName == "1")
            //   .ToArray();


            viewSrmInfoRecord[] view = (from info in _context.SrmInforecords
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
                                        select new viewSrmInfoRecord(info)
                                        {
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
                                            viewstatus = ((Status)info.Status).ToString()
                                        }
                                     )
                .AndIfHaveValue(query.MatnrId, r => r.MatnrId.Equals(query.MatnrId))
                .AndIfHaveValue(query.VendorId, r => r.VendorId.Equals(query.VendorId))
                .AndIfHaveValue(query.Status, r => r.Status.Equals(query.Status))
                .ToArray();

            var result = view.Skip(skip).Take(query.size);
            return new PagingResultModel<viewSrmInfoRecord>
            {
                Data = result.ToArray(),
                Count = view.Count()
            };

                //(from rfqH in _context.SrmRfqHs
                // join sourcer in _context.AspNetUsers on rfqH.Sourcer equals sourcer.UserName
                // join create in _context.AspNetUsers on rfqH.CreateBy equals create.UserName
                // where rfqH.RfqId.Equals(RfqId)
                // select new ViewSrmRfqH
                // {

                //new PagingResultModel<SrmInforecord>{ 
                //data
                //}
                //JObject obj = new JObject() {
                //    { "data",JArray.FromObject(r)},
                //    { "total",result.Count()}
                //};
                //return obj;


                //return infoRecordQurty.ToArray();
            }

    }
}
