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
    public class SrmMaterialController : ControllerBase
    {
        private readonly ISrmMaterialService _srmMaterialService;
        private readonly appSettings _appSettingsService;
        private readonly ISrmSupplierService _srmSupplierService;

        public SrmMaterialController(ISrmMaterialService srmMaterialService,
            IOptions<appSettings> appSettingsOption,
            ISrmSupplierService srmSupplierService)
        {
            _srmMaterialService = srmMaterialService;
            _appSettingsService = appSettingsOption.Value;
            _srmSupplierService = srmSupplierService;
        }

        [HttpPost("GetMaterialList")]
        public IActionResult GetMaterialList(QueryMaterial query)
        {
            return Ok(_srmMaterialService.GetMaterialList(query));
        }
        [HttpPost("GetMaterialDetail")]
        public IActionResult GetSupplierDetail(QueryMaterial query)
        {
            var detail = _srmMaterialService.GetMaterialDetail(query);
            return Ok(detail);
        }
        [HttpPost("UpdateMaterial")]
        public IActionResult UpdateDeliveryL(ViewSrmMatnr1 data)
        {
            if (_srmMaterialService.UpdateMaterial(data)) return Ok();
            return BadRequest("料號資料 更新 失敗");
        }
        [HttpPost("Checkdata")]
        public IActionResult Checkdata(ViewSrmMatnr1 data)
        {
            if (string.IsNullOrWhiteSpace(_srmMaterialService.Checkdata(data))) return Ok();
            return BadRequest(_srmMaterialService.Checkdata(data));
        }
        [HttpPost("AddMatnr")]
        public IActionResult AddMatnr(ViewSrmMatnr1 data)
        {
            try
            {
                _srmMaterialService.AddMatnr(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return this.BadRequestResult("料號："+data.SrmMatnr1+"，"+ex.Message);
            }
        }
        [HttpPost("GetEkgrp")]
        public IActionResult GetEkgrp(SrmEkgry data)
        {
            return Ok(_srmMaterialService.GetEkgrp(data));
        }
        [HttpPost("GetGroupList")]
        public IActionResult GetGroupList(QueryMaterial data)
        {
            return Ok(_srmMaterialService.GetGroupList(data));
        }
        [HttpPost("GetUnitList")]
        public IActionResult GetUnitList(QueryMaterial data)
        {
            return Ok(_srmMaterialService.GetUnitList(data));
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

                string[] headers = new string[] { "料號", "物料內文", "物料群組", "工廠", "採購群組代碼", "版次", "材質規格", "長", "寬", "高(厚)", "圓外徑", "圓內徑", "密度", "重量", "重量單位", "評估案號", "備註", "數量" };
                string[] cols = new string[] { "SrmMatnr1", "Description", "MatnrGroup", "Werks", "Ekgrp", "Version", "Material", "Length", "Width", "Height", "Major_diameter", "Minor_diameter", "Density", "Weight", "Gewei", "Bn_num", "Note", "QTY" };
                string checkname = "物料內文";
                DataTable data_m = new Utility().ReadExcel(path, 0, headers, cols, checkname);
                //DataTable data_v = _srmRfqHService.ReadExcel_Vendor(path, user);
                if (data_m.Rows.Count == 0)
                {
                    throw new Exception("供應商至少需一筆");
                }
                string errTitle = "";
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        SrmRfqV[] m = JsonConvert.DeserializeObject<SrmRfqV[]>(JsonConvert.SerializeObject(data_m));
                        for (int i = 0; i < data_m.Rows.Count; i++)
                        {
                            DataRow dr_m = data_m.Rows[i];
                            errTitle = $"料號:{dr_m["SrmMatnr1"].ToString()}，";

                            ViewSrmMatnr1 temp = JsonConvert.DeserializeObject<ViewSrmMatnr1>(Convert_DataRowToJson(dr_m));
                            temp.User = user.UserName;
                            _srmMaterialService.AddMatnr(temp);
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
    }
}
