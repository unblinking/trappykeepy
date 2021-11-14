using Microsoft.EntityFrameworkCore;

namespace TrappyKeepy.Api.Models
{
    public class KeepyContext : DbContext
    {
        public KeepyContext(DbContextOptions<KeepyContext> options) : base(options){

        }

        public DbSet<Keeper> Keepers { get; set; } = null!;
    }
}
