using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIME.Models
{
    public class Tag
  
    {
        public int TagId { get; set; }
        public int EvidenceId { get; set; }
        public string Categoty { get; set; }
        public string TagName { get; set; }
    }
}