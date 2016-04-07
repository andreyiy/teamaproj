using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModels.Models
{
   public class StudentUploadPath
    {
       public int TeacherId { get; set; }
       public string TeacherName { get; set; }
       public int StudentId { get; set; }
       public int UploadID { get; set; }
       public string HomeWorkName { get; set; }


    }

}
