using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BCRM_App.Models.DBModel.Demoquickwin
{
    public partial class BCRM_81_Entities : DbContext
    {
        public BCRM_81_Entities()
        {
        }

        public BCRM_81_Entities(DbContextOptions<BCRM_81_Entities> options)
            : base(options)
        {
        }

        public virtual DbSet<DemoQuickwin_Campaign> DemoQuickwin_Campaigns { get; set; }
        public virtual DbSet<DemoQuickwin_Campaign_Login> DemoQuickwin_Campaign_Logins { get; set; }
        public virtual DbSet<DemoQuickwin_CarRegistration_Info> DemoQuickwin_CarRegistration_Infos { get; set; }
        public virtual DbSet<DemoQuickwin_Customer_Info> DemoQuickwin_Customer_Infos { get; set; }
        public virtual DbSet<DemoQuickwin_Line_Info> DemoQuickwin_Line_Infos { get; set; }
        public virtual DbSet<DemoQuickwin_Login_Info> DemoQuickwin_Login_Infos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=dev-chocobcrm.database.windows.net;Initial Catalog=BCRM_81_BL7R8X768NUF;Persist Security Info=True;User ID=bcrm_BL7R8X768NUF;password=DebYCEhWmcjCxeMtgQ7y;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<DemoQuickwin_Campaign>(entity =>
            {
                entity.HasKey(e => e.CampaignId);

                entity.ToTable("DemoQuickwin_Campaign");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DemoQuickwin_Campaign_Login>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("DemoQuickwin_Campaign_Login");

                entity.Property(e => e.Identity_SRef)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<DemoQuickwin_CarRegistration_Info>(entity =>
            {
                entity.HasKey(e => e.PlateId);

                entity.ToTable("DemoQuickwin_CarRegistration_Info");

                entity.Property(e => e.CarRegistrationProvince).HasMaxLength(50);

                entity.Property(e => e.CarRegistration_Back).HasMaxLength(10);

                entity.Property(e => e.CarRegistration_Front).HasMaxLength(10);

                entity.Property(e => e.Identity_SRef)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<DemoQuickwin_Customer_Info>(entity =>
            {
                entity.HasKey(e => e.CustomerId);

                entity.ToTable("DemoQuickwin_Customer_Info");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.District).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IdCard).HasMaxLength(15);

                entity.Property(e => e.Identity_SRef)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.MobileNo).HasMaxLength(15);

                entity.Property(e => e.PostalCode).HasMaxLength(10);

                entity.Property(e => e.Province).HasMaxLength(50);

                entity.Property(e => e.SubDistrict).HasMaxLength(50);
            });

            modelBuilder.Entity<DemoQuickwin_Line_Info>(entity =>
            {
                entity.HasKey(e => e.AccountId);

                entity.ToTable("DemoQuickwin_Line_Info");

                entity.Property(e => e.Identity_SRef).HasMaxLength(250);

                entity.Property(e => e.LineId).HasMaxLength(250);

                entity.Property(e => e.LineName).HasMaxLength(250);

                entity.Property(e => e.LinePictureUrl).HasMaxLength(250);
            });

            modelBuilder.Entity<DemoQuickwin_Login_Info>(entity =>
            {
                entity.HasKey(e => e.AccountId)
                    .HasName("PK_Thanachart_Line_Info");

                entity.ToTable("DemoQuickwin_Login_Info");

                entity.Property(e => e.Access_Token).HasMaxLength(1000);

                entity.Property(e => e.IAM_OAuth_TX_Ref).HasMaxLength(250);

                entity.Property(e => e.Identity_SRef).HasMaxLength(50);

                entity.Property(e => e.Line_OAuth_State).HasMaxLength(250);

                entity.Property(e => e.Payload).HasMaxLength(1000);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
