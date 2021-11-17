using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrappyKeepy.Api
{
    /// <summary>
    /// Individual user records including login credentials.
    /// </summary>
    [Table("users", Schema = "tk")]
    [Index(nameof(Email), Name = "users_email_key", IsUnique = true)]
    [Index(nameof(Name), Name = "users_name_key", IsUnique = true)]
    public partial class User
    {
        /// <summary>
        /// UUID/GUID primary key of the user record.
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Unique user display name.
        /// </summary>
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        /// <summary>
        /// Encrypted user password using the pgcrypto crypt function, and gen_salt with the blowfish algorithm and iteration count of 8.
        /// </summary>
        [Column("password")]
        public string Password { get; set; } = null!;
        /// <summary>
        /// Unique email address for the user.
        /// </summary>
        [Column("email")]
        public string Email { get; set; } = null!;
        /// <summary>
        /// The datetime when the user record was created in the database.
        /// </summary>
        [Column("date_created")]
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// The datetime when the user record was activated for login.
        /// </summary>
        [Column("date_activated")]
        public DateTime? DateActivated { get; set; }
        /// <summary>
        /// The datetime when the user last logged into the system successfully.
        /// </summary>
        [Column("date_last_login")]
        public DateTime? DateLastLogin { get; set; }
    }
}
