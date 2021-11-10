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
using System.IO;
using NPOI.SS.UserModel;
using System.Data;
using NPOI.XSSF.UserModel;
using Convience.Filestorage.Abstraction;

namespace Convience.Service.SRM
{
    public interface ISrmSupplierService
    {
        /// <summary>
        /// 取得全部角色
        /// </summary>
        //public PagingResultModel<ViewSrmSupplier> GetVendor();
        public PagingResultModel<ViewSrmSupplier> GetVendor(QueryVendorModel vendorQuery);

        public ViewSrmSupplier GetSupplierDetail(QueryVendorModel query);

        public bool UpdateSupplier(ViewSrmSupplier data);
        public string Checkdata(ViewSrmSupplier data);
        public ViewSrmSupplier AddVendor(ViewSrmSupplier data);
        public string Upload(Model.Models.SRM.FileUploadViewModel_RFQ fileUploadModel);
        public void Delete(string path);
        public bool DeleteList(ViewSrmSupplier data);
    }
    public class SrmSupplierService : ISrmSupplierService
    {
        private readonly SRMContext _context;

        private readonly IRepository<SrmSupplier> _srmVendorRepository;



        public SrmSupplierService(IRepository<SrmSupplier> srmVendorRepository, SRMContext context)
        {
            //_mapper = mapper;
            _srmVendorRepository = srmVendorRepository;
            _context = context;
        }


