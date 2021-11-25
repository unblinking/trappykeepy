using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Table to store keeper/document metadata records.
    /// </summary>
    [Table("keepers", Schema = "tk")]
    [Index(nameof(Filename), Name = "keepers_filename_key", IsUnique = true)]
    public partial class Keeper
    {
        /// <summary>
        /// UUID primary key.
        /// </summary>
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Unique document filename.
        /// </summary>
        [Column("filename")]
        public string Filename { get; set; } = null!;
        /// <summary>
        /// Description of the document.
        /// </summary>
        [Column("description")]
        public string? Description { get; set; }
        /// <summary>
        /// Category of the document.
        /// </summary>
        [Column("category")]
        public string? Category { get; set; }
        /// <summary>
        /// Datetime the document was created in the database.
        /// </summary>
        [Column("date_posted")]
        public DateTime DatePosted { get; set; }
        /// <summary>
        /// User id associated with creating the document in the database.
        /// </summary>
        [Column("user_posted")]
        public Guid UserPosted { get; set; }

        [ForeignKey(nameof(UserPosted))]
        [InverseProperty(nameof(User.Keepers))]
        public virtual User UserPostedNavigation { get; set; } = null!;
        [InverseProperty("Keeper")]
        public virtual Filebytea Filebytea { get; set; } = null!;
    }
}
