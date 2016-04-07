using ServiceHelpers.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHelpers
{
    public class FileSystemManager
    {
        public ExplorerModel GetExplorerModel(string path, Uri requestURL)
        {
            string realPath;
            realPath = path;
            // or realPath = "FullPath of the folder on server" 

            ExplorerModel explorerModel = new ExplorerModel();

            if (System.IO.File.Exists(realPath))
            {
                //http://stackoverflow.com/questions/1176022/unknown-file-type-mime
                explorerModel.isFile = true;
                return explorerModel;
            }
            else if (System.IO.Directory.Exists(realPath))
            {

                List<FileModel> fileListModel = new List<FileModel>();

                List<DirModel> dirListModel = new List<DirModel>();

                IEnumerable<string> dirList = Directory.EnumerateDirectories(realPath);
                foreach (string dir in dirList)
                {
                    DirectoryInfo d = new DirectoryInfo(dir);

                    DirModel dirModel = new DirModel();

                    dirModel.DirName = Path.GetFileName(dir);
                    dirModel.DirAccessed = d.LastAccessTime;

                    dirListModel.Add(dirModel);
                }

                IEnumerable<string> fileList = Directory.EnumerateFiles(realPath);
                foreach (string file in fileList)
                {
                    FileInfo f = new FileInfo(file);

                    FileModel fileModel = new FileModel();

                    if (f.Extension.ToLower() != "php" && f.Extension.ToLower() != "aspx"
                        && f.Extension.ToLower() != "asp")
                    {
                        fileModel.FileName = Path.GetFileName(file);
                        fileModel.FileAccessed = f.LastAccessTime;
                        fileModel.FileSizeText = (f.Length < 1024) ? f.Length.ToString() + " B" : f.Length / 1024 + " KB";

                        fileListModel.Add(fileModel);
                    }
                }

                explorerModel = new ExplorerModel(dirListModel, fileListModel);
                explorerModel.isFile = false;

                return explorerModel;
            }
            else return new ExplorerModel();
        }
    }
}
