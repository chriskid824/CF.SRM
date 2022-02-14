using AutoMapper;

using Convience.Entity.Data;
using Convience.Entity.Entity;
using Convience.Entity.Entity.Identity;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.Filestorage.Abstraction;
using Convience.JwtAuthentication;
using Convience.Model.Constants.SystemManage;
using Convience.Model.Models;
using Convience.Model.Models.ContentManage;
using Convience.Model.Models.SRM;
using Convience.Model.Models.SystemManage;
using Convience.Service.ContentManage;
using Convience.Util.Extension;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmPoService
    {
        public bool CheckAllReply(int id);

        /// <summary>
        /// 取得全部角色
        /// </summary>
        public IEnumerable<ViewSrmPoH> GetAll();

        public IEnumerable<ViewSrmPoH> GetAll(QueryPoList query);

        public IEnumerable<ViewSrmPoL> GetPoL(QueryPoList query);
        public PagingResultModel<ViewSrmPoPoL> GetPoPoL(QueryPoList query, int page, int size);

        public bool UpdateStatus(int id, int status);
        public List<SapResultData> UpdateSapData(SapPoData data, string userName);
        public string UpdatePoLDoc(int matnr_id,string des, int vendor_id, List<BaseFileData> fdList, string userid);
        public List<BaseFileData> GetMatnrDocListFromSap(string ponum, int polid, List<T_DRAD> dataSet, bool isvendor);
        public void addLog(int poid, int polid, string filename, string ip, string username);
        public PagingResultModel<ViewSrmDownloadLog> GetDownloadList(QueryPoDownloadLogList query);
    }

    public class SrmPoService : ISrmPoService
    {
        private readonly SRMContext _context;

        //private readonly IMapper _mapper;
        private readonly IRepository<SrmPoH> _srmPohRepository;

        private readonly IRepository<SrmPoL> _srmPolRepository;
        private readonly IFileManageService _fileService;
        private IMapper _mapper;

        public SrmPoService(
            //IMapper mapper,
            IFileManageService fileService,
            IRepository<SrmPoH> srmPohRepository, IRepository<SrmPoL> srmPolRepository, SRMContext context, IMapper mapper)
        {
            _mapper = mapper;
            _srmPohRepository = srmPohRepository;
            _srmPolRepository = srmPolRepository;
            _context = context;
            _fileService = fileService;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }

        public bool CheckAllReply(int id)
        {
            //必須是待接收 或是已接收狀態
            if (_context.SrmPoHs.Find(id).Status == 21 || _context.SrmPoHs.Find(id).Status == 11)
            {
                if (_context.SrmPoLs.Any(p => p.PoId == id && p.ReplyDeliveryDate == null))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public IEnumerable<ViewSrmPoH> GetAll()
        {
            return GetAll(new QueryPoList());
        }

        public IEnumerable<ViewSrmPoH> GetAll(QueryPoList query)
        {
            var result = (from poh in _context.SrmPoHs
                          join status in _context.SrmStatuses on poh.Status equals status.Status
                          join vendor in _context.SrmVendors on poh.VendorId equals vendor.VendorId              
                          select new ViewSrmPoH
                          {
                              PoId = poh.PoId,
                              PoNum = poh.PoNum,
                              VendorId = poh.VendorId,
                              Status = poh.Status,
                              Buyer = poh.Buyer,
                              Org = poh.Org,
                              DocDate = poh.DocDate,
                              ReplyDate = poh.ReplyDate,
                              CreateDate = poh.CreateDate,
                              CreateBy = poh.CreateBy,
                              LastUpdateDate = poh.LastUpdateDate,
                              LastUpdateBy = poh.LastUpdateBy,
                              TotalAmount = poh.TotalAmount,
                              StatusDesc = status.StatusDesc,
                              VendorName = vendor.VendorName,
                              SapVendor = vendor.SapVendor,
                              //SrmPoLs = poh.SrmPoLs,
                          })
                          .AndIfCondition(!query.user.GetIsVendor(), p => query.user.GetUserWerks().Contains(p.Org.ToString()))
                          .AndIfCondition(query.user.GetIsVendor(), p => p.SapVendor == query.user.GetVendorId())
                          .AndIfHaveValue(query.poId,p=>p.PoId==query.poId)
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.buyer), p => p.Buyer.IndexOf(query.buyer) > -1)
                .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
                .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();
            // List<string> numberList = _context.ViewSrmFileRecords.Where(m=> _context.ViewSrmFileRecords.Where(p => p.RecordLId == null).Select(p => p.Number).Except(m.Number)).Select(m=>m.Number).ToList();
            FileQueryModel filequery = new FileQueryModel() { Directory= "PoFiles",Size=200,Page=1 };
            var files = _fileService.GetContentsAsync(filequery);
            List<string> fileDirList = files.Result.Select(p => p.Name).ToList();
            var numberList = from c in _context.ViewSrmFileRecords
    where !(from o in _context.ViewSrmFileRecords
            where o.RecordLId ==null
            select o.Number )
           .Contains(c.Number)
    select c.Number;
            result.ForEach(p =>
            {
                p.hasFile = fileDirList.Contains(p.PoNum);
                p.SrmPoLs = (from l in _context.SrmPoLs
                             join h in _context.SrmPoHs on l.PoId equals h.PoId
                             join status in _context.SrmStatuses on l.Status equals status.Status
                             join vendor in _context.SrmVendors on h.VendorId equals vendor.VendorId
                             join matnr in _context.SrmMatnrs on l.MatnrId equals matnr.MatnrId into matnrInfo
                             from matnr in matnrInfo.DefaultIfEmpty()
                             select new ViewSrmPoL
                             {
                                 PoNum = h.PoNum,
                                 PoLId = l.PoLId,
                                 PoId = l.PoId,
                                 MatnrId = l.MatnrId,
                                 Description = l.Description,
                                 Qty = l.Qty,
                                 Price = l.Price,
                                 DeliveryDate = l.DeliveryDate,
                                 ReplyDeliveryDate = l.ReplyDeliveryDate,
                                 DeliveryPlace = l.DeliveryPlace,
                                 CriticalPart = l.CriticalPart,
                                 InspectionTime = l.InspectionTime,
                                 Status = h.Status,
                                 VendorId = h.VendorId,
                                 VendorName = vendor.VendorName,
                                 TotalAmount = h.TotalAmount,
                                 Buyer = h.Buyer,
                                 StatusDesc = status.StatusDesc,
                                 Matnr = matnr.SapMatnr,
                                 Org = p.Org
                             }).Where(l => l.PoId == p.PoId)
                             //.AndIfCondition(query.isNeedData, p => _context.ViewSrmFileRecords)
                             .ToList();
                if (query.dataStatus == 2)
                {
                    p.SrmPoLs = (from c in p.SrmPoLs
                                                          where !(from o in numberList
                                                                  select o)
                                                                 .Contains(c.PoNum + '-' + c.PoLId)
                                                          select c).ToList();
                }
                else if (query.dataStatus == 1)
                {
                    p.SrmPoLs = (from c in p.SrmPoLs
                                                          where (from o in numberList
                                                                  select o)
                                                                 .Contains(c.Number)
                                                          select c).ToList();
                }

            });
            return result.Where(p=>p.SrmPoLs.Count()>0).ToList();
        }

        public IEnumerable<SrmPoH> GetMatnrById(int id)
        {
            return _srmPohRepository.Get(r => r.PoId == id);
        }

        public IEnumerable<ViewSrmPoL> GetPoL(QueryPoList query)
        {
            var result = (from l in _context.SrmPoLs
                          join h in _context.SrmPoHs on l.PoId equals h.PoId
                          join status in _context.SrmStatuses on l.Status equals status.Status
                          join vendor in _context.SrmVendors on h.VendorId equals vendor.VendorId
                          join matnr in _context.SrmMatnrs on l.MatnrId equals matnr.MatnrId into matnrInfo
                          from matnr in matnrInfo.DefaultIfEmpty()
                          select new ViewSrmPoL
                          {
                              PoNum = h.PoNum,
                              PoLId = l.PoLId,
                              PoId = l.PoId,
                              MatnrId = l.MatnrId,
                              Description = l.Description,
                              Qty = l.Qty,
                              Price = l.Price,
                              DeliveryDate = l.DeliveryDate,
                              ReplyDeliveryDate = l.ReplyDeliveryDate,
                              DeliveryPlace = l.DeliveryPlace,
                              CriticalPart = l.CriticalPart,
                              InspectionTime = l.InspectionTime,
                              Status = h.Status,
                              VendorId = h.VendorId,
                              VendorName = vendor.VendorName,
                              SapVendor = vendor.SapVendor,
                              TotalAmount = h.TotalAmount,
                              Buyer = h.Buyer,
                              StatusDesc = status.StatusDesc,
                              Matnr = matnr.SapMatnr,
                              Org = h.Org
                          })
                          .AndIfCondition(!query.user.GetIsVendor(), p => query.user.GetUserWerks().Contains(p.Org.ToString()))
                          .AndIfCondition(query.user.GetIsVendor(), p => p.SapVendor == query.user.GetVendorId())
                              .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
                              .AndIfCondition(query.poLId != 0, p => p.PoLId == query.poLId)
                .AndIfHaveValue(query.replyDeliveryDate_s, p => p.DeliveryDate >= query.replyDeliveryDate_s.Value.Date)
                .AndIfHaveValue(query.replyDeliveryDate_e, p => p.DeliveryDate <= query.replyDeliveryDate_e.Value.AddDays(1).Date)
                .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();

            //var result = _context.SrmPoLs.Join(
            //    _context.SrmPoHs,
            //    l => l.PoId,
            //    h => h.PoId,
            //    (l, h) => new ViewSrmPoL
            //    {
            //        PoNum = h.PoNum,
            //        PoLId = l.PoLId,
            //        PoId = l.PoId,
            //        MatnrId = l.MatnrId,
            //        Description = l.Description,
            //        Qty = l.Qty,
            //        Price = l.Price,
            //        DeliveryDate = l.DeliveryDate,
            //        ReplyDeliveryDate = l.ReplyDeliveryDate,
            //        DeliveryPlace = l.DeliveryPlace,
            //        CriticalPart = l.CriticalPart,
            //        InspectionTime = l.InspectionTime,
            //        Status = h.Status,
            //        VendorId = h.VendorId,
            //        TotalAmount = h.TotalAmount,
            //        Buyer = h.Buyer,
            //    }
            //    )
            //    //.AndIfCondition(!string.IsNullOrWhiteSpace(query.buyer), p => p.Buyer.IndexOf(query.buyer) > -1)
            //    .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
            //    .AndIfHaveValue(query.replyDeliveryDate_s, p => p.DeliveryDate >= query.replyDeliveryDate_s.Value.Date)
            //    .AndIfHaveValue(query.replyDeliveryDate_e, p => p.DeliveryDate <= query.replyDeliveryDate_e.Value.AddDays(1).Date)
            //    .AndIfCondition(query.status != 0, p => p.Status == query.status).ToList();

            result.ForEach(p =>
            {
                p.RemainQty = p.Qty - _context.SrmDeliveryLs.Where(q => q.PoId == p.PoId && q.PoLId == p.PoLId).Sum(q => q.DeliveryQty);
            });

            //.AndIfCondition(query.status != 0, p => p.Status == query.status);
            return result.Where(p => p.RemainQty > 0).ToList();
        }

        public IEnumerable<SrmPoL> GetPolById(int id)
        {
            return _srmPolRepository.Get(r => r.PoId == id);
        }

        public bool UpdateStatus(int id, int status)
        {
            SrmPoH data = _context.SrmPoHs.Find(id);
            data.Status = status;
            if (status == 11)
            {
                data.ReplyDate = DateTime.Now;
            }
            _context.SrmPoHs.Update(data);
            var LList = _context.SrmPoLs.Where(p => p.PoId == data.PoId);
            foreach (var item in LList)
            {
                item.Status = status;
                _context.SrmPoLs.Update(item);
            }
            _context.SaveChanges();
            return true;
        }
        public PagingResultModel<ViewSrmPoPoL> GetPoPoL(QueryPoList query, int page, int size)
        {
            int skip = (page - 1) * size;

            var result = (from l in _context.SrmPoLs
                          join h in _context.SrmPoHs on l.PoId equals h.PoId
                          join status in _context.SrmStatuses on l.Status equals status.Status
                          join vendor in _context.SrmVendors on h.VendorId equals vendor.VendorId
                          join matnr in _context.SrmMatnrs on l.MatnrId equals matnr.MatnrId into matnrInfo
                          from matnr in matnrInfo.DefaultIfEmpty()
                          select new ViewSrmPoPoL
                          {
                              PoNum = h.PoNum,
                              PoLId = l.PoLId,
                              PoId = l.PoId,
                              MatnrId = l.MatnrId,
                              Description = l.Description,
                              Qty = l.Qty,
                              Price = l.Price,
                              DeliveryDate = l.DeliveryDate,
                              ReplyDeliveryDate = l.ReplyDeliveryDate,
                              DeliveryPlace = l.DeliveryPlace,
                              CriticalPart = l.CriticalPart,
                              InspectionTime = l.InspectionTime,
                              Status = h.Status,
                              VendorId = h.VendorId,
                              VendorName = vendor.VendorName,
                              SapVendor = vendor.SapVendor,
                              TotalAmount = h.TotalAmount,
                              Buyer = h.Buyer,
                              StatusDesc = status.StatusDesc,
                              Matnr = matnr.SapMatnr,
                              Org = h.Org,
                              DocDate = h.DocDate,
                              ReplyDate = h.ReplyDate,
                              CreateDate = h.CreateDate,
                          })
              .AndIfCondition(!query.user.GetIsVendor(), p => query.user.GetUserWerks().Contains(p.Org.ToString()))
              .AndIfCondition(query.user.GetIsVendor(), p => p.SapVendor == query.user.GetVendorId())
                  .AndIfCondition(!string.IsNullOrWhiteSpace(query.poNum), p => p.PoNum.IndexOf(query.poNum) > -1)
    //.AndIfCondition(query.poLId != 0, p => p.PoLId == query.poLId)
    //.AndIfHaveValue(query.replyDeliveryDate_s, p => p.DeliveryDate >= query.replyDeliveryDate_s.Value.Date)
    //.AndIfHaveValue(query.replyDeliveryDate_e, p => p.DeliveryDate <= query.replyDeliveryDate_e.Value.AddDays(1).Date)
    .AndIfCondition(query.status != 0, p => p.Status == query.status)
                .AndIfHaveValue(query.buyer, p => p.Buyer == query.buyer).ToList();

            var r = result.AsQueryable().Skip(skip).Take(size).ToArray();//result.Skip(skip).Take(size);
            return new PagingResultModel<ViewSrmPoPoL>
            {
                Data = r,
                Count = result.Count()
            };
        }
        public List<SapResultData> UpdateSapData(SapPoData data, string userName)
        {
            List<SapResultData> result = new List<SapResultData>();
            data.T_EKKO.ForEach(po =>
            {
                SapResultData r = new SapResultData() { Id = po.EBELN, Type = "採購單" };
                //採購單
                if (!_context.SrmPoHs.Any(p => p.PoNum == po.EBELN))
                {
                    //供應商
                    if (_context.SrmVendors.Any(v => v.SapVendor == po.LIFNR))
                    {
                        //採購項次-即將匯入
                        if (data.T_EKPO.Any(ekpo => ekpo.EBELN == po.EBELN))
                        {
                            List<T_EKPO> ekList = data.T_EKPO.Where(e => e.EBELN == po.EBELN).ToList();
                            int ekcount = 0;
                            ekList.ForEach(ek =>
                            {
                                if (_context.SrmMatnrs.Any(m => m.SapMatnr == ek.MATNR))
                                {
                                    ekcount++;
                                }
                            });

                            //採購項次料號
                            if (ekcount > 0)
                            {
                                int vendorid = _context.SrmVendors.FirstOrDefault(p => p.SapVendor == po.LIFNR).VendorId;
                                DateTime now = DateTime.Now;
                                SrmPoH poH = new SrmPoH()
                                {
                                    PoNum = po.EBELN,
                                    Status = 21,
                                    VendorId = vendorid,
                                    TotalAmount = Convert.ToInt32(Convert.ToDouble(po.RLWRT)),
                                    Buyer = po.EKGRP,
                                    Org = po.EKORG,
                                    DocDate = po.BEDAT,
                                    CreateDate = now,
                                    CreateBy = userName,
                                    LastUpdateDate = now,
                                    LastUpdateBy = userName,
                                };
                                _context.SrmPoHs.Add(poH);
                                r.OutCome = "成功";
                                result.Add(r);
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                        }

                    }
                    else
                    {
                        r.OutCome = "失敗";
                        r.Reason = "供應商 " + po.LIFNR + " 不存在";
                        result.Add(r);
                    }
                }
                else
                {
                    r.OutCome = "失敗";
                    r.Reason = "該採購單號已存在";
                    result.Add(r);
                }

            });
            _context.SaveChanges();
            data.T_EKPO.ForEach(pol =>
            {
                SapResultData r = new SapResultData() { Id = pol.EBELN, LId = pol.EBELP, Type = "採購項次" };
                //採購單號
                if (_context.SrmPoHs.Any(p => p.PoNum == pol.EBELN))
                {
                    SrmPoH poH = _context.SrmPoHs.FirstOrDefault(h => h.PoNum == pol.EBELN);
                    //採購項次
                    if (!_context.SrmPoLs.Any(l => l.PoId == poH.PoId && l.PoLId == pol.EBELP))
                    {
                        //料號
                        if (_context.SrmMatnrs.Any(m => m.SapMatnr == pol.MATNR))
                        {
                            int matnrid = pol.MATNR==""?0: _context.SrmMatnrs.FirstOrDefault(p => p.SapMatnr == pol.MATNR).MatnrId;
                            SrmPoL poL = new SrmPoL()
                            {
                                PoLId = pol.EBELP,
                                PoId = poH.PoId,
                                MatnrId = matnrid,
                                Description = pol.MAKTX,
                                Qty = Convert.ToInt32(Convert.ToDouble(pol.MENGE)),
                                Price = pol.NETPR,
                                DeliveryDate = pol.EINDT,
                                DeliveryPlace = pol.LGOBE,
                                CriticalPart = pol.KZKRI,
                                InspectionTime = 1,
                                Status = 21,
                                WoNum = pol.AUFNR,
                            };
                            _context.SrmPoLs.Add(poL);
                            r.OutCome = "成功";
                            result.Add(r);
                        }
                        else
                        {
                            r.OutCome = "失敗";
                            r.Reason = "料號 " + pol.MATNR + " 不存在";
                            result.Add(r);
                        }
                    }
                    else
                    {
                        r.OutCome = "失敗";
                        r.Reason = "該採購項次已存在";
                        result.Add(r);
                    }
                }
                else
                {
                    //如果已經提示過採購單號無法匯入 就不提示採購項次
                    if (data.T_EKKO.Any(ko => ko.EBELN == pol.EBELN))
                    {
                        if (!_context.SrmMatnrs.Any(m => m.SapMatnr == pol.MATNR))
                        {
                            r.OutCome = "失敗";
                            r.Reason = "料號 " + pol.MATNR + " 不存在";
                            result.Add(r);
                        }
                    }
                    else
                    {
                        r.OutCome = "失敗";
                        r.Reason = "該採購單號不存在";
                        result.Add(r);
                    }
                }
            });
            _context.SaveChanges();
            return result;
        }

        public string UpdatePoLDoc(int matnr_id, string des, int vendor_id, List<BaseFileData> fdList,string userid)
        {
            foreach (var item in fdList)
            {
                SrmMatnrDoc doc = _context.SrmMatnrDocs.Where(p => p.Filename == item.Name&&p.VendorId==vendor_id)
                    .AndIfCondition(matnr_id==0,p=>p.Description==des)
                    .AndIfCondition(matnr_id!=0,p=> p.MatnrId == matnr_id)
                    .FirstOrDefault();
                if (doc == null)
                {
                    doc = new SrmMatnrDoc() { 
                        MatnrId=matnr_id,
                        Description=des,
                        VendorId=vendor_id,
                        Filename=item.Name,
                        Active=item.active,
                        CreateBy= userid,
                        LastUpdateBy=userid
                    };
                    _context.SrmMatnrDocs.Add(doc);
                }
                else
                {
                    doc.Active = item.active;
                    doc.LastUpdateBy = userid;
                    doc.LastUpdateDate = DateTime.Now;
                    _context.SrmMatnrDocs.Update(doc);
                }
            }
            _context.SaveChanges();
            return null;
        }

        public List<BaseFileData> GetMatnrDocListFromSap(string ponum, int polid, List<T_DRAD> dataSet,bool isvendor)
        {
            List<BaseFileData> fdList = new List<BaseFileData>();
            int poid = _context.SrmPoHs.Where(p => p.PoNum == ponum).Select(p=>p.PoId).FirstOrDefault();
            int vendor_id = _context.SrmPoHs.Where(p => p.PoNum == ponum).Select(p => p.VendorId).FirstOrDefault();
            if (poid <= 0||vendor_id<=0) return null;
            int matnr_id = _context.SrmPoLs.Where(p => p.PoId == poid && p.PoLId == polid).Select(p => p.MatnrId).FirstOrDefault();
            string des= _context.SrmPoLs.Where(p => p.PoId == poid && p.PoLId == polid).Select(p => p.Description).FirstOrDefault();
            //if (matnr_id <= 0) return null;
            List<SrmMatnrDoc> doclist = _context.SrmMatnrDocs.Where(p => p.VendorId == vendor_id)
                .AndIfCondition(matnr_id == 0, p => p.Description == des)
                .AndIfCondition(matnr_id != 0, p => p.MatnrId == matnr_id)
                .ToList();
            foreach (var set in dataSet)
            {

                BaseFileData fd = new BaseFileData()
                {
                    Name = set.DOKNR,
                    active = false,
                };
                if (doclist != null)
                {
                    SrmMatnrDoc doc = doclist.Where(p => p.Filename == set.DOKNR).FirstOrDefault();
                    if (doc != null) fd.active = doc.Active.HasValue ? doc.Active.Value : false;
                }

                if (!isvendor || (isvendor && fd.active))
                {
                    fdList.Add(fd);
                }

                //此處要有從資料庫抓取資料的紀錄
            }
            return fdList;
        }
        public void addLog(int poid, int polid, string filename,string ip,string username)
        {
            SrmDownloadLog log = new SrmDownloadLog()
            {
                PoId=poid,
                PoLId=polid,
                FileName=filename,
                Ip=ip,
                CreateBy=username,
                LastUpdateBy=username
            };
            _context.SrmDownloadLogs.Add(log);
            _context.SaveChanges();
        }
        public PagingResultModel<ViewSrmDownloadLog> GetDownloadList(QueryPoDownloadLogList query)
        {
            int skip = (query.page - 1) * query.size;

            var result = (from log in _context.SrmDownloadLogs
                          join h in _context.SrmPoHs on log.PoId equals h.PoId
                          join l in _context.SrmPoLs on new { PoId = log.PoId, PoLId = log.PoLId } equals new { PoId = l.PoId, PoLId = l.PoLId }
                          join user in _context.AspNetUsers on log.CreateBy equals user.UserName
                          join matnr in _context.SrmMatnrs on l.MatnrId equals matnr.MatnrId into matnrInfo
                          from matnr in matnrInfo.DefaultIfEmpty()
                          select new ViewSrmDownloadLog
                          {
                              DId = log.DId,
                              PoNum = h.PoNum,
                              PoLId = log.PoLId,
                              PoId = log.PoId,
                              FileName = log.FileName,
                              Ip = log.Ip,
                              CreateBy = log.CreateBy,
                              CreateDate = log.CreateDate,
                              SapMatnr = matnr.SapMatnr,
                              Description = l.Description,
                              Name = user.Name,
                          })
                          .AndIfCondition(!string.IsNullOrWhiteSpace(query.sapMatnr), p => p.SapMatnr.IndexOf(query.sapMatnr) > -1)
                          .AndIfCondition(!string.IsNullOrWhiteSpace(query.description), p => p.Description.IndexOf(query.description) > -1)
                          .AndIfCondition(!string.IsNullOrWhiteSpace(query.username), p => p.Name.IndexOf(query.username) > -1)
                          .AndIfHaveValue(query.Date_s, p => p.CreateDate >= query.Date_s.Value.Date)
                          .AndIfHaveValue(query.Date_e, p => p.CreateDate <= query.Date_e.Value.AddDays(1).Date)
                          .OrderByDescending(p => p.CreateDate)
                          .ToList();

            var r = result.AsQueryable().Skip(skip).Take(query.size).ToArray();//result.Skip(skip).Take(size);
            return new PagingResultModel<ViewSrmDownloadLog>
            {
                Data = r,
                Count = result.Count()
            };
        }
    }
}