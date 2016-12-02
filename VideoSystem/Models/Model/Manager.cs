using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VideoSystem.Models
{
    public class Manager
    {
        [Key]
        public int ManagerID { get; set; }
        [Required]
        public string ManagerAccount { get; set; }
        [Required]
        public string ManagerPassword { get; set; }
        [Required]
        public string ManagerEmail { get; set; }
        [Required]
        public string ManagerPhone { get; set; }
        [Required]
        public int ManagerRange { get; set; }
        [Required]
        public int ManagerStatus { get; set; }

        public ICollection<ManagerLimit> ManagerLimits { get; set; }
    }

    public class ManagerLimit
    {
        [Key]
        public int LimitID { get; set; }
        [Required]
        public string LimitAction { get; set; }
        [Required]
        public int IsAllowed { get; set; }
        [Required]
        public int ManagerID { get; set; }

        public virtual Manager Manager { get; set; }
    }

 
}