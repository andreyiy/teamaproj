using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BusinessLayer.Models
{
    public class HomeworkVM
    {
        public int HomeworkID { get; set; }

        [Required]
        [DisplayName("Homework name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Homework description")]
        public string Description { get; set; }
        
        [Required]
        [MyDate]
        [DisplayName("Deadline")]
        public DateTime Deadline { get; set; }
        public int TeacherID { get; set; }
        
    }
}