        public PagingResultModel<ViewSrmSupplier> GetVendor(QueryVendorModel vendorQuery)
        {
            int skip = (vendorQuery.Page-1) * vendorQuery.Size;

            var resultQuery = (from vendor in _context.SrmVendors
                          join status in _context.SrmStatuses on vendor.Status equals status.Status
                          select new ViewSrmSupplier
                          {
                              VendorId = vendor.VendorId,
                              SrmVendor1 = vendor.SrmVendor1,
                              SapVendor = vendor.SapVendor,
                              VendorName = vendor.VendorName,
                              Org = vendor.Org,
                              Ekorg = vendor.Ekorg,
                              Person = vendor.Person,
                              Address = vendor.Address,
                              TelPhone = vendor.TelPhone,
                              Ext = vendor.Ext,
                              FaxNumber = vendor.FaxNumber,
                              CellPhone = vendor.CellPhone,
                              Mail = vendor.Mail,
                              StatusDesc = status.StatusDesc, 
                          })
                          .AndIfHaveValue(vendorQuery.Code, r => r.SrmVendor1.Contains(vendorQuery.Code))
                          .AndIfHaveValue(vendorQuery.Vendor, r => r.VendorName.Contains(vendorQuery.Vendor))
                .Where(r => vendorQuery.Werks.Contains(r.Ekorg.Value));
            
            var vendors = resultQuery.Skip(skip).Take(vendorQuery.Size).ToArray();

            return new PagingResultModel<ViewSrmSupplier>
            {
                Data = JsonConvert.DeserializeObject<ViewSrmSupplier[]>(JsonConvert.SerializeObject(vendors)),
                Count = resultQuery.Count()
            };
        }
        public ViewSrmSupplier GetSupplierDetail(QueryVendorModel query)
        {
            var supplier = (from vendor in _context.SrmVendors
                            join status in _context.SrmStatuses on vendor.Status equals status.Status
                            select new ViewSrmSupplier
                            {
                                SrmVendor1 = vendor.SrmVendor1,
                                SapVendor = vendor.SapVendor,
                                VendorName = vendor.VendorName,
                                Org = vendor.Org,
                                Ekorg = vendor.Ekorg,
                                Person = vendor.Person,
                                Address = vendor.Address,
                                TelPhone = vendor.TelPhone,
                                Ext = vendor.Ext,
                                FaxNumber = vendor.FaxNumber,
                                CellPhone = vendor.CellPhone,
                                Mail = vendor.Mail,
                                StatusDesc = status.StatusDesc,
                            })
                          .Where(r => r.SrmVendor1 == query.Code).FirstOrDefault()
                          //.Where(r => r.Org == query.Org)
                          //.Where(r => r.Ekorg == query.Ekorg)
                          ;

            //var suppliers = supplier.ToArray();
            //
            return new ViewSrmSupplier
            {
                SrmVendor1 = supplier.SrmVendor1,
                SapVendor = supplier.SapVendor,
                VendorName = supplier.VendorName,
                Org = supplier.Org,
                Ekorg = supplier.Ekorg,
                Person = supplier.Person,
                Address = supplier.Address,
                TelPhone = supplier.TelPhone,
                Ext = supplier.Ext,
                FaxNumber = supplier.FaxNumber,
                CellPhone = supplier.CellPhone,
                Mail = supplier.Mail,
                StatusDesc = supplier.StatusDesc,
            };
        }
        public bool UpdateSupplier(ViewSrmSupplier data)
        {

            SrmVendor vendor = _context.SrmVendors.Where(p => p.SrmVendor1 == data.SrmVendor1).FirstOrDefault();
            //SrmStatus status = _context.SrmStatuses.Where(p => p.StatusDesc == data.StatusDesc).FirstOrDefault();
            SrmVendor vendorname = _context.SrmVendors.Where(p => p.VendorName == data.VendorName && p.Ekorg == data.Ekorg && p.SrmVendor1 != data.SrmVendor1).FirstOrDefault();


            if (vendorname!=null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(data.SapVendor))
            {
                vendor.SrmVendor1 = data.SrmVendor1;
            }
            else
            {
                vendor.SrmVendor1 = data.SapVendor;
            }
            vendor.SapVendor = data.SapVendor;
            vendor.VendorName = data.VendorName;
            vendor.Org = data.Org;
            vendor.Ekorg = data.Ekorg;
            vendor.Person = data.Person;
            vendor.Address = data.Address;
            vendor.TelPhone = data.TelPhone;
            vendor.Ext = data.Ext;
            vendor.FaxNumber = data.FaxNumber;
            vendor.CellPhone = data.CellPhone;
            vendor.Mail = data.Mail;
            //vendor.Status = status.Status;
            vendor.LastUpdateDate = DateTime.Now;
            vendor.LastUpdateBy = data.User;


            _context.SrmVendors.Update(vendor);

            _context.SaveChanges();

            return true;
        }
        public string Checkdata(ViewSrmSupplier data)
        {
            string msg = string.Empty;

            SrmVendor srmvendor = _context.SrmVendors.Where(p => p.SrmVendor1 == data.SrmVendor1).FirstOrDefault();
            SrmVendor sapvendor = _context.SrmVendors.Where(p => p.SapVendor == data.SapVendor).FirstOrDefault();
            SrmVendor vendorname = _context.SrmVendors.Where(p => p.VendorName == data.VendorName).FirstOrDefault();

            if (vendorname != null)
            {
                msg = "供應商名稱 已重複使用";
            }
            if (!string.IsNullOrWhiteSpace(data.SapVendor))
            {
                if (sapvendor != null)
                {
                    msg = "SAP供應商代碼 已重複使用";
                }
            }
            if (srmvendor != null)
            {
                msg = "SRM供應商代碼 已重複使用";
            }

            return msg;
        }
        public ViewSrmSupplier AddVendor(ViewSrmSupplier data)
        {
            var vendor = _context.SrmVendors.Where(p => p.SrmVendor1.StartsWith("S")).Max(p1 => p1.SrmVendor1);
            SrmVendor vendorname = _context.SrmVendors.Where(p => p.VendorName == data.VendorName && p.Ekorg==data.Ekorg).FirstOrDefault();
            SrmVendor srmvendor1 = _context.SrmVendors.Where(p => p.SrmVendor1 == data.SrmVendor1).FirstOrDefault();

            string v = data.SrmVendor1;


            string no = string.Empty;
            
            if (vendorname!=null)
            {
                throw new Exception("名稱已重複使用");
            }
            if (srmvendor1 != null)
            {
                throw new Exception("編號已重複使用");
            }

            if (string.IsNullOrWhiteSpace(data.VendorName))
            {
                throw new Exception("供應商名稱，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Org.ToString()))
            {
                throw new Exception("公司代碼，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Ekorg.ToString()))
            {
                throw new Exception("採購組織，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Person))
            {
                throw new Exception("聯絡人，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Address))
            {
                throw new Exception("地址，必填");
            }
            if (string.IsNullOrWhiteSpace(data.Mail))
            {
                throw new Exception("信箱，必填");
            }
            if (string.IsNullOrWhiteSpace(data.FaxNumber))
            {
                throw new Exception("傳真號碼，必填");
            }
            string sapno = string.Empty;

            if (string.IsNullOrWhiteSpace(data.SrmVendor1))
            {
                if (vendor == null)
                {
                    no = "S00001";
                }
                else
                {
                    no = "S" + (int.Parse(vendor.Substring(1, 5)) + 1).ToString().PadLeft(5, '0');
                }
            }
            else
            {
                no = data.SrmVendor1;
                if (data.SrmVendor1.Substring(0, 1) == "V")
                {
                    sapno = data.SrmVendor1;
                }
                else
                {
                    throw new Exception("編碼有誤，請重新輸入");
                }
            }

            SrmVendor srmvendor = new SrmVendor()
            {
                SrmVendor1 = no,
                SapVendor = sapno,
                VendorName = data.VendorName,
                Org = data.Org,
                Ekorg = data.Ekorg,
                Person = data.Person,
                Address = data.Address,
                TelPhone = data.TelPhone,
                Ext = data.Ext,
                FaxNumber = data.FaxNumber,
                CellPhone = data.CellPhone,
                Mail = data.Mail,
                Status = 1,

                CreateDate = DateTime.Now,
                CreateBy = data.User,
                LastUpdateDate = DateTime.Now,
                LastUpdateBy = data.User,
            };


            _context.SrmVendors.Add(srmvendor);
            _context.SaveChanges();


            return new ViewSrmSupplier
            {
                VendorId = srmvendor.VendorId,
                SrmVendor1 = no,
            };
        }
        public void Delete(string path)
        {
            try
            {
                FileInfo f = new FileInfo(path);
                if (f.Exists)
                {
                    f.Delete();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public string Upload(Model.Models.SRM.FileUploadViewModel_RFQ fileUploadModel)
        {
            Guid g = Guid.NewGuid();
            var file = fileUploadModel.Files.First();
            var path = fileUploadModel.CurrentDirectory?.TrimEnd('/') + '/' + fileUploadModel.CreateBy + '/' + g + '_' + file.FileName;
            switch (Path.GetExtension(file.FileName).ToLower())
            {
                case ".xlsx":
                    break;
                default:
                    throw new FileStoreException("限定xlsx！");
            }
            var info = new Utility.UploadFile().GetFileInfoAsync(path);
            if (info != null)
            {
                throw new FileStoreException("文件名重複！");
            }
            var stream = file.OpenReadStream();
            var result = new Utility.UploadFile().CreateFileFromStreamAsync(path, stream);
            if (string.IsNullOrEmpty(result))
            {
                throw new FileStoreException("文件上傳失敗！");
            }
            return path;
        }
        public bool DeleteList(ViewSrmSupplier data)
        {
            SrmVendor vendorid = _context.SrmVendors.Where(p => p.VendorId == data.VendorId).FirstOrDefault();
            SrmRfqV rfqv = _context.SrmRfqVs.Where(p => p.VendorId == data.VendorId).FirstOrDefault();
            SrmInforecord inforecord = _context.SrmInforecords.Where(p => p.VendorId == data.VendorId).FirstOrDefault();
            SrmQotH qoth = _context.SrmQotHs.Where(p => p.VendorId == data.VendorId).FirstOrDefault();

            if (rfqv != null || inforecord != null || qoth != null)
            {
                throw new Exception("此供應商已有申請紀錄，無法刪除");
            }

            _context.SrmVendors.Remove(vendorid);
            _context.SaveChanges();
            return true;
        }

    }
}
