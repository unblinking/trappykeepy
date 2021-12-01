using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TrappyKeepy.Domain.Models
{
    /// <summary>
    /// Table to store group membership records.
    /// </summary>
    [Keyless]
    [Table("memberships", Schema = "tk")]
    [Index(nameof(GroupId), Name = "group_membership_index")]
    [Index(nameof(UserId), Name = "user_membership_index")]
    public partial class Membership
    {
        /// <summary>
        /// UUID primary key, and foreign key to the tk.groups table.
        /// </summary>
        [Column("group_id")]
        public Guid GroupId { get; set; }
        /// <summary>
        /// UUID, and foreign key to the tk.users table.
        /// </summary>
        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; } = null!;
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
