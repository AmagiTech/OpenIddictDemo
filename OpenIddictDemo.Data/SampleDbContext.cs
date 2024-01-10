using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using OpenIddictDemo.Models;
using OpenIddictDemo.Models.Interfaces;
using System.Data;

namespace OpenIddictDemo.Data
{
    public class SampleDbContext: DbContext
    {
        #region Configuration
        private const string _systemUserId = "1BB7A9DC-1A3E-40AA-A127-548AD51F3565";
        private static IConfigurationRoot _configuration;

        public SampleDbContext()
        {

        }

        public SampleDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                _configuration = builder.Build();

                var connectionString = _configuration.GetConnectionString("SampleDb");
                optionsBuilder.UseSqlServer(connectionString);
                optionsBuilder.UseOpenIddict();
            }
        }

        private sealed class PersonalDataConverter : ValueConverter<string, string>
        {
            public PersonalDataConverter(IPersonalDataProtector protector) : base(s => protector.Protect(s), s => protector.Unprotect(s), default)
            { }
        }
        #endregion

        #region Tables
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<IdentityUserClaim<Guid>> UserClaims { get; set; } = default!;
        public DbSet<IdentityUserToken<Guid>> UserTokens { get; set; } = default!;
        public DbSet<IdentityUserLogin<Guid>> UserLogins { get; set; } = default!;
        public DbSet<Role> Roles { get; set; } = default!;
        public DbSet<IdentityUserRole<Guid>> UserRoles { get; set; } = default!;
        public DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; } = default!;

        #endregion

        #region OverrideMethods
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var maxKeyLength = 0;
            var encryptPersonalData = false;
            PersonalDataConverter? converter = null;

            builder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
                b.ToTable("Users");
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.Surname).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.Property(u => u.Email).HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);

                if (encryptPersonalData)
                {
                    converter = new PersonalDataConverter(this.GetService<IPersonalDataProtector>());
                    var personalDataProps = typeof(User).GetProperties().Where(
                                    prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
                    foreach (var p in personalDataProps)
                    {
                        if (p.PropertyType != typeof(string))
                        {
                            throw new InvalidOperationException("Can only protect strings!");
                        }
                        b.Property(typeof(string), p.Name).HasConversion(converter);
                    }
                }

                b.HasMany<IdentityUserClaim<Guid>>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
                b.HasMany<IdentityUserLogin<Guid>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
                b.HasMany<IdentityUserToken<Guid>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();
            });

            builder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

                if (maxKeyLength > 0)
                {
                    b.Property(l => l.LoginProvider).HasMaxLength(maxKeyLength);
                    b.Property(l => l.ProviderKey).HasMaxLength(maxKeyLength);
                }

                b.ToTable("UserLogins");
            });

            builder.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                if (maxKeyLength > 0)
                {
                    b.Property(t => t.LoginProvider).HasMaxLength(maxKeyLength);
                    b.Property(t => t.Name).HasMaxLength(maxKeyLength);
                }

                if (encryptPersonalData)
                {
                    var tokenProps = typeof(IdentityUserToken<Guid>).GetProperties().Where(
                                    prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
                    foreach (var p in tokenProps)
                    {
                        if (p.PropertyType != typeof(string))
                        {
                            throw new InvalidOperationException("Can only protect strings!");
                        }
                        b.Property(typeof(string), p.Name).HasConversion(converter);
                    }
                }

                b.ToTable("UserTokens");
            });
            builder.Entity<User>(b =>
            {
                b.HasMany<IdentityUserRole<Guid>>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
            });

            builder.Entity<Role>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
                b.ToTable("Roles");
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                b.HasMany<IdentityUserRole<Guid>>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany<IdentityRoleClaim<Guid>>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            builder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserRole<Guid>>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.ToTable("UserRoles");
            });
        }

        private void SaveChanagesHandler(ChangeTracker tracker)
        {
            foreach (var entry in tracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        if (entry?.Entity is ISoftDeletable)
                        {
                            ((ISoftDeletable)entry.Entity).IsDeleted = true;
                            if (entry.Entity is IAuditedModel)
                            {
                                var entity = entry.Entity as IAuditedModel;
                                if (string.IsNullOrWhiteSpace(entity.LastModifiedUserId))
                                    entity.LastModifiedUserId = _systemUserId;
                                entity.LastModifiedDate = DateTime.Now;
                            }
                            entry.State = EntityState.Modified;
                        }
                        break;
                    case EntityState.Modified:
                        if (entry.Entity is IAuditedModel)
                        {
                            var entity = entry.Entity as IAuditedModel;
                            if (string.IsNullOrWhiteSpace(entity.LastModifiedUserId))
                                entity.LastModifiedUserId = _systemUserId;
                            entity.LastModifiedDate = DateTime.Now;
                        }
                        break;
                    case EntityState.Added:
                        if (entry.Entity is IAuditedModel)
                        {
                            var entity = entry.Entity as IAuditedModel;
                            if (string.IsNullOrWhiteSpace(entity.CreatedByUserId))
                                entity.CreatedByUserId = _systemUserId;
                            entity.CreatedDate = DateTime.Now;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public override int SaveChanges()
        {
            SaveChanagesHandler(ChangeTracker);
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SaveChanagesHandler(ChangeTracker);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChanagesHandler(ChangeTracker);
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SaveChanagesHandler(ChangeTracker);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        #endregion
    }
}
