using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModels.Models
{
    public class Homework
    {
        public int HomeworkID { get; set; }
        public string HomeworkName { get; set; }
        public int TeacherUserID { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
    }
}
