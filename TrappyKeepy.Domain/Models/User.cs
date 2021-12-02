using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Table to store user records.
    /// </summary>
    [Table("users", Schema = "tk")]
    [Index(nameof(Role), Name = "user_role_index")]
    [Index(nameof(Email), Name = "users_email_key", IsUnique = true)]
    [Index(nameof(Name), Name = "users_name_key", IsUnique = true)]
    public partial class User
    {
        public User()
        {
            Keepers = new HashSet<Keeper>();
            Memberships = new HashSet<Membership>();
        }

        /// <summary>
        /// UUID primary key.
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Unique display name.
        /// </summary>
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        /// <summary>
        /// Salted/Hashed password using the pgcrypto crypt function, and gen_salt with the blowfish algorithm and iteration count of 8.
        /// </summary>
        [Column("password")]
        public string Password { get; set; } = null!;
        /// <summary>
        /// Unique email address.
        /// </summary>
        [Column("email")]
        public string Email { get; set; } = null!;
        /// <summary>
        /// Security level role.
        /// </summary>
        [Column("role")]
        public string Role { get; set; } = null!;
        /// <summary>
        /// Datetime the user was created in the database.
        /// </summary>
        [Column("date_created")]
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// Datetime the user was activated for login.
        /// </summary>
        [Column("date_activated")]
        public DateTime DateActivated { get; set; }
        /// <summary>
        /// Datetime the user last logged into the system successfully.
        /// </summary>
        [Column("date_last_login")]
        public DateTime? DateLastLogin { get; set; }

        [InverseProperty(nameof(Keeper.UserPostedNavigation))]
        public virtual ICollection<Keeper> Keepers { get; set; }
        [InverseProperty(nameof(Membership.User))]
        public virtual ICollection<Membership> Memberships { get; set; }
    }
}
