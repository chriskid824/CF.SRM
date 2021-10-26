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
        public PagingResultModel<ViewSrmMatnr1> GetMaterialList(QueryMaterial query);
        public ViewSrmMatnr1 GetMaterialDetail(QueryMaterial query);
        public bool UpdateMaterial(ViewSrmMatnr1 data);
        public ViewSrmMatnr1 AddMatnr(ViewSrmMatnr1 data);
        public string Checkdata(ViewSrmMatnr1 data);
        public SrmEkgry GetEkgrp(SrmEkgry data);
        public PagingResultModel<SrmMaterialGroup> GetGroupList(QueryMaterial query);
        public PagingResultModel<SrmWeightUnit> GetUnitList(QueryMaterial query);
    }
    public class SrmMaterialService : ISrmMaterialService
    {
        private readonly SRMContext _context;

        private readonly IRepository<SrmMatnr> _srmMaterialRepository;

        public SrmMaterialService(IRepository<SrmMatnr> srmMaterialRepository, SRMContext context)
        {
            //_mapper = mapper;
            _srmMaterialRepository = srmMaterialRepository;
            _context = context;
        }

        public PagingResultModel<ViewSrmMatnr1> GetMaterialList(QueryMaterial query)
        {
            int skip = (query.Page - 1) * query.Size;

            var resultQuery = (from matnr in _context.SrmMatnrs
                               join status in _context.SrmStatuses on matnr.Status equals status.Status
                               join ekgry in _context.SrmEkgries on matnr.Ekgrp equals ekgry.Ekgry
                               select new ViewSrmMatnr1
                               {
                                   SrmMatnr1 = matnr.SrmMatnr1,
                                   SapMatnr = matnr.SapMatnr,
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
                                   Gewei = matnr.Gewei,
                                   Ekgrp = ekgry.Ekgry+ekgry.EkgryDesc,
                                   Bn_num = matnr.Bn_num,
                                   Major_diameter = matnr.Major_diameter,
                                   Minor_diameter = matnr.Minor_diameter,
                               })
                          .AndIfHaveValue(query.material, r => r.SrmMatnr1.Contains(query.material))
                          .AndIfHaveValue(query.name, r => r.Description.Contains(query.name));

            var materials = resultQuery.Skip(skip).Take(query.Size).ToArray();

            return new PagingResultModel<ViewSrmMatnr1>
            {
                Data = JsonConvert.DeserializeObject<ViewSrmMatnr1[]>(JsonConvert.SerializeObject(materials)),
                Count = resultQuery.Count()
            };
        }
        public ViewSrmMatnr1 GetMaterialDetail(QueryMaterial query)
        {
            var material = (from matnr in _context.SrmMatnrs
                            join status in _context.SrmStatuses on matnr.Status equals status.Status
                            select new ViewSrmMatnr1
                            {
                                SrmMatnr1 = matnr.SrmMatnr1,
                                SapMatnr = matnr.SapMatnr,
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
                                Gewei = matnr.Gewei,
                                Ekgrp = matnr.Ekgrp,
                                Bn_num = matnr.Bn_num,
                                Major_diameter= matnr.Major_diameter,
                                Minor_diameter = matnr.Minor_diameter,
                            })
                          .Where(r => r.SrmMatnr1 == query.material).FirstOrDefault()
                          //.Where(r => r.Org == query.Org)
                          //.Where(r => r.Ekorg == query.Ekorg)
                          ;

            //var suppliers = supplier.ToArray();
            //
            return new ViewSrmMatnr1
            {
                SrmMatnr1 = material.SrmMatnr1,
                SapMatnr = material.SapMatnr,
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
                StatusDesc = material.StatusDesc,
                Gewei = material.Gewei,
                Ekgrp = material.Ekgrp,
                Bn_num = material.Bn_num,
                Major_diameter = material.Major_diameter,
                Minor_diameter = material.Minor_diameter,
            };
        }
        public bool UpdateMaterial(ViewSrmMatnr1 data)
        {
            SrmMatnr material = _context.SrmMatnrs.Where(p => p.SrmMatnr1 == data.SrmMatnr1).FirstOrDefault();
            //SrmStatus status = _context.SrmStatuses.Where(p => p.StatusDesc == data.StatusDesc).FirstOrDefault();
            SrmMatnr description = _context.SrmMatnrs.Where(p => p.Description == data.Description).FirstOrDefault();

            if (description!=null)
            {
                return false;
            }


            material.SrmMatnr1 = data.SrmMatnr1;
            material.SapMatnr = data.SapMatnr;
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
            //material.Status = status.Status;
            material.LastUpdateDate = DateTime.Now;
            material.LastUpdateBy = data.User;
            material.Gewei = data.Gewei;
            material.Ekgrp = data.Ekgrp;
            material.Bn_num = data.Bn_num;
            material.Major_diameter = data.Major_diameter;
            material.Minor_diameter = data.Minor_diameter;


            _context.SrmMatnrs.Update(material);
            _context.SaveChanges();

            return true;
        }

        public string Checkdata(ViewSrmMatnr1 data)
        {
            string msg = string.Empty;

            SrmMatnr srmmatnr = _context.SrmMatnrs.Where(p => p.SrmMatnr1 == data.SrmMatnr1).FirstOrDefault();
            SrmMatnr sapmatnr = _context.SrmMatnrs.Where(p => p.SapMatnr == data.SapMatnr).FirstOrDefault();
            SrmMatnr description = _context.SrmMatnrs.Where(p => p.Description == data.Description && p.Werks==data.Werks).FirstOrDefault();

            if (description != null)
            {
                msg = "物料內文 已重複使用";
            }
            if (!string.IsNullOrWhiteSpace(data.SapMatnr))
            {
                if (sapmatnr != null)
                {
                    msg = "SAP料號 已重複使用";
                }
            }
            if (srmmatnr != null)
            {
                msg = "SRM料號 已重複使用";
            }

            return msg;
        }

        public ViewSrmMatnr1 AddMatnr(ViewSrmMatnr1 data)
        {
            string no = string.Empty;
            string year = DateTime.Now.ToString("yy");

            var GetMatnr = _context.SrmMatnrs.Where(p => p.SrmMatnr1.StartsWith("BN" + year)).Max(p1 => p1.SrmMatnr1);
            SrmMatnr srmmatnr = _context.SrmMatnrs.Where(p => p.SrmMatnr1 == data.SrmMatnr1).FirstOrDefault();
            SrmMatnr description = _context.SrmMatnrs.Where(p => p.Description == data.Description && p.Werks == data.Werks).FirstOrDefault();


            if (srmmatnr != null)
            {
                throw new Exception("編號已重複使用");
            }
            if (description != null)
            {
                throw new Exception("內文已重複使用");
            }


            if (string.IsNullOrWhiteSpace(data.MatnrGroup))
            {
                throw new Exception("物料群組，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Description))
            {
                throw new Exception("物料內文，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Werks.ToString()))
            {
                throw new Exception("工廠，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Ekgrp))
            {
                throw new Exception("採購群組代碼，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Length))
            {
                throw new Exception("長，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Width))
            {
                throw new Exception("寬，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Height))
            {
                throw new Exception("高(厚)，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Density))
            {
                throw new Exception("密度，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Weight.ToString()))
            {
                throw new Exception("重量，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Gewei))
            {
                throw new Exception("重量單位，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Material))
            {
                throw new Exception("材質規格，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Version))
            {
                throw new Exception("版次，必填");
            }


            if (string.IsNullOrWhiteSpace(data.SrmMatnr1))
            {
                if (GetMatnr == null)
                {
                    no = "BN" + year + "000001";
                }
                else
                {
                    no = "BN" + year + (int.Parse(GetMatnr.Substring(4, 6)) + 1).ToString().PadLeft(6, '0');
                }
            }
            else
            {
                no = data.SrmMatnr1;
            }


            SrmMatnr material = new SrmMatnr()
            {
                SrmMatnr1 = no,
                SapMatnr = data.SapMatnr,
                MatnrGroup = data.MatnrGroup,
                Description = data.Description,
                Version = data.Version,
                Material = data.Material,
                Length = data.Length,
                Width = data.Width,
                Height = data.Height,
                Density = data.Density,
                Weight = data.Weight,
                Werks=data.Werks,
                Status = 1,
                Note = data.Note,
                Bn_num = data.Bn_num,
                Gewei = data.Gewei,
                Ekgrp = data.Ekgrp,
                Major_diameter=data.Major_diameter,
                Minor_diameter=data.Minor_diameter,

                CreateDate = DateTime.Now,
                CreateBy = data.User,
                LastUpdateDate= DateTime.Now,
                LastUpdateBy = data.User,

            };


            _context.SrmMatnrs.Add(material);
            _context.SaveChanges();


            return new ViewSrmMatnr1
            {
                MatnrId = material.MatnrId,
                SrmMatnr1 = no,
            };
        }
        public SrmEkgry GetEkgrp(SrmEkgry data)
        {
            SrmEkgry ekgry = _context.SrmEkgries.Where(p => p.Empid == data.Empid).FirstOrDefault();
            string no = string.Empty;

            if (ekgry!=null)
            {
                no = ekgry.Ekgry;
            }

            return new SrmEkgry
            {
                Ekgry = no,
            };
        }
        public PagingResultModel<SrmMaterialGroup> GetGroupList(QueryMaterial query)
        {
            int skip = (query.Page - 1) * query.Size;
            var resultQuery = (from material in _context.SrmMaterialGroups
                               select material);
            var materials = resultQuery.Skip(skip).Take(query.Size).ToArray();


            return new PagingResultModel<SrmMaterialGroup>
            {
                Data = JsonConvert.DeserializeObject<SrmMaterialGroup[]>(JsonConvert.SerializeObject(materials)),
                Count = resultQuery.Count()
            };
        }
        public PagingResultModel<SrmWeightUnit> GetUnitList(QueryMaterial query)
        {
            int skip = (query.Page - 1) * query.Size;
            var resultQuery = (from material in _context.SrmWeightUnits
                               select material);
            var materials = resultQuery.Skip(skip).Take(query.Size).ToArray();


            return new PagingResultModel<SrmWeightUnit>
            {
                Data = JsonConvert.DeserializeObject<SrmWeightUnit[]>(JsonConvert.SerializeObject(materials)),
                Count = resultQuery.Count()
            };
        }
    }
}
