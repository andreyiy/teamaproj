using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccessModels.Models;

namespace BusinessLayer.Models
{
    public class StudentHomeworkBroswerDetailsVM
    {
        public StudentHomeworkDetailsVM Details { get; set; }
        public ExplorerModelVM FolderStructure { get; set; }
    }
}
