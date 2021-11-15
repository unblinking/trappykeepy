using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrappyKeepy.Api.Models
{
    public class Keeper
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime? DatePosted { get; set; }
    }
}