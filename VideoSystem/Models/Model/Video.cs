using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace VideoSystem.Models
{
 
    public class Video
    {
        [Key]
        public int VideoID { get; set; }
        [Required]
        public string VideoName { get; set; }
        [Required]
        public DateTime UploadTime { get; set; }
        [Required]
        public int CodeCounts { get; set; }
        [Required]
        public int CodeUsed { get; set; }
        [Required]
        public int CodeNotUsed { get; set; }
        [Required]
        public string VideoLocal { get; set; }
        [Required]
        public string VideoImageLocal { get; set; }
        [Required]
        public string VideoMD5 { get; set; }

        public ICollection<Code> Code { get; set; }
    }

    public class Code
    {
        [Key]
        public int CodeID { get; set; }
        [Required]
        public string CodeValue { get; set; }
        [Required]
        public int CodeStatus { get; set; }

        
        public int UserID { get; set; }
        [Required]
        public int VideoID { get; set; }


        public virtual Video Video { get; set; }
    }
}