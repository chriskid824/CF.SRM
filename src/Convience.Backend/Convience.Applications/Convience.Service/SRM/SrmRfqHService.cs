using AutoMapper;

using Convience.Entity.Data;
using Convience.Entity.Entity;
using Convience.Entity.Entity.Identity;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.JwtAuthentication;
using Convience.Model.Constants.SystemManage;
using Convience.Model.Models;
using Convience.Model.Models.SystemManage;
using Convience.Util.Extension;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Convience.Model.Models.SRM;
using Newtonsoft.Json;
using Convience.Service.SystemManage;

namespace Convience.Service.SRM
{
    public interface ISrmRfqHService
    {
        public void Save(SrmRfqH rfqH);
        public void Save(SrmRfqH rfqH, SrmRfqM[] rfqMs, SrmRfqV[] rfqVs);
        public ViewSrmRfqH GetDataByRfqId(int RfqId);
        public PagingResultModel<ViewSrmRfqH> GetRfqList(QueryRfqList q, int page, int size);
        public SrmRfqH UpdateStatus(int status, SrmRfqH rfqH);
        public PagingResultModel<AspNetUser> GetSourcer(string name, int[] werks, int size, int page);
        public SrmRfqH GetRfq(QueryRfq query);
        public void AsyncSourcer();
    }
    public class SrmRfqHService : ISrmRfqHService
    {
        private readonly IRepository<SrmRfqH> _srmRfqHRepository;
        private readonly SRMContext _context;
        private readonly IRepository<SystemUser> _userRepository;
        public SrmRfqHService(
            //IMapper mapper,
            IRepository<SrmRfqH> srmRfqHRepository, SRMContext dbContext, IRepository<SystemUser> userRepository)
        //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmRfqHRepository = srmRfqHRepository;
            _context = dbContext;
            _userRepository = userRepository;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }
        public void Save(SrmRfqH rfqH)
        {
            //using (var db = new SRMContext())
            //{
            if (rfqH.RfqId == 0)
            {
                _context.SrmRfqHs.Add(rfqH);
            }
            else
            {
                _context.SrmRfqHs.Update(rfqH);
            }
            _context.SaveChanges();
            //}
        }

        public void Save(SrmRfqH rfqH, SrmRfqM[] rfqMs, SrmRfqV[] rfqVs)
        {
            //using (var db = new SRMContext())
            //{
            //db.Database.BeginTransaction();
            //try
            //{
            DateTime now = DateTime.Now;
            rfqH.LastUpdateDate = now;
            if (rfqH.RfqId == 0)
            {
                rfqH.CreateDate = now;
                _context.SrmRfqHs.Add(rfqH);
                _context.SaveChanges();
                //rfqH.RfqNum = "V" + rfqH.RfqId.ToString().PadLeft(6,'0');
                foreach (var rfqM in rfqMs)
                {
                    rfqM.RfqId = rfqH.RfqId;
                }
                foreach (var rfqV in rfqVs)
                {
                    rfqV.RfqId = rfqH.RfqId;
                }
                _context.SrmRfqHs.Update(rfqH);
                _context.SrmRfqMs.AddRange(rfqMs);
                _context.SrmRfqVs.AddRange(rfqVs);
            }
            else
            {
                //db.SrmRfqHs.Any(r => r.RfqId == rfqH.RfqId && r.Status == (int)Status.初始)
                if (!_context.SrmRfqHs.Any(r => r.RfqId == rfqH.RfqId && r.Status == (int)Status.初始))
                {
                    throw new Exception("非初始無法修改");
                }
                _context.SrmRfqHs.Update(rfqH);
                foreach (var rfqM in rfqMs)
                {
                    rfqM.RfqId = rfqH.RfqId;
                    if (rfqM.RfqMId == 0)
                    {
                        _context.SrmRfqMs.Add(rfqM);
                    }
                    else
                    {
                        _context.SrmRfqMs.Update(rfqM);
                    }
                }
                var oldRfqMs = _context.SrmRfqMs.Where(r => r.RfqId == rfqH.RfqId);
                foreach (var oldrfqM in oldRfqMs)
                {
                    if (rfqMs.AsEnumerable().Where(item => item.RfqMId == oldrfqM.RfqMId).Count() == 0)
                    {
                        _context.SrmRfqMs.Remove(oldrfqM);
                    }
                }
                foreach (var rfqV in rfqVs)
                {
                    rfqV.RfqId = rfqH.RfqId;
                    if (rfqV.RfqVId == 0)
                    {
                        _context.SrmRfqVs.Add(rfqV);
                    }
                    else
                    {
                        _context.SrmRfqVs.Update(rfqV);
                    }
                }
                var oldRfqVs = _context.SrmRfqVs.Where(r => r.RfqId == rfqH.RfqId);
                foreach (var oldrfqV in oldRfqVs)
                {
                    if (rfqVs.AsEnumerable().Where(item => item.RfqVId == oldrfqV.RfqVId).Count() == 0)
                    {
                        _context.SrmRfqVs.Remove(oldrfqV);
                    }
                }
            }
            _context.SaveChanges();
            //    db.Database.CommitTransaction();
            //}
            //catch (Exception ex)
            //{
            //    db.Database.RollbackTransaction();
            //    throw;
            //}
            //}
        }


