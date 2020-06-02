using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SectorModel.Shared.Entities;

namespace SectorModel.Server.Managers
{
    public class SectorModelContext : DbContext
    {
        public DbSet<Equity> Equities { get; set; }

        public DbSet<EquityGroup> EquityGroups { get; set; }

        public DbSet<EquityGroupItem> EquityGroupItems { get; set; }

        public DbSet<Quote> Quotes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserModel> UserModels { get; set; }

        public DbSet<UserModelComment> UserModelComments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connString = @" Data Source=(LocalDB)\MSSQLLocalDB;
                            AttachDbFilename=C:\Users\42505\source\repos\SectorModel\Server\data\sectormodel.mdf;
                            Integrated Security=True;
                            Connect Timeout=30";

            optionsBuilder.UseSqlServer(connString);
        }
    }
}
