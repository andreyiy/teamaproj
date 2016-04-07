using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessLayer.Models
{
    public class DirModel
    {
        public string DirName { get; set; }
        public DateTime DirAccessed { get; set; }
        public DateTime Deadline { get; set; }
    }

    public class FileModel
    {
        public string FileName { get; set; }
        public string FileSizeText { get; set; }
        public DateTime FileAccessed { get; set; }
    }

    public class ExplorerModelVM
    {
        public List<DirModel> dirModelList;
        public List<FileModel> fileModelList;
        public bool isFile;

        public ExplorerModelVM()
        {
            dirModelList = new List<DirModel>();
            fileModelList = new List<FileModel>();
            isFile = false;
        }

        public ExplorerModelVM(List<DirModel> _dirModelList, List<FileModel> _fileModelList)
        {
            dirModelList = _dirModelList;
            fileModelList = _fileModelList;
            isFile = false;
        }
    }
}