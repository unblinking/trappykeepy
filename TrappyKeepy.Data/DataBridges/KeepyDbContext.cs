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

        public virtual DbSet<Filedata> Filedatas { get; set; } = null!;
        public virtual DbSet<Group> Groups { get; set; } = null!;
        public virtual DbSet<Keeper> Keepers { get; set; } = null!;
        public virtual DbSet<Membership> Memberships { get; set; } = null!;
        public virtual DbSet<Permit> Permits { get; set; } = null!;
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

            modelBuilder.Entity<Filedata>(entity =>
            {
                entity.HasKey(e => e.KeeperId)
                    .HasName("filedatas_pkey");

                entity.HasComment("Table to store keeper/document binary data records.");

                entity.Property(e => e.KeeperId)
                    .ValueGeneratedNever()
                    .HasComment("UUID primary key, and foreign key to the tk.keepers table.");

                entity.Property(e => e.BinaryData).HasComment("Bytea binary string of the actual keeper/document uploaded.");

                entity.HasOne(d => d.Keeper)
                    .WithOne(p => p.Filedata)
                    .HasForeignKey<Filedata>(d => d.KeeperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_keeper_of_filedata");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasComment("Table to store group records.");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasComment("UUID primary key.");

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("Datetime the group was created in the database.");

                entity.Property(e => e.Description).HasComment("Description of the group.");

                entity.Property(e => e.Name).HasComment("Unique group name.");
            });

            modelBuilder.Entity<Keeper>(entity =>
            {
                entity.HasComment("Table to store keeper/document metadata records.");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasComment("UUID primary key.");

                entity.Property(e => e.Category).HasComment("Category of the document.");

                entity.Property(e => e.ContentType).HasComment("The media type of the resource.");

                entity.Property(e => e.DatePosted)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("Datetime the document was created in the database.");

                entity.Property(e => e.Description).HasComment("Description of the document.");

                entity.Property(e => e.Filename).HasComment("Unique document filename.");

                entity.Property(e => e.UserPosted).HasComment("User id associated with creating the document in the database.");

                entity.HasOne(d => d.UserPostedNavigation)
                    .WithMany(p => p.Keepers)
                    .HasForeignKey(d => d.UserPosted)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_posted_keeper");
            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.HasComment("Table to store group membership records.");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasComment("UUID primary key");

                entity.Property(e => e.GroupId).HasComment("UUID, and foreign key to the tk.groups table.");

                entity.Property(e => e.UserId).HasComment("UUID, and foreign key to the tk.users table.");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_group_of_memberships");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_of_memberships");
            });

            modelBuilder.Entity<Permit>(entity =>
            {
                entity.HasComment("Table to store permit records.");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasComment("UUID primary key.");

                entity.Property(e => e.GroupId).HasComment("UUID from the tk.groups table.");

                entity.Property(e => e.KeeperId).HasComment("UUID from the tk.keepers table.");

                entity.Property(e => e.UserId).HasComment("UUID from the tk.users table.");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasComment("Table to store user records.");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasComment("UUID primary key.");

                entity.Property(e => e.DateActivated).HasComment("Datetime the user was activated for login.");

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasComment("Datetime the user was created in the database.");

                entity.Property(e => e.DateLastLogin).HasComment("Datetime the user last logged into the system successfully.");

                entity.Property(e => e.Email).HasComment("Unique email address.");

                entity.Property(e => e.Name).HasComment("Unique display name.");

                entity.Property(e => e.Password).HasComment("Salted/Hashed password using the pgcrypto crypt function, and gen_salt with the blowfish algorithm and iteration count of 8.");

                entity.Property(e => e.Role)
                    .HasDefaultValueSql("'basic'::text")
                    .HasComment("Security level role.");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
