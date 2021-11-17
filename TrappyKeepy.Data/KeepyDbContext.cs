using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TrappyKeepy.Api
{
    public partial class KeepyDbContext : DbContext
    {
        public KeepyDbContext()
        {
        }

        public KeepyDbContext(DbContextOptions<KeepyDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Name=ConnectionStrings:keepydb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("en_US.utf8")
                .HasPostgresExtension("pgcrypto");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasComment("Individual user records including login credentials.");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasComment("UUID/GUID primary key of the user record.");

                entity.Property(e => e.DateActivated).HasComment("The datetime when the user record was activated for login.");

                entity.Property(e => e.DateCreated).HasComment("The datetime when the user record was created in the database.");

                entity.Property(e => e.DateLastLogin).HasComment("The datetime when the user last logged into the system successfully.");

                entity.Property(e => e.Email).HasComment("Unique email address for the user.");

                entity.Property(e => e.Name).HasComment("Unique user display name.");

                entity.Property(e => e.Password).HasComment("Encrypted user password using the pgcrypto crypt function, and gen_salt with the blowfish algorithm and iteration count of 8.");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
