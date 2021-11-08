using Convience.EntityFrameWork.Infrastructure;
using System;
using System.Collections.Generic;


#nullable disable

namespace Convience.Entity.Entity.SRM
{

    [Entity(DbContextType = typeof(SRMContext))]
    public partial class SrmEqpH
    {
        public int EqpId { get; set; }
        public string EqpNum { get; set; }
        public int? PoId { get; set; }
        public string WoNum { get; set; }
        public int? MatnrId { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public double? PoQty { get; set; }
        public double? NgQty { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string CauseDept { get; set; }
        public string Dispoaition { get; set; }
        public string QcDispoaition { get; set; }
        public string QcNote { get; set; }
        public string ReworkCosts { get; set; }
        public string Qdr { get; set; }
        public string QdrNum { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateBy { get; set; }
        public string NgDesc { get; set; }
        public string CauseAnalyses { get; set; }
        public string PeAction { get; set; }
        public string no { get; set; }
        public int? Status { get; set; }
        public string FileName { get; set; }
    }
    public partial class ViewSrmEqpH : SrmEqpH
    {
        public string matnr { get; set; }
        public string filename { get; set; }
        public string ekgry { get; set; }
        public string ekgryid { get; set; }
        public string poNum { get; set; }
        public int vendorid { get; set; }
        public int? vstatus { get; set; }
        public string StatusDesc { get; set; }
        public string txtSN { get; set; }
        public string CreateByName { get; set; }

    }
 }
