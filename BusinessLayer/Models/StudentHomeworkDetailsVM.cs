using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class StudentHomeworkDetailsVM
    {
        [Required]
        public int HomeworkId { get; set; }
        public string HomeWorkName { get; set; }
        public string TeacherName { get; set; }
        [Required]
        public int TeacherId { get; set; }
        public int StudentGrade { get; set; }
        public DateTime UploadDate { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public int UploadId { get; set; }
    }
}
