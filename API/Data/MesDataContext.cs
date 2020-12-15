using Bottom_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bottom_API.Data
{
    public class MesDataContext : DbContext
    {
        public MesDataContext(DbContextOptions<MesDataContext> options) : base(options) { }
        public DbSet<MES_MO> MES_MO {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<MES_MO>().HasKey(x => new {x.Factory_ID, x.Cycle_No});
        }
    }
}