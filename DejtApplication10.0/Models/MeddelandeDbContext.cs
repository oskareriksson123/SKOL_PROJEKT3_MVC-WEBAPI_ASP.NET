using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DejtApplication10._0.Models
{
    public class MeddelandeDbContext : DbContext
    {

        public DbSet<MeddelandeModel> meddelande { get; set; }

        public DbSet<AnvändareModel> AnvändareModel { get; set; }

        public MeddelandeDbContext() : base("Alla-Meddelanden")
        {


        }

        
    }
}