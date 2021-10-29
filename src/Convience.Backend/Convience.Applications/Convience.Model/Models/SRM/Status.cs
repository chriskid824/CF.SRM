using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Model.Models.SRM
{
    public enum Status
    {
        初始 = 1,
        詢價 = 2,
        接收 = 3,
        報價 = 4,
        確認 = 5,
        拒絕 = 6,
        啟動 = 7,
        簽核中 = 8,
        已核發 = 9,
        已拋轉 = 10,
        已接收 = 11,
        已回覆 = 12,
        檢驗中 = 13,
        已交貨 = 14,
        待交貨 = 15,
        已通知 = 16,
        失效 = 17,
        完成 = 18,
        刪除 = 19,
        作廢 = 20
    }

    public class appSettings
    {
        public string Environment { get; set; }
        public string CurrentDirectory { get; set; }
        public string FileDirectory { get; set; }
    }
}
