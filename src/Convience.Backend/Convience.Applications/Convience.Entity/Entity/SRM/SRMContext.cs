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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
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

                entity.Property(e => e.Matnr)
                    .HasMaxLength(18)
                    .HasColumnName("MATNR")
                    .HasComment("SAP料號");

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
                    .HasColumnName("RFQ_NUM")
                    .HasComment("詢價單號");

                entity.Property(e => e.Sourcer)
                    .HasMaxLength(8)
                    .HasColumnName("SOURCER")
                    .HasComment("詢價人員");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasComment("狀態");
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

                entity.Property(e => e.Qot)
                    .HasColumnName("QOT")
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

                entity.Property(e => e.Vendor)
                    .HasMaxLength(8)
                    .HasColumnName("VENDOR")
                    .HasComment("SAP供應商代碼");

                entity.Property(e => e.VendorName)
                    .HasMaxLength(20)
                    .HasColumnName("VENDOR_NAME")
                    .HasComment("供應商名稱");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
