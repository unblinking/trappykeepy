using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Data
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

        public virtual DbSet<Filebytea> Filebyteas { get; set; } = null!;
        public virtual DbSet<Keeper> Keepers { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Name=ConnectionStrings:TKDB_CONN_STRING");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("en_US.utf8")
                .HasPostgresExtension("pgcrypto");

            modelBuilder.Entity<Filebytea>(entity =>
            {
                entity.HasKey(e => e.KeeperId)
                    .HasName("filebyteas_pkey");

                entity.HasComment("Table to store keeper/document file/bytea/blob records.");

                entity.Property(e => e.KeeperId)
                    .ValueGeneratedNever()
                    .HasComment("UUID primary key, and foreign key to the tk.keepers table.");

                entity.Property(e => e.Filebytea1).HasComment("Bytea blob of the actual keeper/document uploaded.");

                entity.HasOne(d => d.Keeper)
                    .WithOne(p => p.Filebytea)
                    .HasForeignKey<Filebytea>(d => d.KeeperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_keeper_of_filebytea");
            });

            modelBuilder.Entity<Keeper>(entity =>
            {
                entity.HasComment("Table to store keeper/document metadata records.");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasComment("UUID primary key.");

                entity.Property(e => e.Category).HasComment("Category of the document.");

                entity.Property(e => e.DatePosted).HasComment("Datetime the document was created in the database.");

                entity.Property(e => e.Description).HasComment("Description of the document.");

                entity.Property(e => e.Filename).HasComment("Unique document filename.");

                entity.Property(e => e.UserPosted).HasComment("User id associated with creating the document in the database.");

                entity.HasOne(d => d.UserPostedNavigation)
                    .WithMany(p => p.Keepers)
                    .HasForeignKey(d => d.UserPosted)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_posted_keeper");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasComment("Table to store user records.");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasComment("UUID primary key.");

                entity.Property(e => e.DateActivated).HasComment("Datetime the user was activated for login.");

                entity.Property(e => e.DateCreated).HasComment("Datetime the user was created in the database.");

                entity.Property(e => e.DateLastLogin).HasComment("Datetime the user last logged into the system successfully.");

                entity.Property(e => e.Email).HasComment("Unique email address.");

                entity.Property(e => e.Name).HasComment("Unique display name.");

                entity.Property(e => e.Password).HasComment("Salted/Hashed password using the pgcrypto crypt function, and gen_salt with the blowfish algorithm and iteration count of 8.");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
