using CF.BPM.Helper.SAP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CF.Sap.Controllers
{
    [RoutePrefix("api/kpi")]
    public class KpiController : ApiController
    {
        public class DateModel
        {
            public string I_S_AUDAT { get; set; }
            public string I_E_AUDAT { get; set; }
        }
        #region 3000 3100 精密公司
        #region met007 訂單金額達成率
        #region Sap_ysd10 實際值

        [HttpPost]
        [Route("Getysd10")]
        public string Getysd10([FromBody]DateModel model)
        {
            if (string.IsNullOrWhiteSpace(model.I_S_AUDAT) || string.IsNullOrWhiteSpace(model.I_E_AUDAT))
            {
                throw new Exception("『日期』未填");
            }
            Dictionary<string, object> import = new Dictionary<string, object>();
            import.Add("I_S_AUDAT", model.I_S_AUDAT.Replace("/", "").Replace("-", "").Replace(" ", ""));
            import.Add("I_E_AUDAT", model.I_E_AUDAT.Replace("/", "").Replace("-", "").Replace(" ", ""));
            Dictionary<string, object> import_table = new Dictionary<string, object>();
            Dictionary<string, string> export = new Dictionary<string, string>();
            DataSet ds = SapHelper.RFC("sapdev_121", "Z_RFC_GET_YSD10", import, import_table, export, new string[] { "LIST_TAB2" });

            string json = JsonConvert.SerializeObject(ds);
            return json;
        }
        #endregion
        #endregion
        #endregion

        #region 1000 1200 千附實業
        #region met038 met041 756系列成本降低
        #region Sap_co04 
        [HttpPost]
        [Route("GetCO04")]
        public string GetCO04([FromBody] DateModel model)
        {
            if (string.IsNullOrWhiteSpace(model.I_S_AUDAT) || string.IsNullOrWhiteSpace(model.I_E_AUDAT))
            {
                throw new Exception("『日期』未填");
            }
            Dictionary<string, object> import = new Dictionary<string, object>();
            import.Add("I_B_GLTRI", model.I_S_AUDAT.Replace("/", "").Replace("-", "").Replace(" ", ""));
            import.Add("I_E_GLTRI", model.I_E_AUDAT.Replace("/", "").Replace("-", "").Replace(" ", ""));
            import.Add("I_WERKS", "1200");
            import.Add("I_MTART", "FERT");
            import.Add("I_OPTION", "1");
            Dictionary<string, object> import_table = new Dictionary<string, object>();
            Dictionary<string, string> export = new Dictionary<string, string>();
            export.Add("E_TYPE", null);
            export.Add("E_MESSAGE", null);
            DataSet ds = SapHelper.RFC("sapdev_120", "Z_RFC_GET_MT_COST", import, import_table, export, new string[] { "OUTPUT" });

            string json = JsonConvert.SerializeObject(ds);
            return json;
        }
        #endregion
        #endregion
        #endregion




        /// <summary>
        /// 收貨
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetPoData")]
        public HttpResponseMessage GetPoData(JObject data)
        {
            if (data == null)
            {
                return null;
            }

            Dictionary<string, object> dicImport = new Dictionary<string, object>();
            dicImport.Add("I_EBELN", data["I_EBELN"].ToString());
            dicImport.Add("I_EKORG", data["I_EKORG"].ToString());
            if (data["Date"] != null && data["Date"].Count() > 0)
            {
                dicImport.Add("I_BEDAT_FM", Convert.ToDateTime(data["Date"][0]).ToString("yyyyMMdd"));
                dicImport.Add("I_BEDAT_TO", Convert.ToDateTime(data["Date"][1]).ToString("yyyyMMdd"));
            }


            Dictionary<string, object> dicImportTable = new Dictionary<string, object>();

            Dictionary<string, string> dicExport = new Dictionary<string, string>();
            dicExport.Add("E_TYPE", null);
            dicExport.Add("E_MESSAGE", null);
            string[] strExportTable = new string[] { "T_EKKO", "T_EKPO" };
            try
            {
                string id = "sapdev_121";
                if (data["I_EKORG"].ToString() != "3100")
                { id = "sapdev_120"; }
                DataSet dsSTOCK = SapHelper.RFC(id, "Z_RFC_GET_PO_DATA",
                dicImport,
                dicImportTable,
                dicExport,
                strExportTable);

                var aaa = dsSTOCK.Tables["T_EKKO"];
                var bbb = dsSTOCK.Tables["T_EKPO"];
                if (dicExport["E_TYPE"] == "E")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, dicExport["E_MESSAGE"]);
                }
                else if (dicExport["E_TYPE"] == "")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "sap server找不到該筆採購單"); 
                }
                return Request.CreateResponse(HttpStatusCode.OK, dsSTOCK);
                //return JsonConvert.SerializeObject(aaa);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "sap報錯");
            }
        }

    }
}
