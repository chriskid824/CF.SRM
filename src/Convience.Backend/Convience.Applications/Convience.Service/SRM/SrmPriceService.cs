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
        public int? Start(ViewSrmInfoRecord[] infos);
        public void Save(ViewSrmInfoRecord[] infos);
        public SrmCurrency[] GetCurrency();
        public SrmTaxcode[] GetTaxcodes();
        public SrmEkgry[] GetEkgry(int[] werks);
        public void UpdateCaseid(ViewSrmInfoRecord[] infos);
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
            int[] qotIds = query.AsEnumerable().Select(r => r.QotId).ToArray();
            int[] vendorIds = query.AsEnumerable().Select(r => r.VendorId.Value).Distinct().ToArray();
            var temp = query.AsEnumerable().Select(r => new { a = r.RfqId, b = r.VendorId }).ToArray();
            ViewSrmPriceDetail price = new ViewSrmPriceDetail();
            price.qot = query;

            price.material = (from material in _context.SrmQotMaterial
                              join qot in _context.SrmQotHs
                              on material.QotId equals qot.QotId
                              join vendor in _context.SrmVendors
                              on qot.VendorId equals vendor.VendorId
                              where qotIds.Contains(material.QotId.Value)
                              select new viewSrmQotMaterial(material)
                              {
                                  VendorId = vendor.VendorId,
                                  VendorName = vendor.VendorName
                              }).ToArray();
            price.process = (from process in _context.SrmQotProcesses
                             join qot in _context.SrmQotHs
                             on process.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             join processMain in _context.SrmProcesss
                             on process.PProcessNum equals processMain.ProcessNum.ToString()
                             where qotIds.Contains(process.QotId.Value)
                             select new viewSrmQotProcess(process)
                             {
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 SubTotal = process.PPrice.GetValueOrDefault() * (decimal)process.PHours.GetValueOrDefault(),
                                 ProcessName = processMain.Process
                             }).ToArray();
            price.surface = (from surface in _context.SrmQotSurfaces
                             join qot in _context.SrmQotHs
             on surface.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             join processMain in _context.SrmSurfaces
                             on surface.SProcess equals processMain.SurfaceId.ToString()
                             where qotIds.Contains(surface.QotId.Value)
                             select new viewSrmQotSurface(surface)
                             {
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 SubTotal = surface.SPrice.GetValueOrDefault() * (decimal)surface.STimes.GetValueOrDefault(),
                                 ProcessName = processMain.SurfaceDesc
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

            ViewSrmInfoRecord[] infos = new ViewSrmInfoRecord[query.Count()];
            for (int i = 0; i < infos.Length; i++)
            {
                SrmInforecord info = _context.SrmInforecords.Where(r => r.QotId == query[i].QotId).FirstOrDefault();
                ViewSrmInfoRecord Vinfo = new ViewSrmInfoRecord(info);
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
            return price;
        }
        public int? Start(ViewSrmInfoRecord[] infos) {
            int? rfqId = null;
            //string[] must = new string[] { "price", "unit", "ekgry", "leadTime", "standQty", "minQty", "taxcode", "effectiveDate", "expirationDate" };
            Validate(infos);
            foreach (ViewSrmInfoRecord info in infos)
            {
                if (_context.SrmInforecords.Any(r => r.QotId == info.QotId))
                {
                    throw new Exception($"已啟動過簽核");
                }

                SrmInforecord t = new SrmInforecord();

                //foreach (PropertyInfo prop in t.GetType().GetProperties())
                //    t.GetType().GetProperty(prop.Name).SetValue(t, prop.GetValue(info, null), null);

                _context.SrmInforecords.Add(info);
            }

            var qot = _context.SrmQotHs.Where(r => r.QotId == infos[0].QotId).FirstOrDefault();
            if (qot != null)
            {
                rfqId = qot.RfqId.Value;
            }

            _context.SaveChanges();
            return rfqId;
        }

        public void Save(ViewSrmInfoRecord[] infos)
        {
            Validate(infos);
            List<int> infoIds = new List<int>();
            foreach (ViewSrmInfoRecord info in infos)
            {
                if (_context.SrmInforecords.Any(r => r.QotId == info.QotId))
                {
                    _context.SrmInforecords.Update(info);
                    infoIds.Add(info.InfoId);
                }
                else
                {
                    _context.SrmInforecords.Add(info);
                }
            }

            var old = _context.SrmInforecords.Where(r => r.Caseid == infos[0].Caseid && !infoIds.Contains(r.InfoId));
            _context.SrmInforecords.RemoveRange(old);
            _context.SaveChanges();
        }


        private void Validate(ViewSrmInfoRecord[] infos)
        {
            Dictionary<string, string> must = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            must.Add("price", "總計(NT)");
            must.Add("unit", "幣別");
            must.Add("ekgry", "採購群組");
            must.Add("currency", "幣別");
            must.Add("leadTime", "計畫交貨時間");
            must.Add("standQty", "標準採購數量");
            must.Add("minQty", "最小採購數量");
            must.Add("taxcode", "稅碼");
            must.Add("effectiveDate", "生效日期");
            must.Add("expirationDate", "有效日期");
            //must.Add("note", "備註");
            must.Add("org", "採購組織");
            must.Add("infoKind", "資訊紀錄種類");
            must.Add("type", "資訊紀錄類型");
            foreach (ViewSrmInfoRecord info in infos)
            {
                foreach (PropertyInfo prop in info.GetType().GetProperties())
                {
                    var temp = prop.GetValue(info);
                    if (must.Keys.Contains(prop.Name, StringComparer.OrdinalIgnoreCase) && (temp == null || string.IsNullOrWhiteSpace(temp.ToString())))
                    {
                        throw new Exception($"報價單號:{info.qotNum}，{must[prop.Name]}未填");
                    }
                }
                int tempInt = 0;
                if (!int.TryParse((info.Price.Value*info.Unit.Value).ToString().TrimEnd('0','.'), out tempInt))
                {
                    throw new Exception($"報價單號:{info.qotNum}，總計*價格單位需為整數");
                }
                int digNmu = 4;
                if (info.Currency.ToUpper() == "TWD")
                {
                    digNmu = 2;
                }
                if (info.Price.Value.ToString().Split('.').Count() > 1)
                {
                    if (info.Price.Value.ToString().Split('.')[1].Length > digNmu)
                    {
                        throw new Exception($"報價單號:{info.qotNum}，總計格式錯誤，{info.currencyName}小數點最高{digNmu}碼");
                    }
                }

                if (!int.TryParse(info.Unit.Value.ToString(), out tempInt))
                {
                    throw new Exception($"報價單號:{info.qotNum}，價格單位格式錯誤");
                }

                if (info.LeadTime.Value < 1)
                {
                    throw new Exception($"報價單號:{info.qotNum}，計畫交貨時間必須大於等於1");
                }
                if (!int.TryParse(info.LeadTime.Value.ToString(), out tempInt))
                {
                    throw new Exception($"報價單號:{info.qotNum}，計畫交貨時間需為整數");
                }

                if (info.StandQty.Value < 1)
                {
                    throw new Exception($"報價單號:{info.qotNum}，標準採購數量必須大於等於1");
                }

                if (info.Type.ToUpper() == "W" && string.IsNullOrWhiteSpace(info.Sortl))
                {
                    throw new Exception($"報價單號:{info.qotNum}，資訊紀錄類型W時，排序條件必填");
                }

                if (info.ExpirationDate <= info.EffectiveDate)
                {
                    throw new Exception($"有效日期需大於生效日期");
                }
            }
        }

        public void UpdateCaseid(ViewSrmInfoRecord[] infos)
        {
            foreach (var info in infos) {
                _context.Entry(info).Property(x => x.Caseid).IsModified = true;
            }
            _context.SaveChanges();
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
