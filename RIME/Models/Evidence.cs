using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIME.Models
{
    public class Evidence
    {
        public int EvidenceId { get; set; }
        public string UserName { get; set; }
        public string EvidencePic { get; set; }
        public string EvidencePath { get; set; }
        public string EvidenceLocation { get; set; }
        public string Title{ get; set; }
        [DataType(DataType.MultilineText)]
        public string Prolog{ get; set; }
        [DataType(DataType.MultilineText)]
        public string Content{ get; set; }
        public DateTime Date { get; set; }
        public int Likes { get; set; }
        [DataType(DataType.MultilineText)]
        public string Quote { get; set; }
        public virtual ICollection<EvidenceComment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public int RelatedEvidance { get; set; }
    }
}