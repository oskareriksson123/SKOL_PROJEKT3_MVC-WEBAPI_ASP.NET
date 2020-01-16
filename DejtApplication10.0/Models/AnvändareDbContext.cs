using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DejtApplication10._0.Models
{
    public class AnvändareDbContext : DbContext
    {
        public DbSet<AnvändareModel> användare { get; set; }
        public DbSet<MeddelandeModel> meddelanden { get; set; }

        public DbSet<vänn> vänner { get; set; }
        public DbSet<VännFörFrågningar> Vännförfrågnignar { get; set; }
        public AnvändareDbContext() : base("Alla-Användare")
        {
          
            // skapar databas som heter "alla användare"
        }
    }
}