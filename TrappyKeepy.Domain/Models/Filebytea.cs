using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Table to store keeper/document file/bytea/blob records.
    /// </summary>
    [Table("filebyteas", Schema = "tk")]
    public partial class Filebytea
    {
        /// <summary>
        /// UUID primary key, and foreign key to the tk.keepers table.
        /// </summary>
        [Key]
        [Column("keeper_id")]
        public Guid KeeperId { get; set; }
        /// <summary>
        /// Bytea blob of the actual keeper/document uploaded.
        /// </summary>
        [Column("filebytea")]
        public byte[] Filebytea1 { get; set; } = null!;

        [ForeignKey(nameof(KeeperId))]
        [InverseProperty("Filebytea")]
        public virtual Keeper Keeper { get; set; } = null!;
    }
}
