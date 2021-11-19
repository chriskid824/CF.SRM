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
using Convience.Model.Models.SRM;
using Convience.Model.Models.SystemManage;
using Convience.Util.Extension;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmDisscussionService
    {
        public bool AddTitle(ViewSrmDisscussionH data);
        public bool AddContent(SrmDisscussionC data);
        public bool UpdateContent(SrmDisscussionC data);
        public bool DeleteContent(SrmDisscussionC data);
        public PagingResultModel<ViewSrmDisscussionH> GetDissList(QueryFile query, int page, int size);
        public PageResultModel<ViewSrmDisscussionH> GetDisscussion(QueryFile query, int page, int size);
    }

    public class SrmDisscussionService : ISrmDisscussionService
    {
        private readonly SRMContext _context;

        //private readonly IMapper _mapper;
        private readonly IRepository<SrmPoH> _srmPohRepository;

        private readonly IRepository<SrmPoL> _srmPolRepository;
        private IMapper _mapper;
        private readonly IFileStore _fileStore;
        private readonly appSettings _appSettingsService;
        private readonly ISrmPoService _srmPoService;

        public SrmDisscussionService(
            //IMapper mapper,
            IRepository<SrmPoH> srmPohRepository, IRepository<SrmPoL> srmPolRepository, SRMContext context, IMapper mapper, IFileStore fileStore
            , IOptions<appSettings> appSettingsOption, ISrmPoService srmPoService)
        {
            _mapper = mapper;
            _srmPohRepository = srmPohRepository;
            _srmPolRepository = srmPolRepository;
            _context = context;
            _fileStore = fileStore;
            _appSettingsService = appSettingsOption.Value;
            _srmPoService = srmPoService;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }

        public bool AddTitle(ViewSrmDisscussionH data)
        {
            _context.SrmDisscussionHs.Add(data);
            _context.SaveChanges();
            //if (!string.IsNullOrEmpty(data.content))
            //{
            SrmDisscussionC content = new SrmDisscussionC();
            content.DisscussionId = data.DisscussionId;
                content.DisscussionIdC = 1;
                content.CreateBy = data.CreateBy;
                content.CreateDate = data.CreateDate;
            content.DisscustionContent = data.content;
                _context.SrmDisscussionCs.Add(content);
                _context.SaveChanges();
            //}
            return true;
        }
        public bool AddContent(SrmDisscussionC data)
        {
            data.DisscussionIdC = _context.SrmDisscussionCs.Max(p => p.DisscussionIdC) + 1;
            _context.SrmDisscussionCs.Add(data);
            _context.SaveChanges();
            return true;
        }
        public bool UpdateContent(SrmDisscussionC data)
        {
            SrmDisscussionC content = _context.SrmDisscussionCs.Where(p => p.DisscussionId == data.DisscussionId && p.DisscussionIdC == data.DisscussionIdC).FirstOrDefault();
            if (content == null) return false;
            content.DisscustionContent = data.DisscustionContent;
            content.LastUpdateBy = data.LastUpdateBy;
            content.LastUpdateDate = data.LastUpdateDate;
            _context.SrmDisscussionCs.Update(content);
            _context.SaveChanges();
            return true;
        }
        public bool DeleteContent(SrmDisscussionC data)
        {
            SrmDisscussionC content = _context.SrmDisscussionCs.Where(p => p.DisscussionId == data.DisscussionId && p.DisscussionIdC == data.DisscussionIdC).FirstOrDefault();
            if (content == null) return false;
            content.LastUpdateBy = data.LastUpdateBy;
            content.LastUpdateDate = data.LastUpdateDate;
            content.Active = false;
            _context.SrmDisscussionCs.Update(content);
            _context.SaveChanges();
            return true;
        }
        public PagingResultModel<ViewSrmDisscussionH> GetDissList(QueryFile query, int page, int size)
        {
            int skip = (page - 1) * size;
            List<string> roleids = query.user.GetUserRoleIds().Split(',').ToList();
            List<string> werksList = query.user.GetUserWerks().Split(',').ToList();
            List<ViewSrmDisscussionH> result = 
                (from dissh in _context.SrmDisscussionHs
                join func in _context.SrmFunctionLists on dissh.TemplateType equals func.FunctionId
                join user in _context.AspNetUsers on dissh.CreateBy equals user.UserName
                join vendor in _context.SrmVendors on user.UserName equals vendor.SapVendor 
                into groupjoin from vendor in groupjoin.DefaultIfEmpty()
                 select new ViewSrmDisscussionH
                {
                    DisscussionId = dissh.DisscussionId,
                    TemplateType = dissh.TemplateType,
                    Number = dissh.Number,
                    Title = dissh.Title,
                    CreateDate = dissh.CreateDate,
                    CreateBy = dissh.CreateBy,
                    LastUpdateDate = dissh.LastUpdateDate,
                    LastUpdateBy = dissh.LastUpdateBy,
                    FunctionName = func.FunctionName,
                    UserName = user.Name,
                    Ekorg=vendor.Ekorg
                })
                .AndIfCondition(!query.user.GetIsVendor(), p => werksList.Contains(p.Ekorg.ToString()) || p.CreateBy == query.user.GetUserName())
                .AndIfCondition(query.user.GetIsVendor(), p => p.CreateBy == query.user.GetUserName())
                //.AndIfCondition(!roleids.Contains("1"), p => p.CreateBy == query.user.GetUserName())
                .AndIfHaveValue(query.id, p => p.Number.Contains(query.id.ToString()))
                .ToList();
            var r = result.AsQueryable().Skip(skip).Take(size).ToArray();//result.Skip(skip).Take(size);
            foreach (var item in r)
            {
                item.LastCreateDateC = _context.SrmDisscussionCs.Where(p => p.DisscussionId == item.DisscussionId).Max(p => p.CreateDate);
                item.ContentCount = _context.SrmDisscussionCs.Where(p => p.DisscussionId == item.DisscussionId).Count();
            }
            return new PagingResultModel<ViewSrmDisscussionH>
            {
                Data = r,
                Count = result.Count()
            };
        }
        public PageResultModel<ViewSrmDisscussionH> GetDisscussion(QueryFile query, int page, int size)
        {
            int skip = (page - 1) * size;
            List<string> roleids = query.user.GetUserRoleIds().Split(',').ToList();

            ViewSrmDisscussionH result =
                (from dissh in _context.SrmDisscussionHs
                 join func in _context.SrmFunctionLists on dissh.TemplateType equals func.FunctionId
                 join user in _context.AspNetUsers on dissh.CreateBy equals user.UserName
                 select new ViewSrmDisscussionH
                 {
                     DisscussionId = dissh.DisscussionId,
                     TemplateType = dissh.TemplateType,
                     Number = dissh.Number,
                     Title = dissh.Title,
                     CreateDate = dissh.CreateDate,
                     CreateBy = dissh.CreateBy,
                     LastUpdateDate = dissh.LastUpdateDate,
                     LastUpdateBy = dissh.LastUpdateBy,
                     FunctionName = func.FunctionName,
                     UserName = user.Name,
                 })
                //.AndIfCondition(!roleids.Contains("1"), p => p.CreateBy == query.user.GetUserName())
                .AndIfHaveValue(query.id, p => p.DisscussionId == query.id)
                .FirstOrDefault();
            result.ViewSrmDisscussionCs =
                (from dissc in _context.SrmDisscussionCs
                 join user in _context.AspNetUsers on dissc.CreateBy equals user.UserName
                 select new ViewSrmDisscussionC
                 {
                     DisscussionId = dissc.DisscussionId,
                     DisscussionIdC = dissc.DisscussionIdC,
                     DisscustionContent = dissc.DisscustionContent,
                     CreateDate = dissc.CreateDate,
                     CreateBy = dissc.CreateBy,
                     LastUpdateDate = dissc.LastUpdateDate,
                     LastUpdateBy = dissc.LastUpdateBy,
                     UserName = user.Name,
                     isEdit = false,
                     Active=dissc.Active
                 }).Where(p => p.DisscussionId == result.DisscussionId).ToList();
            //_context.SrmDisscussionCs.Where(p => p.DisscussionId == result.DisscussionId).ToList();
            int Count = result.ViewSrmDisscussionCs.Count();
            result.ViewSrmDisscussionCs = result.ViewSrmDisscussionCs.AsQueryable().Skip(skip).Take(size).ToArray();//result.Skip(skip).Take(size);
            return new PageResultModel<ViewSrmDisscussionH>
            {
                Data = result,
                Count = Count
            };
        }
    }
}