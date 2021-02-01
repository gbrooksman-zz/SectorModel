using SectorModel.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace SectorModel.Server.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext() { }
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Equity> Equities { get; set; }
        public DbSet<ModelComment> ModelComments { get; set; }
        public DbSet<ModelItem> ModelItems { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Quote> Quotes { get; set; }
    }
}