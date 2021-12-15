using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Table to store keeper/document binary data records.
    /// </summary>
    [Table("filedatas", Schema = "tk")]
    public partial class Filedata
    {
        /// <summary>
        /// UUID primary key, and foreign key to the tk.keepers table.
        /// </summary>
        [Key]
        [Column("keeper_id")]
        public Guid KeeperId { get; set; }
        /// <summary>
        /// Bytea binary string of the actual keeper/document uploaded.
        /// </summary>
        [Column("binary_data")]
        public byte[] BinaryData { get; set; } = null!;

        [ForeignKey(nameof(KeeperId))]
        [InverseProperty("Filedata")]
        public virtual Keeper Keeper { get; set; } = null!;
    }
}
