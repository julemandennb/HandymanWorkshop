using HandymanWorkshop.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HandymanWorkshop.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<DepartmentProduct> DepartmentProducts { get; set; }
        public DbSet<WorkOrderProduct> WorkOrderProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<WorkOrder>(entity =>
            {
                entity.Property(w => w.Status)
               .HasConversion<string>()
               .HasMaxLength(30)
               .IsRequired();
            });

            builder.Entity<Employee>()
            .HasOne(e => e.ApplicationUser)
            .WithOne()
            .HasForeignKey<Employee>(e => e.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Customer>()
            .HasOne(c => c.ApplicationUser)
            .WithOne()
            .HasForeignKey<Customer>(c => c.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

            PivotSetUp(builder);
            ConfigureRelationships(builder);
            ConfigureSoftDeleteFilters(builder);
        }


        private void PivotSetUp(ModelBuilder builder)
        {
            builder.Entity<DepartmentProduct>(entity =>
            {
                entity.HasKey(dp => dp.Id);

                entity.HasOne(dp => dp.Department)
                    .WithMany(d => d.DepartmentProducts)
                    .HasForeignKey(dp => dp.DepartmentId);

                entity.HasOne(dp => dp.Product)
                    .WithMany(p => p.DepartmentProducts)
                    .HasForeignKey(dp => dp.ProductId);
            });


            builder.Entity<WorkOrderProduct>(entity =>
            {
                entity.HasKey(dp => dp.Id);

                entity.HasOne(wp => wp.WorkOrder)
                    .WithMany(w => w.WorkOrderProducts)
                    .HasForeignKey(wp => wp.WorkOrderId);

                entity.HasOne(wp => wp.Product)
                    .WithMany(p => p.WorkOrderProducts)
                    .HasForeignKey(wp => wp.ProductId);

                // Track which Identity user added the product
                entity.HasOne(wp => wp.AddedByUser)
                    .WithMany()
                    .HasForeignKey(wp => wp.AddedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureRelationships(ModelBuilder builder)
        {
            // Employee -> Department
            builder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkOrder -> Customer
            builder.Entity<WorkOrder>()
                .HasOne(w => w.Customer)
                .WithMany(c => c.WorkOrders)
                .HasForeignKey(w => w.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkOrder -> Employee
            builder.Entity<WorkOrder>()
                .HasOne(w => w.Employee)
                .WithMany(e => e.WorkOrders)
                .HasForeignKey(w => w.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkOrder -> Department
            builder.Entity<WorkOrder>()
                .HasOne(w => w.Department)
                .WithMany()
                .HasForeignKey(w => w.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureSoftDeleteFilters(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes()
                         .Where(e => !e.IsOwned()
                             && typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(
                        nameof(SetSoftDeleteFilter),
                        BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { builder });
            }

            builder.Entity<ApplicationUser>()
                .HasQueryFilter(user => !user.IsDeleted);
        }

        private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder)
            where TEntity : BaseEntity
        {
            builder.Entity<TEntity>()
                .HasQueryFilter(entity => !entity.IsDeleted);
        }

        public override int SaveChanges()
        {
            ApplySoftDelete();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplySoftDelete();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ApplySoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            ApplySoftDelete();
            return base.SaveChangesAsync(
                acceptAllChangesOnSuccess,
                cancellationToken);
        }

        public async Task<bool> SoftDeleteAsync<TEntity>(
            int id,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var entity = await Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            await SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> RestoreAsync<TEntity>(
            int id,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var entity = await Set<TEntity>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity == null || !entity.IsDeleted)
                return false;

            entity.IsDeleted = false;
            entity.DeletedAt = null;

            await SaveChangesAsync(cancellationToken);

            return true;
        }

        private void ApplySoftDelete()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>()
                         .Where(e => e.State == EntityState.Deleted))
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = now;
            }

            foreach (var entry in ChangeTracker.Entries<ApplicationUser>()
                         .Where(e => e.State == EntityState.Deleted))
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = now;
                entry.Entity.LockoutEnabled = true;
                entry.Entity.LockoutEnd = DateTimeOffset.MaxValue;
            }
        }
    }
}
