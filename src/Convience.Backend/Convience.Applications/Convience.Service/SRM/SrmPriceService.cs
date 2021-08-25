using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.Model.Models.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmPriceService
    {
        public ViewSrmPriceDetail GetDetail(SrmQotH[] query);
        public int? Start(viewSrmInfoRecord[] infos);
        public SrmCurrency[] GetCurrency();
        public SrmTaxcode[] GetTaxcodes();
        public SrmEkgry[] GetEkgry(int[] werks);
    }
    public class SrmPriceService:ISrmPriceService
    {
        private readonly IRepository<SrmInforecord> _srmPriceRepository;
        private readonly SRMContext _context;
        public SrmPriceService(IRepository<SrmInforecord> srmPriceRepository, SRMContext context)
        {
            _srmPriceRepository = srmPriceRepository;
            _context = context;
        }
        public ViewSrmPriceDetail GetDetail(SrmQotH[] query)
        {
            //.AsEnumerable().Select(r => r.Field<string>("Name")).ToArray();
            int[] qotIds = query.AsEnumerable().Select(r => r.QotId).ToArray();
            int[] vendorIds = query.AsEnumerable().Select(r => r.VendorId.Value).Distinct().ToArray();
            var temp = query.AsEnumerable().Select(r => new { a = r.RfqId, b = r.VendorId }).ToArray();
            ViewSrmPriceDetail price = new ViewSrmPriceDetail();
            price.qot = query;
            //using (SRMContext _context = new SRMContext())
            //{

            price.material = (from material in _context.SrmQotMaterial
                              join qot in _context.SrmQotHs
                              on material.QotId equals qot.QotId
                              join vendor in _context.SrmVendors
                              on qot.VendorId equals vendor.VendorId
                              where qotIds.Contains(material.QotId.Value)
                              select new viewSrmQotMaterial(material)
                              {
                                  //QotMId = material.QotMId,
                                  //QotId = material.QotId,
                                  //MMaterial = material.MMaterial,
                                  //MPrice = material.MPrice,
                                  //MCostPrice = material.MCostPrice,
                                  //Length = material.Length,
                                  //Width = material.Width,
                                  //Height = material.Height,
                                  //Density = material.Density,
                                  //Weight = material.Weight,
                                  //Note = material.Note,
                                  VendorId = vendor.VendorId,
                                  VendorName = vendor.VendorName
                              }).ToArray();
            //price.material = (from material in db.SrmQotMaterial
            //           where qotIds.Contains(material.QotId.Value)
            //           select material).ToArray();
            price.process = (from process in _context.SrmQotProcesses
                             join qot in _context.SrmQotHs
                             on process.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             where qotIds.Contains(process.QotId.Value)
                             select new viewSrmQotProcess(process)
                             {
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 SubTotal = process.PPrice.GetValueOrDefault() * (decimal)process.PHours.GetValueOrDefault()
                             }).ToArray();
            //price.process = (from process in db.SrmQotProcesses
            //                             where qotIds.Contains(process.QotId.Value)
            //                             select process).ToArray();
            price.surface = (from surface in _context.SrmQotSurfaces
                             join qot in _context.SrmQotHs
             on surface.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             where qotIds.Contains(surface.QotId.Value)
                             select new viewSrmQotSurface(surface)
                             {
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 SubTotal = surface.SPrice.GetValueOrDefault() * (decimal)surface.STimes.GetValueOrDefault()
                             }).ToArray();
            price.other = (from other in _context.SrmQotOthers
                           join qot in _context.SrmQotHs
             on other.QotId equals qot.QotId
                           join vendor in _context.SrmVendors
                           on qot.VendorId equals vendor.VendorId
                           where qotIds.Contains(other.QotId.Value)
                           select new viewSrmQotOther(other)
                           {
                               VendorId = vendor.VendorId,
                               VendorName = vendor.VendorName
                           }).ToArray();


            //(from material in db.SrmQotMaterial
            // join qot in db.SrmQotHs
            // on material.QotId equals qot.QotId
            // join vendor in db.SrmVendors
            // on qot.VendorId equals vendor.VendorId
            // where qotIds.Contains(material.QotId.Value)
            // select new viewSrmQotMaterial(material)
            // {
            //     VendorId = vendor.VendorId,
            //     VendorName = vendor.VendorName
            // })
            // .GroupBy(r => r.VendorName).Select(g => new
            // {
            //     VendorId = g.Key,
            //     count = g.Sum(s => s.MCostPrice)
            // });

            viewSrmInfoRecord[] infos = new viewSrmInfoRecord[query.Count()];
            for (int i = 0; i < infos.Length; i++)
            {
                SrmInforecord info = _context.SrmInforecords.Where(r => r.QotId == query[i].QotId).FirstOrDefault();
                viewSrmInfoRecord Vinfo = new viewSrmInfoRecord(info);
                if (!string.IsNullOrWhiteSpace(Vinfo.Currency)) { Vinfo.currencyName = _context.SrmCurrencies.Where(r => r.Currency.Equals(Vinfo.Currency)).Select(r=>r.CurrencyName).FirstOrDefault(); }
                if (!string.IsNullOrWhiteSpace(Vinfo.Taxcode)) { Vinfo.taxcodeName = _context.SrmTaxcodes.Where(r => r.Taxcode.Equals(Vinfo.Taxcode)).Select(r => r.TaxcodeName).FirstOrDefault(); }
                Vinfo.VendorId = query[i].VendorId;
                Vinfo.VendorName = _context.SrmVendors.Where(r => r.VendorId == Vinfo.VendorId).Select(r => r.VendorName).FirstOrDefault();
                Vinfo.QotId = query[i].QotId;
                Vinfo.MatnrId = query[i].MatnrId;
                Vinfo.Atotal = price.material.Where(r => r.QotId == Vinfo.QotId && r.MCostPrice.HasValue).Select(r => r.MCostPrice).Sum().Value;
                Vinfo.Btotal = price.process.Where(r => r.QotId == Vinfo.QotId).Select(r => r.SubTotal).Sum();
                Vinfo.Ctotal = price.surface.Where(r => r.QotId == Vinfo.QotId).Select(r => r.SubTotal).Sum();
                Vinfo.Dtotal = price.other.Where(r => r.QotId == Vinfo.QotId && r.OPrice.HasValue).Select(r => r.OPrice).Sum().Value;
                infos[i] = Vinfo;
            }
            price.infoRecord = infos;
            //price.infoRecord = price.material.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.MCostPrice)
            //}).GroupJoin(
            //price.process.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.SubTotal)
            //}), a => a.VendorId, b => b.VendorId, (material, process) => new
            //{
            //    material = material,
            //    process = process.DefaultIfEmpty()
            //}).GroupJoin(
            //price.surface.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.SubTotal)
            //}), r => r.material.VendorId, c => c.VendorId, (infoRecord, surface) => new
            //{
            //    infoRecord,
            //    surface = surface.DefaultIfEmpty()
            //}).GroupJoin(
            //price.other.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.OPrice)
            //}), r => r.infoRecord.material.VendorId, d => d.VendorId, (infoRecord, other) => new viewSrmQotInfoRecord
            //{
            //    Atotal = infoRecord.infoRecord.material.count.Value,
            //    Btotal = infoRecord.infoRecord.process.,
            //    Ctotal = infoRecord.surface.count,
            //    Dtotal = other.count.Value
            //}).DefaultIfEmpty().ToArray();

            //}
            return price;
        }
        public int? Start(viewSrmInfoRecord[] infos) {
            DateTime now = DateTime.Now;
            int? rfqId = null;
            //string[] must = new string[] { "price", "unit", "ekgry", "leadTime", "standQty", "minQty", "taxcode", "effectiveDate", "expirationDate" };
            Dictionary<string, string> must = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            must.Add("price", "總計(NT)");
            must.Add("unit", "幣別");
            must.Add("ekgry", "採購群組");
            must.Add("leadTime", "計畫交貨時間");
            must.Add("standQty", "標準採購數量");
            must.Add("minQty", "最小採購數量");
            must.Add("taxcode", "稅碼");
            must.Add("effectiveDate", "生效日期");
            must.Add("expirationDate", "有效日期");
            must.Add("note", "備註");
            foreach (viewSrmInfoRecord info in infos) {
                foreach (PropertyInfo prop in info.GetType().GetProperties())
                {
                    var temp = prop.GetValue(info);
                    if (must.Keys.Contains(prop.Name,StringComparer.OrdinalIgnoreCase) && (temp == null || string.IsNullOrWhiteSpace(temp.ToString()))) {
                        throw new Exception($"報價單號:{info.qotNum}，{must[prop.Name]}未填");
                    }
                }

                if (info.ExpirationDate <= info.EffectiveDate) {
                    throw new Exception($"有效日期需大於生效日期");
                }

                if (!_context.SrmInforecords.Any(r => r.QotId == info.QotId)) {
                    SrmInforecord t = new SrmInforecord();

                    foreach (PropertyInfo prop in t.GetType().GetProperties())
                        t.GetType().GetProperty(prop.Name).SetValue(t, prop.GetValue(info, null), null);

                    _context.SrmInforecords.Add(t);
                }
            }

            var qot = _context.SrmQotHs.Where(r => r.QotId == infos[0].QotId).FirstOrDefault();
            if (qot != null)
            {
                rfqId = qot.RfqId.Value;
            }

            _context.SaveChanges();
            return rfqId;
        }

        public SrmCurrency[] GetCurrency()
        {
            return _context.SrmCurrencies.ToArray();
        }

        public SrmTaxcode[] GetTaxcodes()
        {
            return _context.SrmTaxcodes.ToArray();
        }

        public SrmEkgry[] GetEkgry(int[] werks )
        {
            return _context.SrmEkgries.Where(r => werks.Contains(r.Werks)).ToArray();
        }
    }
}
