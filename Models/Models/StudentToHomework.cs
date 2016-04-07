using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModels.Models
{
   public class StudentToHomework
    {
        public int StudentUserID { get; set; }
        public int HomeworkID { get; set; }
        public int UploadID { get; set; }
        public DateTime UploadDate { get; set; }
        public string Comment{ get; set; }
        public string Status { get; set; }
        public int Grade { get; set; }
        public string StudentName { get; set; }
    }
}
