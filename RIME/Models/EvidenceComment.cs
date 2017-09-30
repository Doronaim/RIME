using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIME.Models
{
    public class EvidenceComment
    {
        public int EvidenceCommentId{ get; set; }
        public int EvidenceId{ get; set; }
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public DateTime Date{ get; set; }
        [DataType(DataType.MultilineText)]
        public string Content{ get; set; }
        public virtual ICollection<SubComment> SubComments { get; set; }
    }
}