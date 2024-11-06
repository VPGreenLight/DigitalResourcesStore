using System;
using System.Collections.Generic;
using DigitalResourcesStore.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework;

public partial class DigitalResourcesStoreDbContext : DbContext
{
    public DigitalResourcesStoreDbContext()
    {
    }

    public DigitalResourcesStoreDbContext(DbContextOptions<DigitalResourcesStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminReply> AdminReplies { get; set; }

    public virtual DbSet<Adv> Advs { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ContactRequest> ContactRequests { get; set; }

    public virtual DbSet<DepositHistory> DepositHistories { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Footer> Footers { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MessageSupport> MessageSupports { get; set; }

    public virtual DbSet<OffersInviteFriend> OffersInviteFriends { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderHistory> OrderHistories { get; set; }

    public virtual DbSet<OrderHistoryDetail> OrderHistoryDetails { get; set; }

    public virtual DbSet<OrderReport> OrderReports { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    public virtual DbSet<ReferralProgram> ReferralPrograms { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Share> Shares { get; set; }

    public virtual DbSet<TypeRank> TypeRanks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFavorite> UserFavorites { get; set; }

    public virtual DbSet<VipCustomer> VipCustomers { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
         => optionsBuilder.UseSqlServer("Server=DESKTOP-UDKT6N6\\SQLEXPRESS;Database=BanTheCao;User=sa;Password=sa;Integrated Security=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminReply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AdminRep__3214EC2776FAE7E4");

            entity.Property(e => e.ReplyDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Adv>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Adv__3214EC2731ABFFEF");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brand__3214EC27129D8F41");

            entity.HasOne(d => d.Category).WithMany(p => p.Brands).HasConstraintName("FK__Brand__CategoryI__6E01572D");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC27E1FF4245");
        });

        modelBuilder.Entity<ContactRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContactR__3214EC277AF62BE1");

            entity.HasOne(d => d.AdminReply).WithMany(p => p.ContactRequests).HasConstraintName("FK_ContactRequest_AdminReply");

            entity.HasOne(d => d.User).WithMany(p => p.ContactRequests).HasConstraintName("FK_ContactRequest_User");
        });

        modelBuilder.Entity<DepositHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DepositH__3214EC273074D29D");

            entity.HasOne(d => d.User).WithMany(p => p.DepositHistories).HasConstraintName("FK__DepositHi__UserI__70DDC3D8");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC27DE1A7D0E");

            entity.HasOne(d => d.Product).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__Produc__71D1E811");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__UserID__72C60C4A");
        });

        modelBuilder.Entity<Footer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Footer__3214EC277C0A5B45");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Menu__3214EC270B25F832");
        });

        modelBuilder.Entity<MessageSupport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MessageS__3214EC27B4FDEB24");

            entity.HasOne(d => d.AdminReply).WithMany(p => p.MessageSupports).HasConstraintName("FK_MessageSupport_AdminReply");

            entity.HasOne(d => d.User).WithMany(p => p.MessageSupports).HasConstraintName("FK_MessageSupport_User");
        });

        modelBuilder.Entity<OffersInviteFriend>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OffersIn__3214EC27F675F816");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC27E6C0DB0F");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders).HasConstraintName("FK__Order__ProductID__75A278F5");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("FK__Order__UserID__76969D2E");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders).HasConstraintName("FK__Order__VoucherID__778AC167");
        });

        modelBuilder.Entity<OrderHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderHis__3214EC276E5019C9");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderHistories).HasConstraintName("FK__OrderHist__Produ__787EE5A0");

            entity.HasOne(d => d.User).WithMany(p => p.OrderHistories).HasConstraintName("FK__OrderHist__UserI__797309D9");
        });

        modelBuilder.Entity<OrderHistoryDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderHis__3214EC27695947A7");

            entity.HasOne(d => d.Category).WithMany(p => p.OrderHistoryDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderHist__Categ__7A672E12");

            entity.HasOne(d => d.OrderHistory).WithMany(p => p.OrderHistoryDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderHist__Order__7B5B524B");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.OrderHistoryDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderHist__Produ__4AB81AF0");
        });

        modelBuilder.Entity<OrderReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderRep__3214EC07ECB72FFA");

            entity.HasOne(d => d.OrderHistory).WithMany(p => p.OrderReports).HasConstraintName("FK_OrderReport_OrderHistory");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC27294F7C43");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products).HasConstraintName("FK__Product__BrandID__7E37BEF6");

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK__Product__Categor__00200768");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductD__3214EC27DC98BDDE");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDetails).HasConstraintName("FK__ProductDe__Produ__4E88ABD4");
        });

        modelBuilder.Entity<ReferralProgram>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Referral__3214EC27D730F31D");

            entity.Property(e => e.ReferralDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Referred).WithMany(p => p.ReferralProgramReferreds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReferralProgram_User_Referred");

            entity.HasOne(d => d.Referrer).WithMany(p => p.ReferralProgramReferrers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReferralProgram_User_Referrer");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC2727D21CA2");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Share>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Share__3213E83FD3DED7B2");

            entity.Property(e => e.Isdelete).HasDefaultValue(false);
        });

        modelBuilder.Entity<TypeRank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TypeRank__3214EC27404420A1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC27B9EEB104");

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("FK__User__RoleID__03F0984C");

            entity.HasOne(d => d.TypeRank).WithMany(p => p.Users).HasConstraintName("FK_User_TypeRank");

            entity.HasOne(d => d.VipCustomer).WithMany(p => p.Users).HasConstraintName("FK_User_VipCustomer");
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserFavo__3214EC27A83B3856");

            entity.HasOne(d => d.Product).WithMany(p => p.UserFavorites).HasConstraintName("FK_UserFavorite_Product");

            entity.HasOne(d => d.User).WithMany(p => p.UserFavorites).HasConstraintName("FK_UserFavorite_User");
        });

        modelBuilder.Entity<VipCustomer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VipCusto__3214EC27F9F61B72");

            entity.HasOne(d => d.Rank).WithMany(p => p.VipCustomers).HasConstraintName("FK_VipCustomer_TypeRank");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Voucher__3214EC275A5D607E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
