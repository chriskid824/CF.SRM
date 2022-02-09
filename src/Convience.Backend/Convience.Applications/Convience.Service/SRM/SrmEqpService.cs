using AutoMapper;

using Convience.Entity.Data;
using Convience.Entity.Entity;
using Convience.Entity.Entity.Identity;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.JwtAuthentication;
using Convience.Model.Constants.SystemManage;
using Convience.Model.Models;
using Convience.Model.Models.SRM;
using Convience.Model.Models.SystemManage;
using Convience.Util.Extension;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmEqpService
    {
        public IEnumerable<ViewEqpList> GetPoData(QueryEqp query);
        public PagingResultModel<ViewSrmEqpH> GetEqpList(QueryEqp q, int page, int size);
        public JObject Save(SrmEqpH eqpH);
        public ViewSrmEqpH GetDataByEqpId(int EqpId);
        public void UpdateStatus(int status, SrmEqpH eqpH);
        public int GetVendorId(QueryEqp e);
        public string GetEqptxtSN(SrmEqpH e);
        public bool CheckIfExists(SrmEqpH e);
        public SrmEqpH[] Get(QueryEqp query);
        public string GetEkgryMail(SrmEqpH e);
        public string GetMatnr(SrmEqpH e);
        public string GettxtSN();
        public bool CheckIfStart(SrmEqpH e);
        public SrmEqpH GetEqp(SrmEqpH eqpH);
        //public int GetCaseId(QueryEqp e);
        public string  GetVendorName(QueryEqp e);
        public ViewSrmEqpH GetEqpH(SrmEqpH q);
    }

    public class SrmEqpService : ISrmEqpService
    {
        private readonly SRMContext _context;
        private readonly IRepository<SrmEqpH> _srmEqpHRepository;
        private readonly IRepository<SrmPoH> _srmPoHRepository;
        private readonly IRepository<SrmPoL> _srmPoLRepository;
        private readonly IRepository<SrmEkgry> _srmEkgryRepository;
        private readonly IRepository<SrmMatnr> _srmSrmMatnrRepository;
        private readonly IRepository<SrmStatus> _srmSrmStatusRepository;
        public SrmEqpService(
            //IMapper mapper,
            IRepository<SrmEqpH> srmEqpHRepository,
            IRepository<SrmPoH> srmPoHRepository,
            IRepository<SrmPoL> srmPoLRepository,
            IRepository<SrmEkgry> srmEkgryRepository,
            IRepository<SrmMatnr> srmSrmMatnrRepository,
            IRepository<SrmStatus> srmSrmStatusRepository,
            SRMContext dbContext)
        //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmEqpHRepository = srmEqpHRepository;
            _srmPoHRepository = srmPoHRepository;
            _srmPoLRepository = srmPoLRepository;
            _srmEkgryRepository = srmEkgryRepository;
            _srmSrmMatnrRepository = srmSrmMatnrRepository;
            _srmSrmStatusRepository = srmSrmStatusRepository;
            _context = dbContext;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }
     
        public IEnumerable<ViewEqpList> GetPoData(QueryEqp query)
        {
            int matnrid = 0;
            if (query.matnr != null)  
            {
                var m = _context.SrmMatnrs.Where(p => p.SrmMatnr1 == query.matnr || p.SapMatnr == query.matnr).ToList();
                matnrid = m.Select(r => r.MatnrId).First();
            }
            if (matnrid == 0)
            {
                if (query.matnrid != 0) 
                {
                    matnrid = query.matnrid;
                }
            }

            var eqplist = (from poh in _context.SrmPoHs
                           join pol in _context.SrmPoLs on poh.PoId equals pol.PoId
                           join ek in _context.SrmEkgries on poh.Buyer equals ek.Ekgry
                           join m in _context.SrmMatnrs on pol.MatnrId equals m.MatnrId
                           join v in _context.SrmVendors on poh.VendorId equals v.VendorId
                           select new ViewEqpList
                           {
                               buyer = poh.Buyer,
                               po_id = poh.PoId,
                               po_num = poh.PoNum,
                               polid = pol.PoLId,
                               description = pol.Description,
                               qty = pol.Qty,
                               deliverydate = pol.DeliveryDate,
                               matnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1,
                               matnr_id = m.MatnrId,//pol.MatnrId,
                               ekgry = ek.Ekgry,
                               ekgry_desc = ek.EkgryDesc,
                               ekgry_id = ek.Empid,
                               vendorid = poh.VendorId,
                               vendor = (!string.IsNullOrWhiteSpace(v.SapVendor)) ? v.SapVendor : v.SrmVendor1
                           })
                              .AndIfHaveValue(query.woNum, p => p.po_num == query.woNum)
                              .AndIfHaveValue(query.no, p => p.polid.ToString() == query.no)
                              //.AndIfHaveValue(query.matnr, p => p.matnr == query.matnr);
                              .AndIfCondition(matnrid != 0, p => p.matnr_id == matnrid) //20220111改用matnrid 取
                              .AndIfCondition((!string.IsNullOrWhiteSpace(query.vendor)), p => p.vendor == query.vendor);

            return eqplist;
        }
     
        public JObject Save(SrmEqpH eqpH)
        {
            JObject obj = new JObject();

            obj["msg"] = string.Empty;
            obj["txtSN"] = eqpH.EqpNum;

            string errmsg = string.Empty;
            #region 檢核採購資訊
            QueryEqp qeqp = new QueryEqp();
            qeqp.woNum = eqpH.WoNum;
            qeqp.no = eqpH.no;
            qeqp.matnrid = eqpH.MatnrId.Value;
            //string matnr = string.Empty;
            //var m = _context.SrmMatnrs.Where(p => p.MatnrId == eqpH.MatnrId).ToList();
            //matnr = m.Select(r => r.SapMatnr).First();
            //qeqp.matnr = matnr;
            IEnumerable<ViewEqpList> checkpodata = GetPoData(qeqp);
            if (checkpodata.Count() == 0)
            {
                obj["msg"] = "查無對應採購單資訊";
                return obj;
            }
            #endregion
            DateTime now = DateTime.Now;
            eqpH.LastUpdateDate = now;
            int eqpstatus = 0;
            string EqpNum = string.Empty;
            //新
            if (eqpH.EqpId == 0)
            {
                //20220126為測試先拿掉
                //檢查重複的內容.
                bool isexist = CheckIfExists(eqpH);
                if (!isexist)
                {
                    eqpH.LastUpdateDate = now;
                    eqpH.CreateBy = eqpH.LastUpdateBy;
                    eqpH.Status = 1;
                    EqpNum = GettxtSN();
                    eqpH.EqpNum = EqpNum;
                    obj["txtSN"] = EqpNum;
                    _context.SrmEqpHs.Add(eqpH);
                    _context.SaveChanges();
                }
                else
                {
                    string matnr = GetMatnr(eqpH);
                    obj["msg"] = $"採購單{eqpH.WoNum}，料號{matnr}重複申請";
                    return obj;
                }
            }
            else
            {
                eqpstatus = GetEqpStatus(eqpH);
                if (eqpstatus != 1)
                {
                    //throw new Exception($"非初始狀態無法編輯反應單");
                    obj["msg"] = "非初始狀態無法編輯反應單";
                    return obj;
                }
                else 
                {
                    using (var db = new SRMContext())
                    {
                        var eqp = new SrmEqpH() { EqpId = eqpH.EqpId };//qotH.QotId };
                        db.SrmEqpHs.Attach(eqp);

                        eqp.LastUpdateDate = now;
                        eqp.LastUpdateBy = eqpH.LastUpdateBy;
                        eqp.Status = eqpH.Status;
                        eqp.WoNum = eqpH.WoNum;
                        eqp.MatnrId = eqpH.MatnrId;
                        eqp.no = eqpH.no;
                        eqp.NgQty = eqpH.NgQty;
                        eqp.Version = eqpH.Version;
                        eqp.NgDesc = eqpH.NgDesc;
                        eqp.CauseAnalyses = eqpH.CauseAnalyses;


                        db.Entry(eqp).Property(p => p.LastUpdateDate).IsModified = true;
                        db.Entry(eqp).Property(p => p.LastUpdateBy).IsModified = true;
                        db.Entry(eqp).Property(p => p.Status).IsModified = true;
                        db.Entry(eqp).Property(p => p.WoNum).IsModified = true;
                        db.Entry(eqp).Property(p => p.MatnrId).IsModified = true;
                        db.Entry(eqp).Property(p => p.no).IsModified = true;
                        db.Entry(eqp).Property(p => p.NgQty).IsModified = true;
                        db.Entry(eqp).Property(p => p.Version).IsModified = true;
                        db.Entry(eqp).Property(p => p.NgDesc).IsModified = true;
                        db.Entry(eqp).Property(p => p.CauseAnalyses).IsModified = true;

                        var result = db.SaveChanges();
                    }
                }
            }
            //_context.SaveChanges();
            return obj;
                 
        }
        public PagingResultModel<ViewSrmEqpH> GetEqpList(QueryEqp e, int page, int size)
        {
            int skip = (page - 1) * size;

            var result = GetEqpList(e);
            var r = result.AsQueryable().Skip(skip).Take(size).ToArray();
            return new PagingResultModel<ViewSrmEqpH>
            {
                Data = r,
                Count = result.Count()
            };    
        }
        //e.po_num = query["eqpNum"].ToString();
        //e.woNum = query["woNum"].ToString();
        //e.no = query["no"].ToString();
        //e.matnr = query["matnr"].ToString();

        //UserClaims user = User.GetUserClaims();
        //e.createdBy = "13";//帶供應商id?????


        public ViewSrmEqpH[] GetEqpList(QueryEqp e)
        {
            //20220209 查詢
            int vendorid = 0;
            if (!string.IsNullOrWhiteSpace(e.vendor))
            {
                vendorid = GetVendorId(e);
            }


            //var qotlist = (from r in _context.SrmRfqHs
            //               join q in _context.SrmQotHs on r.RfqId equals q.RfqId
            //               join rm in _context.SrmRfqMs on new { RfqId = q.RfqId, MatnrId = q.MatnrId } equals new { RfqId = rm.RfqId, MatnrId = rm.MatnrId }
            //               join m in _context.SrmMatnrs on q.MatnrId equals m.MatnrId
            //               join v in _context.SrmVendors on q.VendorId equals v.VendorId
            //               join hi in _context.SrmHistoryPrices on new { Vendor = (!string.IsNullOrWhiteSpace(v.SapVendor)) ? v.SapVendor : v.SrmVendor1, Matnr = (!string.IsNullOrWhiteSpace(m.SapMatnr)) ? m.SapMatnr : m.SrmMatnr1 } equals new { Vendor = hi.Vendor, Matnr = hi.Matnr } into hig
            //               from hi in hig.DefaultIfEmpty()
            //               join mu in _context.SrmMeasureUnits on rm.Unit equals mu.MeasureId into mug
            //               from mu in mug.DefaultIfEmpty()
            //               join s in _context.SrmStatuses on q.Status equals s.Status
            //               join u1 in _context.AspNetUsers on q.CreateBy equals u1.UserName into u1g
            //               from u1 in u1g.DefaultIfEmpty()

            var eqps = from eq in _context.SrmEqpHs
                       join p in _context.SrmPoHs on eq.PoId equals p.PoId into pg
                       from phg in pg.DefaultIfEmpty()
                       join l in _context.SrmPoLs on new { no = eq.no, PoId = phg.PoId } equals new { no = l.PoLId.ToString(), PoId = l.PoId }
                        into pl
                       from plg in pl.DefaultIfEmpty()


                       join m in _context.SrmMatnrs on plg.MatnrId equals m.MatnrId

                         into mg
                       from mgg in mg.DefaultIfEmpty()


                       join st in _context.SrmStatuses on eq.Status equals st.Status

                       //var eqps = from eq in _context.SrmEqpHs
                       //           from p in _context.SrmPoHs 
                       //           from l in _context.SrmPoLs
                       //           from m in _context.SrmMatnrs
                       //           from st in _context.SrmStatuses
                       //           where eq.PoId == p.PoId
                       //           && p.PoId == l.PoId
                       //           && eq.no == l.PoLId.ToString()
                       //           && l.MatnrId == m.MatnrId
                       //           && eq.Status == st.Status

                       //var eqps = from eq in _context.SrmEqpHs
                       //           join p in _context.SrmPoHs on eq.PoId equals p.PoId
                       //           join l in _context.SrmPoLs on p.PoId equals l.PoId
                       //           join m in _context.SrmMatnrs on l.MatnrId equals m.MatnrId
                       select new ViewSrmEqpH
                       {
                           EqpNum = eq.EqpNum,
                           EqpId = eq.EqpId,
                           EcreateDate = DateTime.Parse(eq.CreateDate.ToString()).ToString("yyyy/MM/dd"),//eq.CreateDate,
                           poNum = (!string.IsNullOrWhiteSpace(phg.PoNum))? phg.PoNum:eq.WoNum,//p.PoNum,
                           matnr = mgg.SapMatnr,
                           Description = (!string.IsNullOrWhiteSpace(plg.Description)) ? plg.Description : eq.Description,// l.Description,
                           no = (!string.IsNullOrWhiteSpace(plg.PoLId.ToString())) ? plg.PoLId.ToString() : eq.no ,//l.PoLId.ToString(),
                           vendorid = phg.VendorId, //p.VendorId,
                           Status = eq.Status,
                           StatusDesc = st.StatusDesc,
                           vendor = eq.LastUpdateBy

                       }; 
            return eqps
                       //.Where(r => r.vendorid == e.vendorid)
                       //.AndIfHaveValue(e.vendorid, p => p.vendorid == e.vendorid)
                       //.AndIfCondition(vendorid != 0 , p => p.vendorid == vendorid)
                       .AndIfCondition(vendorid != 0, p => p.vendor == e.vendor) //20220209 為取得博格拋轉肇因單位為該供應商及該供應商填的單
                       .AndIfCondition(!string.IsNullOrWhiteSpace(e.txtSN), p => p.EqpNum == e.txtSN) //20220126取vendorid
                       .AndIfCondition(!string.IsNullOrWhiteSpace(e.woNum), p => p.WoNum == e.woNum)
                       .AndIfCondition(!string.IsNullOrWhiteSpace(e.matnr), p => p.matnr == e.matnr)
                       .AndIfCondition(!string.IsNullOrWhiteSpace(e.no), p => p.no == e.no)
                       .AndIfCondition(e.status != 0, p => p.Status == e.status)//????
                       //.AndIfHaveValue(e.txtSN, r => r.EqpNum == e.txtSN)
                       //.AndIfHaveValue(e.woNum, r => r.WoNum == e.woNum)
                       //.AndIfHaveValue(e.matnr, r => r.matnr == e.matnr)
                       //.AndIfHaveValue(e.no, r => r.no == e.no)
                       .Distinct().ToArray();
        }
        #region 博格開單
        //public ViewSrmEqpH GetDataByEqpIdFromBorg(int EqpId)
        //{
        //    var eqp = (from eq in _context.SrmEqpHs
        //               from p in _context.SrmPoHs
        //               from l in _context.SrmPoLs
        //               from m in _context.SrmMatnrs
        //               from ek in _context.SrmEkgries
        //               from status in _context.SrmStatuses
        //               from u in _context.AspNetUsers
        //               where eq.PoId == p.PoId
        //               && p.PoId == l.PoId
        //               && eq.no == l.PoLId.ToString()
        //               && l.MatnrId == m.MatnrId
        //               && p.Buyer == ek.Ekgry
        //               && eq.Status == status.Status
        //               && eq.CreateBy == u.UserName
        //               && eq.EqpId.Equals(EqpId)
        //               select new ViewSrmEqpH
        //               {
        //                   EqpNum = eq.EqpNum,
        //                   CreateBy = eq.CreateBy,
        //                   CreateDate = eq.CreateDate,
        //                   WoNum = eq.WoNum,
        //                   PoId = eq.PoId,
        //                   matnr = m.SapMatnr,//要判斷
        //                   no = eq.no,
        //                   MatnrId = l.MatnrId,
        //                   ekgry = p.Buyer,
        //                   ekgryid = ek.Empid,
        //                   Description = l.Description,
        //                   DeliveryDate = eq.DeliveryDate,
        //                   PoQty = eq.PoQty,
        //                   NgQty = eq.NgQty,
        //                   Version = eq.Version,
        //                   NgDesc = eq.NgDesc,
        //                   CauseAnalyses = eq.CauseAnalyses,
        //                   Dispoaition = (eq.Dispoaition == "Y") ? "是" : "否",
        //                   CauseDept = eq.CauseDept,
        //                   QcDispoaition = eq.QcDispoaition,
        //                   QdrNum = eq.QdrNum,
        //                   QcNote = eq.QcNote,
        //                   ReworkCosts = eq.ReworkCosts,
        //                   PeAction = eq.PeAction,
        //                   Status = status.Status,
        //                   CreateByName = u.Name
        //               }).First();
        //    return eqp;
        //}

        #endregion

        #region SRM開單
        public ViewSrmEqpH GetDataByEqpId(int EqpId)
        {
            var eqp = (from eq in _context.SrmEqpHs
                       from p in _context.SrmPoHs
                       from l in _context.SrmPoLs
                       from m in _context.SrmMatnrs
                       from ek in _context.SrmEkgries
                       from status in _context.SrmStatuses
                       from u in _context.AspNetUsers 
                       where eq.PoId == p.PoId
                       && p.PoId == l.PoId
                       && eq.no == l.PoLId.ToString()
                       && l.MatnrId == m.MatnrId
                       && p.Buyer == ek.Ekgry
                       && eq.Status == status.Status
                       //&& eq.CreateBy == u.UserName
                       && eq.LastUpdateBy == u.UserName
                       && eq.EqpId.Equals(EqpId)
                       select new ViewSrmEqpH
                       {
                           EqpNum = eq.EqpNum,
                           CreateBy = eq.CreateBy,
                           CreateDate = eq.CreateDate,
                           WoNum = eq.WoNum,
                           PoId = eq.PoId,
                           matnr = (!string.IsNullOrWhiteSpace(m.SapMatnr))? m.SapMatnr:m.SrmMatnr1,//要判斷
                           no = eq.no,
                           MatnrId = l.MatnrId,
                           ekgry = p.Buyer,
                           ekgryid = ek.Empid,
                           Description = l.Description,
                           DeliveryDate = eq.DeliveryDate,
                           PoQty = eq.PoQty,
                           NgQty = eq.NgQty,
                           Version = eq.Version,
                           NgDesc = eq.NgDesc,
                           CauseAnalyses = eq.CauseAnalyses,
                           Dispoaition = (eq.Dispoaition == "Y")?"是": "否",
                           CauseDept =eq.CauseDept,
                           QcDispoaition = eq.QcDispoaition,
                           QdrNum = eq.QdrNum,
                           QcNote = eq.QcNote,
                           ReworkCosts = eq.ReworkCosts,
                           PeAction = eq.PeAction,
                           Status = status.Status,
                           CreateByName = u.Name
                       }).First();
            return eqp;
        }
        #endregion
        public void UpdateStatus(int status, SrmEqpH eqpH)
        {
            int eqpid = eqpH.EqpId;
            var e = _srmEqpHRepository.Get(r => r.EqpId == eqpid).First();

            switch ((Status)status)
            {
               
                case Status.刪除:
                    if ((Status)eqpH.Status != Status.初始 && (Status)eqpH.Status != Status.啟動)
                    {
                        throw new Exception($"非初始或啟動狀態無法{((Status)status).ToString()}");
                    }
                    break;
                case Status.啟動:
                    if ((Status)eqpH.Status != Status.初始)
                    {
                        throw new Exception($"非初始狀態無法{((Status)status).ToString()}");
                    }
                    break;

                default:
                    throw new Exception($"未定義{((Status)status).ToString()}");
            }
            e.Status = status;
            e.LastUpdateDate = eqpH.LastUpdateDate;
            e.LastUpdateBy = eqpH.LastUpdateBy;
            using (SRMContext db = new SRMContext())
            {
                db.Update(e);
                db.SaveChanges();
                //return e;
            }

            //var eqps = _srmEqpHRepository.Get(r => r.EqpId == eqpH.EqpId);
            //foreach (var eqp in eqps)
            //{
            //    eqp.Status = status;
            //    eqp.LastUpdateDate = eqpH.LastUpdateDate;
            //    eqp.LastUpdateBy = eqpH.LastUpdateBy;
            //    using (SRMContext db = new SRMContext())
            //    {
            //        db.Update(eqp);
            //        db.SaveChanges();
            //        //return rfq;
            //    }
            //}
            ////_context.UpdateRange(eqps);
            ////_context.SaveChanges();
            ////using (SRMContext db = new SRMContext())
            ////{
            ////    db.Update(eqp);
            ////    db.SaveChanges();
            ////    //return rfq;
            ////}
        }
        public int GetVendorId(QueryEqp e)
        {
            var vendorid = 0;
            var vendor = (from v in _context.SrmVendors
                          where (v.SapVendor == e.vendor || v.SrmVendor1 == e.vendor)
                          select new
                          {
                              VendorId = v.VendorId,
                          });
            vendorid = vendor.Select(r => r.VendorId).First();
            return vendorid;

        }

        public string GetVendorName(QueryEqp e)
        {
            var vendorname = string.Empty;
            var user = (from u in _context.AspNetUsers
                          where (u.UserName == e.vendor)
                          select new
                          {
                              username = u.Name,
                          });
            vendorname = user.Select(r => r.username).First();
            return vendorname;

        }

        public string  GetEqptxtSN(SrmEqpH e)
        {
            string txtSN = string.Empty;
            var eqp = (from eq in _context.SrmEqpHs
                          where (eq.EqpId == e.EqpId)
                          select new
                          {
                              txtSN = eq.EqpNum,
                          });
            if (eqp.Count()>0)
            {
                txtSN = eqp.Select(r => r.txtSN).First();
            }
            return txtSN;
        }
        public bool CheckIfExists(SrmEqpH e)
        {
            //同採單 同料號 狀態非刪除
            bool ifexist = true;
            var eqp = (from eq in _context.SrmEqpHs
                       where ((eq.WoNum == e.WoNum) && (eq.MatnrId == e.MatnrId)
                       && eq.Status != 19
                       )
                        
                       select new
                       {
                           txtSN = eq.EqpNum,
                       });
            if (eqp.Count() == 0) 
            {
                ifexist = false;
            } 
            return ifexist;
        }
        public bool CheckIfStart(SrmEqpH e)
        {
            //同採單 同料號
            bool ifstart = false;
            var eqp = (from eq in _context.SrmEqpHs
                       where ((eq.WoNum == e.WoNum) && (eq.MatnrId == e.MatnrId))
                       select new
                       {
                           txtSN = eq.EqpNum,
                           status = eq.Status
                       })
                       .Where(r => r.status == 7);
            if (eqp.Count() > 0)
            {
                ifstart = true;
            }
            return ifstart;
        }
        #region formaildata
        public SrmEqpH[] Get(QueryEqp query)
        {
            var EqpQ = _context.SrmEqpHs.AsQueryable()
                .AndIfHaveValue(query.eqpid, r => r.EqpId == query.eqpid)
                .OrderBy(r => r.EqpId);
            return EqpQ.ToArray();
        }
        #endregion
        #region
        public string GetEkgryMail(SrmEqpH e)
        {
            string Email = string.Empty;
            var eqp = (
                       from p in _context.SrmPoHs
                       from ek in _context.SrmEkgries
                       from u in _context.AspNetUsers
                       where 
                       p.Buyer == ek.Ekgry
                       && ek.Ekgry == u.UserName
                       && p.PoId.Equals(e.PoId)
                       && u.IsActive.Equals(1) // 表啟用中
                       select new
                       {
                           Email = u.Email
                       });
            if (eqp.Count()>0)
            {
                Email = eqp.Select(r => r.Email).First();
            }
            
            return Email;
        }
        #endregion
        #region 產生單號
        public string GettxtSN() 
        {
            DateTime today = DateTime.Today.Date;
            int year = today.Year;
            string txtSN = string.Empty; 
            string no = "PS" + year.ToString().Substring(2, 2); //string.Empty;
            var eqp = (from e in _context.SrmEqpHs
                       where e.EqpNum.Substring(0,4).Equals(no)
                       orderby e.EqpNum descending
                          select new
                          {
                              EqpNum = e.EqpNum
                          });
            if (eqp.Count() == 0) 
            {
                txtSN = no + "001";
            }
            else 
            {
                //PS21001
                txtSN = eqp.Select(r =>r.EqpNum).First();
                txtSN = (int.Parse(txtSN.Replace(no, "")) + 1).ToString().PadLeft(3, '0');
                txtSN = no + txtSN;
                //txtSN = (int.Parse(txtSN.Replace(txtsn, "")) + 1).ToString().PadLeft(3, '0');
            }          
            return txtSN;
        }
        #endregion

        public string GetMatnr(SrmEqpH e)
        {
            string matnr = string.Empty;
            var matnrt = (
                       from m in _context.SrmMatnrs
                       where m.MatnrId.Equals(e.MatnrId)
                       select new
                       {
                           //SrmMatnr  = m.SrmMatnr1,
                           SapMatnr = m.SapMatnr    //非虛擬料號才會進SAP
                       });
            if (matnrt.Count() > 0)
            {
                matnr = matnrt.Select(r => r.SapMatnr).First();
            }
            return matnr;
        }
        public int GetEqpStatus(SrmEqpH eqpH)
        {
            var eqpstatus = 0;
            var eqp = _context.SrmEqpHs.Where(p => p.EqpId == eqpH.EqpId).ToList();
            eqpstatus = eqp.Select(r => r.Status).First().Value;
            return eqpstatus;
        }
        #region 再取一次eqp table資料
        public SrmEqpH GetEqp(SrmEqpH eqpH)
        {
            var eqp = _context.SrmEqpHs
.Where(p => p.EqpId == eqpH.EqpId).ToList().First();
            return eqp;
        }
        #endregion
        //public int GetCaseId(QueryEqp e)
        //{
        //    int caseid = 0;
        //    var eqp = (from eq in _context.SrmEqpHs
        //                  where eq.EqpNum == e.txtSN
        //                  select new
        //                  {
        //                      caseid = eq.Caseid.Value
        //                  });
        //    caseid = eqp.Select(r => r.caseid).First();
        //    return caseid;

        //}
        //public int GetCaseId(string txtSN)
        //{
        //    int caseid = 0;
        //    var eqp = (from e in _context.SrmEqpHs
        //                  where e.EqpNum == txtSN
        //               select new
        //                  {
        //                      caseid = e.Caseid.Value,
        //                  });
        //    if (eqp.Count()>0)
        //    {
        //        caseid = eqp.Select(r => r.caseid).First();
        //    }
        //    return caseid;
        //}
        public ViewSrmEqpH GetEqpH(SrmEqpH q)
        {
            SrmEqpH eq = new SrmEqpH();
            var eqp = (from poh in _context.SrmPoHs 
                       join pol in _context.SrmPoLs on poh.PoId equals pol.PoId

                          where (poh.PoId == q.PoId)
                           &&　(pol.MatnrId == q.MatnrId)
                           &&  (poh.PoId == q.PoId)
                           && (pol.MatnrId == q.MatnrId)
                          select new ViewSrmEqpH
                          {
                              WoNum = poh.PoNum,
                              no = pol.PoLId.ToString()
                          }).First();
            return eqp;
        }
    }
}