        public ViewSrmRfqH[] GetRfqList(QueryRfqList q)
        {
            var rfqQuery = _srmRfqHRepository.Get().AndIfHaveValue(q.rfqNum, r => r.RfqNum.Contains(q.rfqNum))
        .AndIfHaveValue(q.status, r => r.Status.Equals(q.status));

            rfqQuery = rfqQuery.Where(r => r.Status != (int)Status.刪除);

            var rfqs = from rfq in rfqQuery
                       join e in _context.SrmEkgries on rfq.CreateBy equals e.Empid
                       join u in _context.AspNetUsers on rfq.CreateBy equals u.UserName
                       join s in _context.AspNetUsers on rfq.Sourcer equals s.UserName
                       into gj
                       from x in gj.DefaultIfEmpty()
                       where q.werks.Contains(e.Werks)
                       select new ViewSrmRfqH
                       {
                           RfqId = rfq.RfqId,
                           Status = rfq.Status,
                           RfqNum = rfq.RfqNum,
                           Sourcer = rfq.Sourcer,
                           sourcerName = x.Name,
                           CreateBy = rfq.CreateBy,
                           CreateDate = rfq.CreateDate,
                           C_by = u.Name,
                       };
            return rfqs.AndIfHaveValue(q.name, r => r.C_by.Contains(q.name))
                .AndIfCondition(q.end,r=>r.Status==7 && r.Deadline.Value.AddDays(1)<=DateTime.Now.Date)
                .Distinct().ToArray();
        }

        public PagingResultModel<ViewSrmRfqH> GetRfqList(QueryRfqList q, int page, int size)
        {
            int skip = (page - 1) * size;

            //var rfqQuery = _srmRfqHRepository.Get().AndIfHaveValue(q.rfqNum, r => r.RfqNum.Contains(q.rfqNum))
            //    .AndIfHaveValue(q.status, r => r.Status.Equals(q.status));

            //rfqQuery = rfqQuery.Where(r => r.Status != (int)Status.刪除);

            //var rfqs = from rfq in rfqQuery
            //           join e in _context.SrmEkgries on rfq.CreateBy equals e.Empid
            //           join u in _context.AspNetUsers on rfq.CreateBy equals u.UserName
            //           join s in _context.AspNetUsers on rfq.Sourcer equals s.UserName
            //           into gj
            //           from x in gj.DefaultIfEmpty()
            //           where q.werks.Contains(e.Werks)
            //           select new
            //           {
            //               id = rfq.RfqId,
            //               status = rfq.Status,
            //               rfqNum = rfq.RfqNum,
            //               sourcer = rfq.Sourcer,
            //               sourcerName = x.Name,
            //               createBy = rfq.CreateBy,
            //               createDate = rfq.CreateDate,
            //               c_by = u.Name,
            //               c_Date = rfq.CreateDate.Value.ToString("yyyy-MM-dd"),
            //               viewstatus = ((Status)rfq.Status).ToString(),
            //           };
            //var result = rfqs.AndIfHaveValue(q.name, r => r.c_by.Contains(q.name)).Distinct().ToArray();

            var result = GetRfqList(q);
            var r = result.AsQueryable().Skip(skip).Take(size).ToArray();//result.Skip(skip).Take(size);
            return new PagingResultModel<ViewSrmRfqH>
            {
                Data = r,
                Count = result.Count()
            };
            //JObject obj = new JObject() {
            //    { "data",JArray.FromObject(r)},
            //    { "total",result.Count()}
            //};
            //return obj;
        }

        public ViewSrmRfqH GetDataByRfqId(int RfqId)
        {
            var rfq = (from rfqH in _context.SrmRfqHs
                       join sourcer in _context.AspNetUsers on rfqH.Sourcer equals sourcer.UserName
                       join create in _context.AspNetUsers on rfqH.CreateBy equals create.UserName
                       join ekgry in _context.SrmEkgries on rfqH.Sourcer equals ekgry.Empid into egrouping
                       from ekgry in egrouping.DefaultIfEmpty()
                       where rfqH.RfqId.Equals(RfqId)
                       select new ViewSrmRfqH
                       {
                           CreateBy = rfqH.CreateBy,
                           CreateDate = rfqH.CreateDate,
                           C_by = create.Name,
                           Deadline = rfqH.Deadline,
                           EndDate = rfqH.EndDate,
                           EndBy = rfqH.EndBy,
                           LastUpdateBy = rfqH.LastUpdateBy,
                           LastUpdateDate = rfqH.LastUpdateDate,
                           RfqId = rfqH.RfqId,
                           RfqNum = rfqH.RfqNum,
                           Sourcer = rfqH.Sourcer,
                           sourcerName = sourcer.Name,
                           Status = rfqH.Status,
                           ekgry = ekgry.Ekgry
                       }).First();


            //var rfq = _srmRfqHRepository.Get(r => r.RfqId == RfqId).DefaultIfEmpty().Join(_context.AspNetUsers,a=>a.Sourcer,b=>b.UserName,(a,b)=>new ViewSrmRfqH { 
            // CreateBy = a.CreateBy,
            // CreateDate = a.CreateDate,
            // C_by =     
            //    ,b.Name
            //}).First();

            //string temp = JsonConvert.SerializeObject(rfq);
            //ViewSrmRfqH result = JsonConvert.DeserializeObject<ViewSrmRfqH>(temp);
            return rfq;
        }

