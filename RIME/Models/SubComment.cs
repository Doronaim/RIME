using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIME.Models
{
    public class SubComment
    {
        public int SubCommentId{ get; set; }
        public int EvidenceCommentId { get; set; }
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public DateTime Date { get; set; }
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}