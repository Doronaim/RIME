using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RIME.Models
{
    public class RimeContext : DbContext
    {
        public DbSet<Evidence> Evidences{ get; set; }
        public DbSet<EvidenceComment> EvidenceComments{ get; set; }
        public DbSet<SubComment> SubComments { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}