        public SrmRfqH UpdateStatus(int status, SrmRfqH rfqH)
        {
            var rfq = _srmRfqHRepository.Get(r => r.RfqId == rfqH.RfqId).First();
            switch ((Status)status)
            {
                case Status.啟動:
                    if ((Status)rfq.Status != Status.初始)
                    {
                        throw new Exception($"非初始狀態無法{((Status)status).ToString()}");
                    }
                    break;
                case Status.作廢:
                case Status.刪除:
                    if ((Status)rfq.Status != Status.初始 && (Status)rfq.Status != Status.啟動)
                    {
                        throw new Exception($"非初始或啟動狀態無法{((Status)status).ToString()}");
                    }
                    rfq.EndDate = DateTime.Now;
                    rfq.EndBy = rfqH.EndBy;
                    break;
                case Status.簽核中:
                    if ((Status)rfq.Status != Status.確認 && (Status)rfq.Status != Status.簽核中 && (Status)rfq.Status != Status.已核發)
                    {
                        throw new Exception($"狀態異常無法{((Status)status).ToString()}");
                    }
                    if ((Status)rfq.Status == Status.已核發)
                    {
                        return rfq;
                    }
                    break;
                default:
                    throw new Exception($"未定義{((Status)status).ToString()}");
                    break;
            }
            rfq.Status = status;
            rfq.LastUpdateDate = rfqH.LastUpdateDate;
            rfq.LastUpdateBy = rfqH.LastUpdateBy;
            using (SRMContext db = new SRMContext())
            {
                db.Update(rfq);
                db.SaveChanges();
                return rfq;
            }
        }
        public PagingResultModel<AspNetUser> GetSourcer(string name, int[] werks, int size, int page)
        {
            //int[] werks = _dbContext.SrmEkgries.Where(r => r.Empid == logonid).Select(r => r.Werks).ToArray();

            //using (SRMContext db = new SRMContext())
            //{
            var resultQuery = (from s in _context.AspNetUsers
                               join sa in _context.SrmEkgries on s.UserName equals sa.Empid
                               where werks.Contains(sa.Werks)
                               //where s.UserName.Contains(name) && s.Name.Contains(name) && s.CostNo.StartsWith(werks)
                               //where s.UserName.Contains(name) && s.Name.Contains(name) && sa.Werks.StartsWith(werks)
                               select s).Distinct();
            var skip = size * (page - 1);
            var users = size == 0 ? resultQuery.ToArray() : resultQuery.Skip(skip).Take(size).ToArray();
            return new PagingResultModel<AspNetUser>
            {
                Data = users,
                Count = resultQuery.Count()
            };
            //}
        }
        public SrmRfqH GetRfq(QueryRfq query)
        {
            //using (SRMContext db = new SRMContext())
            //{
            return _context.SrmRfqHs.AsQueryable().AndIfHaveValue(query.rfqNum, r => r.RfqNum == query.rfqNum)
                 .AndIfHaveValue(query.status, r => r.Status == query.status)
                 .AndIfHaveValue(query.statuses, r => query.statuses.Contains(r.Status.Value))
                 .AndIfHaveValue(query.werks, r => _context.SrmEkgries.Where(e => query.werks.Contains(e.Werks)).Select(e => e.Empid).Contains(r.CreateBy))
                 .FirstOrDefault();
            //}
        }
        public void AsyncSourcer()
        {
            int roleid = _context.AspNetRoles.Where(r => r.Name == "詢價人員").Select(r => r.Id).First();
            string[] ekgries = _context.SrmEkgries.Select(r => r.Empid).ToArray();
            int[] userIds = (from e in ekgries
                             join u in _context.AspNetUsers on e equals u.UserName
                             select u.Id).ToArray();
            foreach (int userid in userIds)
            {
                if (!_context.AspNetUserRoles.Any(r => r.UserId == userid && r.RoleId == roleid))
                {
                    _context.AspNetUserRoles.Add(new AspNetUserRole() { UserId = userid, RoleId = roleid });
                }
            }
        }
    }
}
