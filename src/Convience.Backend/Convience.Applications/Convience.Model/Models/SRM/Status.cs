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
        檢驗中 = 11,
        已交貨 = 12,
        待交貨 = 13,
        已通知 = 14,
        失效 = 15,
        完成 = 16,
        刪除 = 17,
        作廢 = 18
    }
}
