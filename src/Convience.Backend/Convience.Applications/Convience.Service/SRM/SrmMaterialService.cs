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
    public interface ISrmMaterialService
    {
        /// <summary>
        /// 取得全部角色
        public PagingResultModel<ViewSrmMaterial> GetMaterialList(QueryMaterial query);
        public ViewSrmMaterial GetMaterialDetail(QueryMaterial query);
        public bool UpdateMaterial(ViewSrmMaterial data);
        public bool CheckMatnr(ViewSrmMaterial data);
    }
    public class SrmMaterialService: ISrmMaterialService
    {
        private readonly SRMContext _context;

        private readonly IRepository<SrmMatnr> _srmMaterialRepository;

        public SrmMaterialService(IRepository<SrmMatnr> srmMaterialRepository, SRMContext context)
        {
            //_mapper = mapper;
            _srmMaterialRepository = srmMaterialRepository;
            _context = context;
        }

        public PagingResultModel<ViewSrmMaterial> GetMaterialList(QueryMaterial query)
        {
            int skip = (query.Page-1) * query.Size;

            var resultQuery = (from matnr in _context.SrmMatnrs
                          join status in _context.SrmStatuses on matnr.Status equals status.Status
                          select new ViewSrmMaterial
                          {
                              SrmMatnr1 = matnr.SrmMatnr1,
                              MatnrGroup = matnr.MatnrGroup,
                              Description = matnr.Description,
                              Version = matnr.Version,
                              Material = matnr.Material,
                              Length = matnr.Length,
                              Width = matnr.Width,
                              Height = matnr.Height,
                              Density = matnr.Density,
                              Weight = matnr.Weight,
                              Note = matnr.Note,
                              StatusDesc = status.StatusDesc, 
                          })
                          .AndIfHaveValue(query.material, r => r.SrmMatnr1.Contains(query.material))
                          .AndIfHaveValue(query.name, r => r.SrmMatnr1.Contains(query.name));
            
            var materials = resultQuery.Skip(skip).Take(query.Size).ToArray();

            return new PagingResultModel<ViewSrmMaterial>
            {
                Data = JsonConvert.DeserializeObject<ViewSrmMaterial[]>(JsonConvert.SerializeObject(materials)),
                Count = resultQuery.Count()
            };
        }
        public ViewSrmMaterial GetMaterialDetail(QueryMaterial query)
        {
            var material = (from matnr in _context.SrmMatnrs
                            join status in _context.SrmStatuses on matnr.Status equals status.Status
                            select new ViewSrmMaterial
                            {
                                SrmMatnr1 = matnr.SrmMatnr1,
                                MatnrGroup = matnr.MatnrGroup,
                                Description = matnr.Description,
                                Version = matnr.Version,
                                Material = matnr.Material,
                                Length = matnr.Length,
                                Width = matnr.Width,
                                Height = matnr.Height,
                                Density = matnr.Density,
                                Weight = matnr.Weight,
                                Note = matnr.Note,
                                StatusDesc = status.StatusDesc,
                            })
                          .Where(r => r.SrmMatnr1 == query.material).FirstOrDefault()
                          //.Where(r => r.Org == query.Org)
                          //.Where(r => r.Ekorg == query.Ekorg)
                          ;

            //var suppliers = supplier.ToArray();
            //
            return new ViewSrmMaterial
            {
                SrmMatnr1 = material.SrmMatnr1,
                MatnrGroup = material.MatnrGroup,
                Description = material.Description,
                Version = material.Version,
                Material = material.Material,
                Length = material.Length,
                Width = material.Width,
                Height = material.Height,
                Density = material.Density,
                Weight = material.Weight,
                Note = material.Note,
                StatusDesc = material.StatusDesc
            };
        }
        public bool UpdateMaterial(ViewSrmMaterial data)
        {
            SrmMatnr material = _context.SrmMatnrs.Where(p => p.SrmMatnr1 == data.SrmMatnr1).FirstOrDefault();
            SrmStatus status = _context.SrmStatuses.Where(p => p.StatusDesc == data.StatusDesc).FirstOrDefault();

            if (status == null || material == null)
            {
                return false;
            }


            material.SrmMatnr1 = data.SrmMatnr1;
            material.MatnrGroup = data.MatnrGroup;
            material.Description = data.Description;
            material.Version = data.Version;
            material.Material = data.Material;
            material.Length = data.Length;
            material.Width = data.Width;
            material.Height = data.Height;
            material.Density = data.Density;
            material.Weight = data.Weight;
            material.Note = data.Note;
            material.Status = status.Status;
            material.LastUpdateDate = DateTime.Now;
            material.LastUpdateBy = data.User;


            _context.SrmMatnrs.Update(material);
            _context.SaveChanges();

            return true;
        }
        public bool CheckMatnr(ViewSrmMaterial data)
        {
            SrmMatnr matnr = _context.SrmMatnrs.Where(p => p.SrmMatnr1 == data.Material).FirstOrDefault();
            if (matnr == null)
            {
                return true;
            }

            return false;
        }
    }
}
