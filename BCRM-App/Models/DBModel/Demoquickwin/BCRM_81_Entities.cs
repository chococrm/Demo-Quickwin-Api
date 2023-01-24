﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BCRM_App.Models.DBModel.Demoquickwin
{
    public partial class BCRM_81_Entities : DbContext
    {
        public BCRM_81_Entities(DbContextOptions<BCRM_81_Entities> options)
            : base(options)
        {
        }

        public virtual DbSet<DemoQuickwin_Login_Info> DemoQuickwin_Login_Infos { get; set; }
        public virtual DbSet<DemoQuickwin_Customer_Info> DemoQuickwin_Customer_Infos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

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

            _ = modelBuilder.Entity<DemoQuickwin_Customer_Info>(entity =>
            {
                entity.HasKey(e => e.CustomerId);

                entity.ToTable("DemoQuickwin_Customer_Info");

                entity.Property(e => e.CustomerId).ValueGeneratedNever();

                entity.Property(e => e.Identity_SRef).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(250);

                entity.Property(e => e.LastName).HasMaxLength(250);

                entity.Property(e => e.IdCard)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.MobileNo)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.PostalCode).HasMaxLength(10);

                entity.Property(e => e.Province).HasMaxLength(50);

                entity.Property(e => e.District).HasMaxLength(50);

                entity.Property(e => e.SubDistrict).HasMaxLength(50);

                
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
