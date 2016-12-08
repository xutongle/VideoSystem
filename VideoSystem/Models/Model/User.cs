using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoSystem.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required]
        public string UserAccount { get; set; }
        [Required]
        public string UserPassword { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserPhone { get; set; }
        [Required]
        public string UserBrowser1 { get; set; }
        [Required]
        public string UserBrowser2 { get; set; }
        [Required]
        public string UserBrowser3 { get; set; }
        [NotMapped]
        public string UserConfirmPassword { get; set; }

        public ICollection<Suggest> Suggests { get; set; }
    }

    public class Suggest
    {
        [Key]
        public int SuggestID { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        [Required]
        public string SuggestText { get; set; }
        [Required]
        public int UserID { get; set; }

        public virtual User User { get; set; }
    }
}