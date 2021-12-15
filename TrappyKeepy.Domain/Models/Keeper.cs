using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Table to store keeper/document metadata records.
    /// </summary>
    [Table("keepers", Schema = "tk")]
    [Index(nameof(Filename), Name = "keepers_filename_key", IsUnique = true)]
    [Index(nameof(UserPosted), Name = "user_posted_index")]
    public partial class Keeper
    {
        public Keeper()
        {
            Permits = new HashSet<Permit>();
        }

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
        /// The media type of the resource.
        /// </summary>
        [Column("content_type")]
        public string ContentType { get; set; } = null!;
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
        public virtual Filedata Filedata { get; set; } = null!;
        [InverseProperty(nameof(Permit.Keeper))]
        public virtual ICollection<Permit> Permits { get; set; }
    }
}
