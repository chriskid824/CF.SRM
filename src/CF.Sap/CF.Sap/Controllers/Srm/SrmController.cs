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
    [RoutePrefix("api/srm")]
    public class SrmController : ApiController
    {
        // GET api/values
        public string Get()
        {
            Dictionary<string, object> hmImportValues = new Dictionary<string, object>();
            Dictionary<string, object> hmImportTables = new Dictionary<string, object>();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("EBELN", "");
            row.Add("EBELP", "");
            row.Add("BUDAT", "");
            row.Add("XBLNR", "");
            row.Add("USNAM", "");
            row.Add("MENGE", "1");
            row.Add("SGTXT", "");
            row.Add("WEMPF", "");
            row.Add("ABLAD", "");

            rows.Add(row);

            hmImportTables.Add("T_RECEIPT", rows);

            Dictionary<string, string> export = new Dictionary<string, string>();
            export.Add("E_TYPE", null);
            export.Add("E_MESSAGE", null);

            string[] table = new string[] { "T_RECEIPT" };

            DataSet ds = SapHelper.RFC("sapdev_121", "Z_RFC_CREATE_RECEIPT_DATA", hmImportValues, hmImportTables, export, table);
            if (export["E_TYPE"] == "E")
            {
                return export["E_MESSAGE"];
            }
            return null;
        }

        // GET api/values/5
        public string Get(int id)
        {
            //Dictionary<string, object> import = new Dictionary<string, object>();
            //import.Add("I_B_GLTRI", "20210101");
            //import.Add("I_E_GLTRI", "20211231");
            //import.Add("I_WERKS", "1200");
            //import.Add("I_MTART", "FERT");
            //import.Add("I_OPTION", "1");
            //Dictionary<string, object> import_table = new Dictionary<string, object>();
            //Dictionary<string, string> export = new Dictionary<string, string>();
            //export.Add("E_TYPE", null);
            //export.Add("E_MESSAGE", null);
            //DataSet ds = SapHelper.RFC("sapdev_120", "Z_RFC_GET_MT_COST", import, import_table, export, new string[] { "OUTPUT" });

            //string json = JsonConvert.SerializeObject(ds);
            //return json;
            // 庫存資料確認
            Dictionary<string, object> dicImport = new Dictionary<string, object>();
            //dicImport.Add("I_MATNR", "T169830188");
            dicImport.Add("I_EKORG", "1");
            dicImport.Add("I_BEDAT_FM", "20010101");
            dicImport.Add("I_BEDAT_TO", "20211231");

            Dictionary<string, object> dicImportTable = new Dictionary<string, object>();

            Dictionary<string, string> dicExport = new Dictionary<string, string>();
            dicExport.Add("E_TYPE", null);
            dicExport.Add("E_MESSAGE", null);
            string[] strExportTable = new string[] { "T_EKKO", "T_EKPO" };



            DataSet dsSTOCK = SapHelper.RFC("sapdev_121", "Z_RFC_GET_PO_DATA",
            dicImport,
            dicImportTable,
            dicExport,
            strExportTable);
            return JsonConvert.SerializeObject(dsSTOCK);
        }

        /// <summary>
        /// 收貨
        /// </summary>
        /// <param name="datalist"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("T_RECEIPT")]
        public string T_RECEIPT(JArray datalist)
        {
            if (datalist == null)
            {
                return null;
            }
            Dictionary<string, object> hmImportValues_121 = new Dictionary<string, object>();
            Dictionary<string, object> hmImportValues_120 = new Dictionary<string, object>();
            Dictionary<string, object> hmImportTables_121 = new Dictionary<string, object>();
            Dictionary<string, object> hmImportTables_120 = new Dictionary<string, object>();
            List<Dictionary<string, string>> rows_121 = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> rows_120 = new List<Dictionary<string, string>>();
            foreach (var data in datalist)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("EBELN", data["PoNum"].ToString());
                row.Add("EBELP", data["PoLId"].ToString());
                row.Add("BUDAT", DateTime.Now.ToString("yyyyMMdd"));
                row.Add("XBLNR", data["DeliveryNum"].ToString());
                row.Add("USNAM", "MM37");
                row.Add("MENGE", data["Qty"].ToString());
                row.Add("SGTXT", data["Description"].ToString());
                row.Add("WEMPF", data["LastUpdateBy"].ToString());
                row.Add("ABLAD", data["DeliveryPlace"].ToString());

                //row.Add("EBELN", "2600000041");
                //row.Add("EBELP", "00100");
                //row.Add("BUDAT", "20210518");
                //row.Add("XBLNR", data["DeliveryNum"].ToString());
                //row.Add("USNAM", "MM37");
                //row.Add("MENGE", "1");
                //row.Add("SGTXT", "test");
                //row.Add("WEMPF", "test");
                //row.Add("ABLAD", "test");
                if (data["Org"].ToString() == "3100")
                {
                    rows_121.Add(row);
                }
                else
                {
                    rows_120.Add(row);
                }
            }

            hmImportTables_121.Add("T_RECEIPT", rows_121);
            hmImportTables_120.Add("T_RECEIPT", rows_120);

            Dictionary<string, string> export = new Dictionary<string, string>();
            export.Add("E_TYPE", null);
            export.Add("E_MESSAGE", null);

            string[] table = new string[] { "T_RECEIPT" };
            try
            {
                if (rows_121.Count > 0)
                {
                    DataSet ds = SapHelper.RFC("sapdev_121", "Z_RFC_CREATE_RECEIPT_DATA", hmImportValues_121, hmImportTables_121, export, table);
                    var aaa = ds.Tables["T_RECEIPT"];
                    if (export["E_TYPE"] == "E")
                    {
                        return export["E_MESSAGE"];
                    }
                    else if (export["E_TYPE"] == "")
                    {
                        return "sap server找不到該筆採購單";
                    }
                }
                else if (rows_120.Count > 0)
                {
                    DataSet ds = SapHelper.RFC("sapdev_120", "Z_RFC_CREATE_RECEIPT_DATA", hmImportValues_120, hmImportTables_120, export, table);
                    var aaa = ds.Tables["T_RECEIPT"];
                    if (export["E_TYPE"] == "E")
                    {
                        return export["E_MESSAGE"];
                    }
                    else if (export["E_TYPE"] == "")
                    {
                        return "sap server找不到該筆採購單";
                    }
                }
            }
            catch (Exception e)
            {
                return "sap報錯";
            }

            //q.deliveryNum = data["deliveryNum"].ToString();
            return null;
        }

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
                // sapdev_121
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


        /// <summary>
        /// 收貨
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RFC_GET_MATNR_DOC")]
        public HttpResponseMessage RFC_GET_MATNR_DOC(JObject data)
        {
            if (data == null)
            {
                return null;
            }

            Dictionary<string, object> dicImport = new Dictionary<string, object>();
            dicImport.Add("I_EBELN", data["I_EBELN"].ToString());
            dicImport.Add("I_EBELP", data["I_EBELP"].ToString());
            dicImport.Add("I_MATNR", data["I_MATNR"].ToString());
            dicImport.Add("I_WERKS", data["I_WERKS"].ToString());

            //dicImport.Add("I_EBELN", "2200001725");
            //dicImport.Add("I_EBELP", "10");
            //dicImport.Add("I_MATNR", "");
            //dicImport.Add("I_WERKS", "3100");

            Dictionary<string, object> dicImportTable = new Dictionary<string, object>();

            Dictionary<string, string> dicExport = new Dictionary<string, string>();
            dicExport.Add("E_TYPE", null);
            dicExport.Add("E_MESSAGE", null);
            string[] strExportTable = new string[] { "T_DRAD" };
            try
            {
                string id = "sapdev_121";
                if (data["I_WERKS"].ToString() != "3100")
                { id = "sapdev_120"; }
                DataSet dsSTOCK = SapHelper.RFC(id, "Z_RFC_GET_MATNR_DOC",
                dicImport,
                dicImportTable,
                dicExport,
                strExportTable);

                var aaa = dsSTOCK.Tables["T_DRAD"];
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, "sap報錯");
            }
        }


        [HttpPost]
        [Route("RFC_GET_DOC_ECN")]
        public HttpResponseMessage RFC_GET_DOC_ECN(JObject data)
        {
            if (data == null)
            {
                return null;
            }

            Dictionary<string, object> dicImport = new Dictionary<string, object>();
            //dicImport.Add("I_DOKNR", data["I_DOKNR"].ToString());
            //if (data["Date"] != null && data["Date"].Count() > 0)
            //{
            //    dicImport.Add("I_DATE_FM", Convert.ToDateTime(data["Date"][0]).ToString("yyyyMMdd"));
            //    dicImport.Add("I_DATE_TO", Convert.ToDateTime(data["Date"][1]).ToString("yyyyMMdd"));
            //}
            //dicImport.Add("I_EBELN", "2200001725");
            dicImport.Add("I_DOKNR", "");
            dicImport.Add("I_DATE_FM", "20211101");
            dicImport.Add("I_DATE_TO", "20220114");

            Dictionary<string, object> dicImportTable = new Dictionary<string, object>();

            Dictionary<string, string> dicExport = new Dictionary<string, string>();
            dicExport.Add("E_TYPE", null);
            dicExport.Add("E_MESSAGE", null);
            string[] strExportTable = new string[] { "T_RECORD" };
            try
            {
                DataSet dsSTOCK = SapHelper.RFC("sapdev_121", "Z_RFC_GET_DOC_ECN",
                dicImport,
                dicImportTable,
                dicExport,
                strExportTable);

                var aaa = dsSTOCK.Tables["T_RECORD"];
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, "sap報錯");
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
