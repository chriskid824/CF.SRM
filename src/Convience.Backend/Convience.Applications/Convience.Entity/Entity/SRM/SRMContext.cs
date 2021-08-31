using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Convience.Entity.Entity.SRM
{
    public partial class SRMContext : DbContext
    {
        public SRMContext()
        {
        }

        public SRMContext(DbContextOptions<SRMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SrmMatnr> SrmMatnrs { get; set; }
        public virtual DbSet<SrmRfqH> SrmRfqHs { get; set; }
        public virtual DbSet<SrmRfqM> SrmRfqMs { get; set; }
        public virtual DbSet<SrmRfqV> SrmRfqVs { get; set; }
        public virtual DbSet<SrmVendor> SrmVendors { get; set; }
        public virtual DbSet<SrmPoH> SrmPoHs { get; set; }
        public virtual DbSet<SrmPoL> SrmPoLs { get; set; }
        public virtual DbSet<SrmQotH> SrmQotHs { get; set; }
        public virtual DbSet<SrmEkgry> SrmEkgries { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<SrmQotMaterial> SrmQotMaterial { get; set; }
        public virtual DbSet<SrmQotProcess> SrmQotProcesses { get; set; }
        public virtual DbSet<SrmQotOther> SrmQotOthers { get; set; }
        public virtual DbSet<SrmQotSurface> SrmQotSurfaces { get; set; }
        public virtual DbSet<SrmInforecord> SrmInforecords { get; set; }
        public virtual DbSet<SrmDeliveryH> SrmDeliveryHs { get; set; }
        public virtual DbSet<SrmDeliveryL> SrmDeliveryLs { get; set; }
        public virtual DbSet<SrmCurrency> SrmCurrencies { get; set; }
        public virtual DbSet<SrmTaxcode> SrmTaxcodes { get; set; }
        public virtual DbSet<SrmStatus> SrmStatuses { get; set; }
        public virtual DbSet<SrmProcess> SrmProcesss { get; set; }
        public virtual DbSet<SrmMaterial> SrmMaterials { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Data Source=10.1.1.181;Initial Catalog=SRM;User ID=sa;Password=Chen@full");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<SrmMatnr>(entity =>
            {
                entity.HasKey(e => e.MatnrId);

                entity.ToTable("SRM_MATNR");

                entity.Property(e => e.MatnrId)
                    .HasColumnName("MATNR_ID")
                    .HasComment("料號識別碼");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(8)
                    .HasColumnName("CREATE_BY")
                    .HasComment("建立人員");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DATE")
                    .HasComment("建立日期");

                entity.Property(e => e.Density)
                    .HasMaxLength(8)
                    .HasColumnName("DENSITY")
                    .HasComment("密度");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.Height)
                    .HasColumnName("HEIGHT")
                    .HasComment("高");

                entity.Property(e => e.LastUpdateBy)
                    .HasMaxLength(8)
                    .HasColumnName("LAST_UPDATE_BY")
                    .HasComment("最後修改人員");

                entity.Property(e => e.LastUpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("LAST_UPDATE_DATE")
                    .HasComment("最後修改日期");

                entity.Property(e => e.Length)
                    .HasColumnName("LENGTH")
                    .HasComment("長");

                entity.Property(e => e.Material)
                    .HasMaxLength(10)
                    .HasColumnName("MATERIAL")
                    .HasComment("材質規格");

                entity.Property(e => e.SapMatnr)
                    .HasMaxLength(18)
                    .HasColumnName("SAP_MATNR")
                    .HasComment("SAP料號");

                entity.Property(e => e.MatnrGroup)
                    .HasMaxLength(5)
                    .HasColumnName("MATNR_GROUP");

                entity.Property(e => e.Note)
                    .HasColumnName("NOTE")
                    .HasComment("備註");

                entity.Property(e => e.SrmMatnr1)
                    .HasMaxLength(18)
                    .HasColumnName("SRM_MATNR")
                    .HasComment("SRM料號");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasComment("狀態");

                entity.Property(e => e.Version)
                    .HasMaxLength(3)
                    .HasColumnName("VERSION")
                    .HasComment("版次");

                entity.Property(e => e.Weight)
                    .HasColumnName("WEIGHT")
                    .HasComment("重量");

                entity.Property(e => e.Werks)
                    .HasColumnName("WERKS")
                    .HasComment("工廠");

                entity.Property(e => e.Width)
                    .HasColumnName("WIDTH")
                    .HasComment("寬");
            });

            modelBuilder.Entity<SrmRfqH>(entity =>
            {
                entity.HasKey(e => e.RfqId);

                entity.ToTable("SRM_RFQ_H");

                entity.Property(e => e.RfqId)
                    .HasColumnName("RFQ_ID")
                    .HasComment("詢價單識別碼");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(8)
                    .HasColumnName("CREATE_BY")
                    .HasComment("建立人員");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DATE")
                    .HasComment("建立日期");

                entity.Property(e => e.Deadline)
                    .HasColumnType("datetime")
                    .HasColumnName("DEADLINE")
                    .HasComment("詢價截止日期");

                entity.Property(e => e.EndBy)
                    .HasMaxLength(8)
                    .HasColumnName("END_BY");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("END_DATE");

                entity.Property(e => e.LastUpdateBy)
                    .HasMaxLength(8)
                    .HasColumnName("LAST_UPDATE_BY")
                    .HasComment("最後修改人員");

                entity.Property(e => e.LastUpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("LAST_UPDATE_DATE")
                    .HasComment("最後修改日期");

                entity.Property(e => e.RfqNum)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("RFQ_NUM")
                    .HasComputedColumnSql("('RFQ'+right('0000000'+CONVERT([varchar],[RFQ_ID]),(7)))", false)
                    .HasComment("詢價單號");

                entity.Property(e => e.Sourcer)
                    .HasMaxLength(8)
                    .HasColumnName("SOURCER")
                    .HasComment("詢價人員");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasComment("狀態");

                entity.Property(e => e.Werks)
                    .HasColumnName("WERKS")
                    .HasComment("工廠");
            });

            modelBuilder.Entity<SrmRfqM>(entity =>
            {
                entity.HasKey(e => e.RfqMId);

                entity.ToTable("SRM_RFQ_M");

                entity.Property(e => e.RfqMId)
                    .HasColumnName("RFQ_M_ID")
                    .HasComment("詢價單物料檔識別碼");

                entity.Property(e => e.Density)
                    .HasMaxLength(8)
                    .HasColumnName("DENSITY")
                    .HasComment("密度");

                entity.Property(e => e.Height)
                    .HasColumnName("HEIGHT")
                    .HasComment("高");

                entity.Property(e => e.Length)
                    .HasColumnName("LENGTH")
                    .HasComment("長");

                entity.Property(e => e.MachineName)
                    .HasMaxLength(10)
                    .HasColumnName("MACHINE_NAME")
                    .HasComment("機種名稱");

                entity.Property(e => e.Material)
                    .HasMaxLength(10)
                    .HasColumnName("MATERIAL")
                    .HasComment("材質規格");

                entity.Property(e => e.MatnrId)
                    .HasColumnName("MATNR_ID")
                    .HasComment("料號識別碼");

                entity.Property(e => e.Note)
                    .HasColumnName("NOTE")
                    .HasComment("備註");

                entity.Property(e => e.Qty)
                    .HasColumnName("QTY")
                    .HasComment("數量");

                entity.Property(e => e.RfqId)
                    .HasColumnName("RFQ_ID")
                    .HasComment("詢價單識別碼");

                entity.Property(e => e.Version)
                    .HasMaxLength(3)
                    .HasColumnName("VERSION")
                    .HasComment("料號版次");

                entity.Property(e => e.Weight)
                    .HasColumnName("WEIGHT")
                    .HasComment("重量");

                entity.Property(e => e.Width)
                    .HasColumnName("WIDTH")
                    .HasComment("寬");
            });

            modelBuilder.Entity<SrmRfqV>(entity =>
            {
                entity.HasKey(e => e.RfqVId);

                entity.ToTable("SRM_RFQ_V");

                entity.Property(e => e.RfqVId)
                    .HasColumnName("RFQ_V_ID")
                    .HasComment("詢價單供應商檔識別碼");

                entity.Property(e => e.RfqId)
                    .HasColumnName("RFQ_ID")
                    .HasComment("詢價單識別碼");

                entity.Property(e => e.VendorId)
                    .HasColumnName("VENDOR_ID")
                    .HasComment("供應商識別碼");
            });

            modelBuilder.Entity<SrmVendor>(entity =>
            {
                entity.HasKey(e => e.VendorId);

                entity.ToTable("SRM_VENDOR");

                entity.Property(e => e.VendorId)
                    .HasColumnName("VENDOR_ID")
                    .HasComment("供應商識別碼");

                entity.Property(e => e.Address)
                    .HasMaxLength(40)
                    .HasColumnName("ADDRESS")
                    .HasComment("地址");

                entity.Property(e => e.CellPhone)
                    .HasMaxLength(10)
                    .HasColumnName("CELL_PHONE")
                    .HasComment("手機號碼");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(8)
                    .HasColumnName("CREATE_BY")
                    .HasComment("建立人員");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DATE")
                    .HasComment("建立日期");

                entity.Property(e => e.Ekorg).HasColumnName("EKORG");

                entity.Property(e => e.Ext)
                    .HasMaxLength(5)
                    .HasColumnName("EXT")
                    .HasComment("分機");

                entity.Property(e => e.FaxNumber)
                    .HasMaxLength(15)
                    .HasColumnName("FAX_NUMBER")
                    .HasComment("傳真號碼");

                entity.Property(e => e.LastUpdateBy)
                    .HasMaxLength(8)
                    .HasColumnName("LAST_UPDATE_BY")
                    .HasComment("最後修改人員");

                entity.Property(e => e.LastUpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("LAST_UPDATE_DATE")
                    .HasComment("最後修改日期");

                entity.Property(e => e.Mail)
                    .HasMaxLength(50)
                    .HasColumnName("MAIL")
                    .HasComment("信箱");

                entity.Property(e => e.Org).HasColumnName("ORG");

                entity.Property(e => e.Person)
                    .HasMaxLength(20)
                    .HasColumnName("PERSON")
                    .HasComment("聯絡人");

                entity.Property(e => e.SrmVendor1)
                    .HasMaxLength(8)
                    .HasColumnName("SRM_VENDOR")
                    .HasComment("SRM供應商代碼");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasComment("狀態");

                entity.Property(e => e.TelPhone)
                    .HasMaxLength(15)
                    .HasColumnName("TEL_PHONE")
                    .HasComment("電話號碼");

                entity.Property(e => e.SapVendor)
                    .HasMaxLength(8)
                    .HasColumnName("SAP_VENDOR")
                    .HasComment("SAP供應商代碼");

                entity.Property(e => e.VendorName)
                    .HasMaxLength(20)
                    .HasColumnName("VENDOR_NAME")
                    .HasComment("供應商名稱");
            });

            modelBuilder.Entity<SrmQotH>(entity =>
            {
                entity.HasKey(e => e.QotId);

                entity.ToTable("SRM_QOT_H");

                entity.Property(e => e.QotId)
                    .HasColumnName("QOT_ID")
                    .HasComment("報價單識別碼");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(8)
                    .HasColumnName("CREATE_BY")
                    .HasComment("建立人員");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DATE")
                    .HasComment("建立日期");

                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("CURRENCY")
                    .HasComment("幣別");

                entity.Property(e => e.LastUpdateBy)
                    .HasMaxLength(8)
                    .HasColumnName("LAST_UPDATE_BY")
                    .HasComment("最後修改人員");

                entity.Property(e => e.LastUpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("LAST_UPDATE_DATE")
                    .HasComment("最後修改日期");

                entity.Property(e => e.LeadTime)
                    .HasColumnName("LEAD_TIME")
                    .HasComment("交貨時間");

                entity.Property(e => e.MatnrId)
                    .HasColumnName("MATNR_ID")
                    .HasComment("料號識別碼");

                entity.Property(e => e.MinQty)
                    .HasColumnName("MIN_QTY")
                    .HasComment("最小採購單數量");

                entity.Property(e => e.QotNum)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("QOT_NUM")
                    .HasComputedColumnSql("('QOT'+right('0000000'+CONVERT([varchar],[QOT_ID]),(7)))", false)
                    .HasComment("報價單號");

                entity.Property(e => e.RfqId)
                    .HasColumnName("RFQ_ID")
                    .HasComment("詢價單識別碼");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasComment("狀態");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("money")
                    .HasColumnName("TOTAL_AMOUNT")
                    .HasComment("總金額");

                entity.Property(e => e.VendorId)
                    .HasMaxLength(8)
                    .HasColumnName("VENDOR_ID")
                    .HasComment("供應商識別碼");

                entity.Property(e => e.MEmptyFlag)
                    .HasMaxLength(1)
                    .HasColumnName("M_EMPTY_FLAG")
                    .HasComment("材料空值指示碼");

                entity.Property(e => e.PEmptyFlag)
                    .HasMaxLength(1)
                    .HasColumnName("P_EMPTY_FLAG")
                    .HasComment("加工工序空值指示碼");

                entity.Property(e => e.SEmptyFlag)
                   .HasMaxLength(1)
                   .HasColumnName("S_EMPTY_FLAG")
                   .HasComment("表面處理空值指示碼");

                entity.Property(e => e.OEmptyFlag)
                   .HasMaxLength(1)
                   .HasColumnName("O_EMPTY_FLAG")
                   .HasComment("其他費用空值指示碼");

            });

            modelBuilder.Entity<SrmEkgry>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SRM_EKGRY");

                entity.Property(e => e.Ekgry)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("EKGRY");

                entity.Property(e => e.EkgryDesc)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("EKGRY_DESC");

                entity.Property(e => e.Werks)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("WERKS");

                entity.Property(e => e.Empid)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("EMPID");
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.CostNo).HasMaxLength(20);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<SrmDeliveryH>(entity =>
            {
                entity.HasKey(e => e.DeliveryId)
                    .HasName("PK__SRM_DELI__7D75E88BA780B676");

                entity.ToTable("SRM_DELIVERY_H");

                entity.Property(e => e.DeliveryId).HasColumnName("DELIVERY_ID");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(8)
                    .HasColumnName("CREATE_BY");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeliveryNum)
                    .HasMaxLength(20)
                    .HasColumnName("DELIVERY_NUM");

                entity.Property(e => e.LastUpdateBy)
                    .HasMaxLength(8)
                    .HasColumnName("LAST_UPDATE_BY");

                entity.Property(e => e.LastUpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("LAST_UPDATE_DATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("((14))");
            });

            modelBuilder.Entity<SrmDeliveryL>(entity =>
            {
                entity.HasKey(e => e.DeliveryLId)
                    .HasName("PK__SRM_DELI__0CF2FB9204EA3A51");

                entity.ToTable("SRM_DELIVERY_L");

                entity.Property(e => e.DeliveryLId).HasColumnName("DELIVERY_L_ID");

                entity.Property(e => e.DeliveryId).HasColumnName("DELIVERY_ID");

                entity.Property(e => e.DeliveryQty).HasColumnName("DELIVERY_QTY");

                entity.Property(e => e.PoId).HasColumnName("PO_ID");

                entity.Property(e => e.PoLId).HasColumnName("PO_L_ID");

                entity.Property(e => e.QmQty).HasColumnName("QM_QTY");

                entity.HasOne(d => d.Delivery)
                    .WithMany(p => p.SrmDeliveryLs)
                    .HasForeignKey(d => d.DeliveryId)
                    .HasConstraintName("FK__SRM_DELIV__DELIV__2DE6D218");
            });

            modelBuilder.Entity<SrmPoH>(entity =>
            {
                entity.HasKey(e => e.PoId)
                    .HasName("PK__SRM_PO_H__5ECDB69DD3352076");

                entity.ToTable("SRM_PO_H");

                entity.Property(e => e.PoId).HasColumnName("PO_ID");

                entity.Property(e => e.Buyer)
                    .HasMaxLength(8)
                    .HasColumnName("BUYER");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(8)
                    .HasColumnName("CREATE_BY");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("datetime")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.LastUpdateBy)
                    .HasMaxLength(8)
                    .HasColumnName("LAST_UPDATE_BY");

                entity.Property(e => e.LastUpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("LAST_UPDATE_DATE");

                entity.Property(e => e.Org).HasColumnName("ORG");

                entity.Property(e => e.PoNum)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("PO_NUM");

                entity.Property(e => e.ReplyDate)
                    .HasColumnType("datetime")
                    .HasColumnName("REPLY_DATE");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("((21))");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("money")
                    .HasColumnName("TOTAL_AMOUNT");

                entity.Property(e => e.VendorId).HasColumnName("VENDOR_ID");
            });

            modelBuilder.Entity<SrmPoL>(entity =>
            {
                entity.HasKey(e => new { e.PoId, e.PoLId })
                    .HasName("PK__SRM_PO_L__96094CB1B64DAE59");

                entity.ToTable("SRM_PO_L");

                entity.Property(e => e.PoId).HasColumnName("PO_ID");

                entity.Property(e => e.PoLId).HasColumnName("PO_L_ID");

                entity.Property(e => e.CriticalPart)
                    .HasMaxLength(5)
                    .HasColumnName("CRITICAL_PART");

                entity.Property(e => e.DeliveryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("DELIVERY_DATE");

                entity.Property(e => e.DeliveryPlace)
                    .HasMaxLength(20)
                    .HasColumnName("DELIVERY_PLACE");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.InspectionTime).HasColumnName("INSPECTION_TIME");

                entity.Property(e => e.MatnrId).HasColumnName("MATNR_ID");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("PRICE");

                entity.Property(e => e.Qty).HasColumnName("QTY");

                entity.Property(e => e.ReplyDeliveryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("REPLY_DELIVERY_DATE");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("((21))");

                entity.HasOne(d => d.Po)
                    .WithMany(p => p.SrmPoLs)
                    .HasForeignKey(d => d.PoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PO_ID");
            });

            modelBuilder.Entity<SrmQotMaterial>(entity =>
            {
                entity.HasKey(e => e.QotMId)
                    .HasName("PK_SRM_QOT_MATNR");

                entity.ToTable("SRM_QOT_MATERIAL");

                entity.Property(e => e.QotMId).HasColumnName("QOT_M_ID");

                entity.Property(e => e.Density).HasColumnName("DENSITY");

                entity.Property(e => e.Height).HasColumnName("HEIGHT");

                entity.Property(e => e.Length).HasColumnName("LENGTH");

                entity.Property(e => e.MCostPrice)
                    .HasColumnType("money")
                    .HasColumnName("M_COST_PRICE");

                entity.Property(e => e.MMaterial)
                    .HasMaxLength(18)
                    .HasColumnName("M_MATERIAL");

                entity.Property(e => e.MPrice)
                    .HasColumnType("money")
                    .HasColumnName("M_PRICE");

                entity.Property(e => e.Note).HasColumnName("NOTE");

                entity.Property(e => e.QotId).HasColumnName("QOT_ID");

                entity.Property(e => e.Weight).HasColumnName("WEIGHT");

                entity.Property(e => e.Width).HasColumnName("WIDTH");
            });

            modelBuilder.Entity<SrmQotOther>(entity =>
            {
                entity.HasKey(e => e.QotOId);

                entity.ToTable("SRM_QOT_OTHER");

                entity.Property(e => e.QotOId).HasColumnName("QOT_O_ID");

                entity.Property(e => e.ODescription)
                    .HasMaxLength(50)
                    .HasColumnName("O_DESCRIPTION");

                entity.Property(e => e.OItem)
                    .HasMaxLength(20)
                    .HasColumnName("O_ITEM");

                entity.Property(e => e.ONote).HasColumnName("O_NOTE");

                entity.Property(e => e.OPrice)
                    .HasColumnType("money")
                    .HasColumnName("O_PRICE");

                entity.Property(e => e.QotId).HasColumnName("QOT_ID");
            });

            modelBuilder.Entity<SrmQotProcess>(entity =>
            {
                entity.HasKey(e => e.QotPId);

                entity.ToTable("SRM_QOT_PROCESS");

                entity.Property(e => e.QotPId).HasColumnName("QOT_P_ID");

                entity.Property(e => e.PHours).HasColumnName("P_HOURS");

                entity.Property(e => e.PMachine)
                    .HasMaxLength(20)
                    .HasColumnName("P_MACHINE");

                entity.Property(e => e.PNote).HasColumnName("P_NOTE");

                entity.Property(e => e.PPrice)
                    .HasColumnType("money")
                    .HasColumnName("P_PRICE");

                entity.Property(e => e.PProcessNum)
                    .HasMaxLength(18)
                    .HasColumnName("P_PROCESS_NUM");

                entity.Property(e => e.QotId).HasColumnName("QOT_ID");
            });

            modelBuilder.Entity<SrmQotSurface>(entity =>
            {
                entity.HasKey(e => e.QotSId);

                entity.ToTable("SRM_QOT_SURFACE");

                entity.Property(e => e.QotSId).HasColumnName("QOT_S_ID");

                entity.Property(e => e.QotId).HasColumnName("QOT_ID");

                entity.Property(e => e.SNote).HasColumnName("S_NOTE");

                entity.Property(e => e.SPrice)
                    .HasColumnType("money")
                    .HasColumnName("S_PRICE");

                entity.Property(e => e.SProcess)
                    .HasMaxLength(18)
                    .HasColumnName("S_PROCESS");

                entity.Property(e => e.STimes).HasColumnName("S_TIMES");
            });

            modelBuilder.Entity<SrmInforecord>(entity =>
            {
                entity.HasKey(e => e.InfoId);

                entity.ToTable("SRM_INFORECORD");

                entity.Property(e => e.InfoId)
                    .HasColumnName("INFO_ID");

                entity.Property(e => e.CreateBy)
                    .HasMaxLength(8)
                    .HasColumnName("CREATE_BY");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DATE");

                entity.Property(e => e.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("EFFECTIVE_DATE");

                entity.Property(e => e.Ekgry)
                    .HasMaxLength(3)
                    .HasColumnName("EKGRY");

                entity.Property(e => e.ExpirationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("EXPIRATION_DATE");

                entity.Property(e => e.InfoNum)
                .HasMaxLength(10)
                .HasColumnName("INFO_NUM")
                .IsUnicode(false)
                .HasComputedColumnSql("('INF'+right('0000000'+CONVERT([varchar],[INFO_ID]),(7)))", false);

                entity.Property(e => e.LastUpdateBy)
                    .HasMaxLength(8)
                    .HasColumnName("LAST_UPDATE_BY");

                entity.Property(e => e.LastUpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("LAST_UPDATE_DATE");

                entity.Property(e => e.LeadTime).HasColumnName("LEAD_TIME");

                entity.Property(e => e.MatnrId).HasColumnName("MATNR_ID");

                entity.Property(e => e.MinQty).HasColumnName("MIN_QTY");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("PRICE");

                entity.Property(e => e.QotId).HasColumnName("QOT_ID");

                entity.Property(e => e.StandQty).HasColumnName("STAND_QTY");

                entity.Property(e => e.Status).HasColumnName("STATUS");

                entity.Property(e => e.Taxcode)
                    .HasMaxLength(2)
                    .HasColumnName("TAXCODE");

                entity.Property(e => e.Unit).HasColumnName("UNIT");

                entity.Property(e => e.VendorId).HasColumnName("VENDOR_ID");

                entity.Property(e => e.Note)
                    .HasColumnName("NOTE")
                    .HasComment("備註");
            });

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<SrmCurrency>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SRM_CURRENCY");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("CURRENCY")
                    .HasComment("幣別");

                entity.Property(e => e.CurrencyName)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("CURRENCY_NAME")
                    .HasComment("幣別名稱");
            });

            modelBuilder.Entity<SrmTaxcode>(entity =>
            {
                entity.HasKey(e => e.Taxcode);

                entity.ToTable("SRM_TAXCODE");

                entity.Property(e => e.Taxcode)
                    .HasMaxLength(2)
                    .HasColumnName("TAXCODE")
                    .HasComment("稅碼");

                entity.Property(e => e.TaxcodeName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("TAXCODE_NAME")
                    .HasComment("稅碼名稱");
            });
            modelBuilder.Entity<SrmStatus>(entity =>
            {
                entity.HasKey(e => e.Status)
                    .HasName("PK__SRM_STAT__AA8A8F01A18570E4");

                entity.ToTable("SRM_STATUS");

                entity.Property(e => e.Status)
                    .ValueGeneratedNever()
                    .HasColumnName("STATUS")
                    .HasComment("代碼");

                entity.Property(e => e.StatusDesc)
                    .HasMaxLength(10)
                    .HasColumnName("STATUS_DESC")
                    .HasComment("說明");
            });
            
            modelBuilder.Entity<SrmProcess>(entity =>
            {
                entity.HasKey(e => e.ProcessNum);

                entity.ToTable("SRM_PROCESS");

                entity.Property(e => e.ProcessNum).HasColumnName("PROCESS_NUM");

                entity.Property(e => e.Process)
                    .HasMaxLength(50)
                    .HasColumnName("PROCESS");

                entity.Property(e => e.Staus).HasColumnName("STAUS");
            });
            modelBuilder.Entity<SrmMaterial>(entity =>
            {
                entity.HasKey(e => e.MaterialNum);

                entity.ToTable("SRM_MATERIAL");

                entity.Property(e => e.MaterialNum).HasColumnName("MATERIAL_NUM");

                entity.Property(e => e.Material)
                    .HasMaxLength(50)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.Staus).HasColumnName("STAUS");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
