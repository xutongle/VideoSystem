using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace VideoSystem.Models
{
    public class VideoSystemContext : DbContext
    {
        public VideoSystemContext()
            : base("name=VideoSystemData")
        {

        }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Code> Codes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Suggest> Suggests { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<ManagerLimit> ManagerLimits { get; set; }
    }
}