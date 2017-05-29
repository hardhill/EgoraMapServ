
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EgoraMap.Models
{
    public class DbEgoraContext:DbContext
    {
        
        public DbSet<Route> Routes { get; set; }
        public DbSet<Photo> Photos { get; set; }

       
    }
}
