using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ItemEntity> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .ToTable("tblUsers", "User");

        modelBuilder.Entity<IdentityUserClaim<int>>()
            .ToTable("tblUserClaims", "User");

        modelBuilder.Entity<IdentityUserLogin<int>>()
            .ToTable("tblUserLogins", "User");

        modelBuilder.Entity<IdentityUserToken<int>>()
            .ToTable("tblUserTokens", "User");

        modelBuilder.Entity<IdentityRole<int>>()
            .ToTable("tblRoles", "User");

        modelBuilder.Entity<IdentityRoleClaim<int>>()
            .ToTable("tblRoleClaims", "User");

        modelBuilder.Entity<IdentityUserRole<int>>()
            .ToTable("tblUserRoles", "User");


        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.ToTable("Product", "dbo");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ProductName)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.CreatedBy)
                .IsRequired();

            entity.Property(x => x.CreatedOn)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(x => x.ModifiedBy)
                .IsRequired(false);

            entity.Property(x => x.ModifiedOn)
                .HasColumnType("datetime")
                .IsRequired(false);
        });

        modelBuilder.Entity<ItemEntity>(entity =>
        {
            entity.ToTable("Item", "dbo");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Quantity)
                .IsRequired();

            entity.HasOne(x => x.Product)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}