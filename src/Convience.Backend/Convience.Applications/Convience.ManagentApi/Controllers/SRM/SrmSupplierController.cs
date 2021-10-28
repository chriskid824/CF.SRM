using Convience.Entity.Entity.SRM;
using Convience.JwtAuthentication;
using Convience.Mail;
using Convience.ManagentApi.Infrastructure.Authorization;
using Convience.ManagentApi.Infrastructure.Logs;
using Convience.Model.Models.ContentManage;
using Convience.Model.Models.SRM;
using Convience.Model.Models.SystemManage;
using Convience.Service.SRM;
using Convience.Service.SystemManage;
using Convience.Util.Extension;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Convience.ManagentApi.Controllers.SRM
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SrmSupplierController : ControllerBase
    {
        private readonly ISrmSupplierService _srmSupplierService;
        private readonly appSettings _appSettingsService;
        private readonly ISrmRfqHService _srmRfqHService;

        public SrmSupplierController(ISrmSupplierService srmSupplierService, 
            IOptions<appSettings> appSettingsOption,
            ISrmRfqHService srmRfqHService)
        {
            _srmSupplierService = srmSupplierService;
            _appSettingsService = appSettingsOption.Value;
            _srmRfqHService = srmRfqHService;

        }
        [HttpPost("GetSupplierList")]
        public IActionResult GetSupplierList(QueryVendorModel query)
        {
            return Ok(_srmSupplierService.GetVendor(query));
        }
        [HttpPost("GetSupplierDetail")]
        public IActionResult GetSupplierDetail(QueryVendorModel query)
        {
            var detail = _srmSupplierService.GetSupplierDetail(query);
            return Ok(detail);
        }
        [HttpPost("UpdateSupplier")]
        public IActionResult UpdateDeliveryL(ViewSrmSupplier dls)
        {
            if (_srmSupplierService.UpdateSupplier(dls)) return Ok();
            return BadRequest("供應商名稱已重複使用");
        }
        [HttpPost("Checkdata")]
        public IActionResult Checkdata(ViewSrmSupplier data)
        {
            if (string.IsNullOrWhiteSpace(_srmSupplierService.Checkdata(data))) return Ok();
            return BadRequest(_srmSupplierService.Checkdata(data));
        }
        [HttpPost("AddSupplier")]
        public IActionResult AddSupplier(ViewSrmSupplier data)
        {
            try
            {
                _srmSupplierService.AddVendor(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
        }
        private string Convert_DataRowToJson(DataRow datarow)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in datarow.Table.Columns)
            {
                dict.Add(col.ColumnName, datarow[col]);
            }
            return JsonConvert.SerializeObject(dict);
        }
        [HttpPost("BatchUpload")]
        public IActionResult BatchUpload([FromForm] Model.Models.SRM.FileUploadViewModel_RFQ fileUploadModel)
        {
            UserClaims user = User.GetUserClaims();
            fileUploadModel.CreateBy = user.UserName;
            fileUploadModel.CurrentDirectory = _appSettingsService.CurrentDirectory + fileUploadModel.CurrentDirectory;
            string path = "";
            try
            {
                if (user.Werks.Count() > 1)
                {
                    //throw new Exception("只限單一工廠人員申請");
                }
                path = _srmSupplierService.Upload(fileUploadModel);

                string[] headers = new string[] { "供應商編號", "供應商名稱", "公司代碼", "採購組織", "聯絡人", "地址", "信箱", "傳真號碼", "電話號碼", "分機", "手機號碼" };
                string[] cols = new string[] { "SrmVendor1", "VendorName", "Org", "Ekorg", "Person", "Address", "Mail", "FaxNumber", "TelPhone", "Ext", "CellPhone" };
                string checkname = "供應商名稱";
                DataTable data_v = new Utility().ReadExcel(path, 0,headers,cols, checkname);
                //DataTable data_v = _srmRfqHService.ReadExcel_Vendor(path, user);
                if (data_v.Rows.Count == 0)
                {
                    throw new Exception("供應商至少需一筆");
                }
                string errTitle = "";
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        for (int i = 0; i < data_v.Rows.Count; i++)
                        {
                            DataRow dr_v = data_v.Rows[i];
                            errTitle = $"供應商:{dr_v["SrmVendor1"].ToString()}，";

                            ViewSrmSupplier temp = JsonConvert.DeserializeObject<ViewSrmSupplier>(Convert_DataRowToJson(dr_v));
                            temp.User = user.UserName;
                            _srmSupplierService.AddVendor(temp);
                        }

                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        transaction.Dispose();
                        throw new Exception(errTitle + ex.Message);
                    }
                    finally
                    {
                    if (!string.IsNullOrWhiteSpace(path))
                        {
                            _srmSupplierService.Delete(path);
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
        }
        [HttpPost("DeleteList")]
        public IActionResult DeleteList(ViewSrmSupplier data)
        {
            try
            {
                return Ok(_srmSupplierService.DeleteList(data));
            }
            catch (Exception ex)
            {
                return this.BadRequestResult(ex.Message);
            }
        }
    }
}
