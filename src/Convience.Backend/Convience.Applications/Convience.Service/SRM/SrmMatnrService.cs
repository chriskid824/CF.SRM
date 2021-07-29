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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmMatnrService
    {
        /// <summary>
        /// 取得全部角色
        /// </summary>
        public PagingResultModel<SrmMatnr> GetMatnr(QueryMatnrModel matnrQuery);
    }
    public class SrmMatnrService: ISrmMatnrService
    {
        //private readonly IMapper _mapper;
        private readonly IRepository<SrmMatnr> _srmMatnrRepository;
        //private readonly SystemIdentityDbUnitOfWork _systemIdentityDbUnitOfWork;

        public SrmMatnrService(
            //IMapper mapper,
            IRepository<SrmMatnr> srmMatnrRepository)
            //SystemIdentityDbUnitOfWork systemIdentityDbUnitOfWork)
        {
            //_mapper = mapper;
            _srmMatnrRepository = srmMatnrRepository;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }

        public PagingResultModel<SrmMatnr> GetMatnr(QueryMatnrModel matnrQuery)
        {
            int skip = (matnrQuery.Page - 1) * matnrQuery.Size;
            var resultQuery = _srmMatnrRepository.Get().AndIfHaveValue(matnrQuery.Matnr, r => r.Matnr.Contains(matnrQuery.Matnr));
            var matnrs = resultQuery.Skip(skip).Take(matnrQuery.Size).ToArray();
            return new PagingResultModel<SrmMatnr>
            {
                Data = matnrs,
                Count = resultQuery.Count()
            };
            // return _srmMatnrRepository.Get(r=>string.IsNullOrWhiteSpace(matnrQuery.MATNR) ? true:r.Matnr.IndexOf(matnr)>=0).ToList().Skip(skip).Take(size).AsQueryable(); ;
        }
        public IEnumerable<SrmMatnr> GetMatnrById(int id)
        {
            //using (var transaction = new TransactionScope())
                return _srmMatnrRepository.Get(r => r.MatnrId == id);
        }
    }
}
