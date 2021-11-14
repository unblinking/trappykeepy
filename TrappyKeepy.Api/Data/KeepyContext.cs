using Microsoft.EntityFrameworkCore;
using TrappyKeepy.Api.Models;

namespace TrappyKeepy.Api.Data
{
    public class KeepyContext : DbContext
    {
        public KeepyContext(DbContextOptions<KeepyContext> options) : base(options){

        }

        public DbSet<Keeper> Keepers { get; set; } = null!;
    }
}
