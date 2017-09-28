using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace RIME.Models
{
    public class User
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string UserName { get; set; }
        public string UserHash{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public DateTime DOB { get; set; }
        public string Summary { get; set; }
        public string Photo { get; set; }
        

    }
}