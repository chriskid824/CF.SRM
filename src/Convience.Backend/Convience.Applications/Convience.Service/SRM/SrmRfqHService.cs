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
        public JObject GetRfqList(QueryRfqList q, int page, int size);
        public SrmRfqH UpdateStatus(int status, SrmRfqH rfqH);
        public PagingResultModel<AspNetUser> GetSourcer(string name,int[] werks, int size, int page);
        public SrmRfqH GetRfq(QueryRfq query);
        public void AsyncSourcer();
    }
    public class SrmRfqHService : ISrmRfqHService
    {
        private readonly IRepository<SrmRfqH> _srmRfqHRepository;
        private readonly SRMContext _dbContext;
        private readonly IRepository<SystemUser> _userRepository;
        public SrmRfqHService(
            //IMapper mapper,
            IRepository<SrmRfqH> srmRfqHRepository, SRMContext dbContext, IRepository<SystemUser> userRepository)
        //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmRfqHRepository = srmRfqHRepository;
            _dbContext = dbContext;
            _userRepository = userRepository;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }
        public void Save(SrmRfqH rfqH)
        {
            using (var db = new SRMContext())
            {
                if (rfqH.RfqId == 0)
                {
                    db.SrmRfqHs.Add(rfqH);
                }
                else
                {
                    db.SrmRfqHs.Update(rfqH);
                }
                db.SaveChanges();
            }
        }

        public void Save(SrmRfqH rfqH, SrmRfqM[] rfqMs, SrmRfqV[] rfqVs)
        {
            using (var db = new SRMContext())
            {
                //db.Database.BeginTransaction();
                //try
                //{
                DateTime now = DateTime.Now;
                rfqH.LastUpdateDate = now;
                if (rfqH.RfqId == 0)
                {
                    rfqH.CreateDate = now;
                    db.SrmRfqHs.Add(rfqH);
                    db.SaveChanges();
                    //rfqH.RfqNum = "V" + rfqH.RfqId.ToString().PadLeft(6,'0');
                    foreach (var rfqM in rfqMs)
                    {
                        rfqM.RfqId = rfqH.RfqId;
                    }
                    foreach (var rfqV in rfqVs)
                    {
                        rfqV.RfqId = rfqH.RfqId;
                    }
                    db.SrmRfqHs.Update(rfqH);
                    db.SrmRfqMs.AddRange(rfqMs);
                    db.SrmRfqVs.AddRange(rfqVs);
                }
                else
                {
                    //db.SrmRfqHs.Any(r => r.RfqId == rfqH.RfqId && r.Status == (int)Status.初始)
                    if (!db.SrmRfqHs.Any(r => r.RfqId == rfqH.RfqId && r.Status == (int)Status.初始))
                    {
                        throw new Exception("非初始無法修改");
                    }
                    db.SrmRfqHs.Update(rfqH);
                    foreach (var rfqM in rfqMs)
                    {
                        rfqM.RfqId = rfqH.RfqId;
                        if (rfqM.RfqMId == 0)
                        {
                            db.SrmRfqMs.Add(rfqM);
                        }
                        else
                        {
                            db.SrmRfqMs.Update(rfqM);
                        }
                    }
                    var oldRfqMs = db.SrmRfqMs.Where(r => r.RfqId == rfqH.RfqId);
                    foreach (var oldrfqM in oldRfqMs)
                    {
                        if (rfqMs.AsEnumerable().Where(item => item.RfqMId == oldrfqM.RfqMId).Count() == 0)
                        {
                            db.SrmRfqMs.Remove(oldrfqM);
                        }
                    }
                    foreach (var rfqV in rfqVs)
                    {
                        rfqV.RfqId = rfqH.RfqId;
                        if (rfqV.RfqVId == 0)
                        {
                            db.SrmRfqVs.Add(rfqV);
                        }
                        else
                        {
                            db.SrmRfqVs.Update(rfqV);
                        }
                    }
                    var oldRfqVs = db.SrmRfqVs.Where(r => r.RfqId == rfqH.RfqId);
                    foreach (var oldrfqV in oldRfqVs)
                    {
                        if (rfqVs.AsEnumerable().Where(item => item.RfqVId == oldrfqV.RfqVId).Count() == 0)
                        {
                            db.SrmRfqVs.Remove(oldrfqV);
                        }
                    }
                }
                db.SaveChanges();
                //    db.Database.CommitTransaction();
                //}
                //catch (Exception ex)
                //{
                //    db.Database.RollbackTransaction();
                //    throw;
                //}
            }
        }
        public JObject GetRfqList(QueryRfqList q, int page, int size)
        {
            int skip = (page - 1) * size;

            //int[] werks = _dbContext.SrmEkgries.Where(r => r.Empid == "").Select(r=>r.Werks).ToArray();

            var rfqQuery = _srmRfqHRepository.Get().AndIfHaveValue(q.rfqNum, r => r.RfqNum.Contains(q.rfqNum))
                .AndIfHaveValue(q.status, r => r.Status.Equals(q.status));

            rfqQuery = rfqQuery.Where(r => r.Status != (int)Status.刪除);

            var rfqs = from rfq in rfqQuery
                              join e in _dbContext.SrmEkgries on rfq.CreateBy equals e.Empid
                              join u in _dbContext.AspNetUsers on rfq.CreateBy equals u.UserName
                              join s in _dbContext.AspNetUsers on rfq.Sourcer equals s.UserName
                              into gj
                              from x in gj.DefaultIfEmpty()
                       where q.werks.Contains(e.Werks)
                       select new
                              {
                                  id = rfq.RfqId,
                                  status = rfq.Status,
                                  rfqNum = rfq.RfqNum,
                                  sourcer = rfq.Sourcer,
                                  sourcerName = x.Name,
                                  createBy = rfq.CreateBy,
                                  createDate = rfq.CreateDate,
                                  c_by = u.Name,
                                  c_Date = rfq.CreateDate.Value.ToString("yyyy-MM-dd"),
                                  viewstatus = ((Status)rfq.Status).ToString(),
                              };
            var result = rfqs.AndIfHaveValue(q.name, r => r.c_by.Contains(q.name)).Distinct().ToArray();



            //var rfqs = rfqQuery.ToList().Join(_userRepository.Get().ToList(), a => a.CreateBy, b => b.UserName, (_srmRfqHRepository, _userRepository) => new
            //{
            //    id = _srmRfqHRepository.RfqId,
            //    status = _srmRfqHRepository.Status,
            //    rfqNum = _srmRfqHRepository.RfqNum,
            //    sourcer = _srmRfqHRepository.Sourcer,
            //    createBy = _srmRfqHRepository.CreateBy,
            //    createDate = _srmRfqHRepository.CreateDate,
            //    c_by = _userRepository.Name,
            //    c_Date = _srmRfqHRepository.CreateDate.Value.ToString("yyyy-MM-dd"),
            //    viewstatus = ((Status)_srmRfqHRepository.Status).ToString(),
            //    costNo = _userRepository.CostNo
            //}).Where(r => werks.Contains()).DefaultIfEmpty().Join(_userRepository.Get().ToList(), a => a?.sourcer ?? "", b => b.UserName, (a, b) => new
            //{
            //    id = a.id,
            //    status = a.status,
            //    rfqNum = a.rfqNum,
            //    sourcer = a.sourcer,
            //    sourcerName = b.Name,
            //    createBy = a.createBy,
            //    createDate = a.createDate,
            //    c_by = a.c_by,
            //    c_Date = a.c_Date,
            //    viewstatus = a.viewstatus,
            //})
            //.AndIfHaveValue(q.name, r => r.c_by.Contains(q.name));
            var r = result.Skip(skip).Take(size);
            JObject obj = new JObject() {
                { "data",JArray.FromObject(r)},
                { "total",result.Count()}
            };
            return obj;
        }

        public ViewSrmRfqH GetDataByRfqId(int RfqId)
        {
            var rfq = _srmRfqHRepository.Get(r => r.RfqId == RfqId).First();
            string temp = JsonConvert.SerializeObject(rfq);
            ViewSrmRfqH result = JsonConvert.DeserializeObject<ViewSrmRfqH>(temp);
            return result;
        }

        public SrmRfqH UpdateStatus(int status, SrmRfqH rfqH)
        {
            var rfq = _srmRfqHRepository.Get(r => r.RfqId == rfqH.RfqId).First();
            switch ((Status)status)
            {
                case Status.啟動:
                    if ((Status)rfq.Status != Status.初始)
                    {
                        throw new Exception($"非底稿狀態無法{((Status)status).ToString()}");
                    }
                    break;
                case Status.作廢:
                case Status.刪除:
                    if ((Status)rfq.Status != Status.初始 && (Status)rfq.Status != Status.啟動)
                    {
                        throw new Exception($"非底稿狀態無法{((Status)status).ToString()}");
                    }
                    rfq.EndDate = DateTime.Now;
                    rfq.EndBy = rfqH.EndBy;
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
                var resultQuery = (from s in _dbContext.AspNetUsers
                                  join sa in _dbContext.SrmEkgries on s.UserName equals sa.Empid
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
            using (SRMContext db = new SRMContext())
            {
                return db.SrmRfqHs.AsQueryable().AndIfHaveValue(query.rfqNum, r => r.RfqNum == query.rfqNum)
                     .AndIfHaveValue(query.status, r => r.Status == query.status)
                     .AndIfHaveValue(query.statuses, r => query.statuses.Contains(r.Status.Value))
                     .FirstOrDefault();
            }
        }
        public void AsyncSourcer()
        {
            int roleid = _dbContext.AspNetRoles.Where(r => r.Name == "詢價人員").Select(r => r.Id).First();
            string[] ekgries = _dbContext.SrmEkgries.Select(r => r.Empid).ToArray();
            int[] userIds = (from e in ekgries
                             join u in _dbContext.AspNetUsers on e equals u.UserName
                             select u.Id).ToArray();
            foreach (int userid in userIds)
            {
                if (!_dbContext.AspNetUserRoles.Any(r => r.UserId == userid && r.RoleId == roleid))
                {
                    _dbContext.AspNetUserRoles.Add(new AspNetUserRole() { UserId = userid, RoleId = roleid });
                }
            }
        }
    }
}
