using BusinessLayer.Models;
using ServiceHelpers;
using ServiceHelpers.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.IO;
using DAL.Repository;

namespace BusinessLayer
{
    public class FileSystemService
    {
        private FileSystemManager manager = new FileSystemManager();
        private HomeworkRepository homeworkRepository = new HomeworkRepository();

        public ExplorerModelVM GetExplorerModel(string path, Uri requestURL, int? homeworkID = null)
        {
            ExplorerModel model = manager.GetExplorerModel(path, requestURL);

            ExplorerModelVM vm = new ExplorerModelVM();
            vm.dirModelList = new List<Models.DirModel>();
            foreach (ServiceHelpers.Model.DirModel item in model.dirModelList)
                if (homeworkID != null)
                    vm.dirModelList.Add(
                        new BusinessLayer.Models.DirModel()
                        {
                            DirAccessed = item.DirAccessed,
                            DirName = item.DirName,
                            Deadline = homeworkRepository.GetHomework((int)homeworkID).Deadline
                        });
                else
                    vm.dirModelList.Add(
                        new BusinessLayer.Models.DirModel()
                        {
                            DirAccessed = item.DirAccessed,
                            DirName = item.DirName,
                            Deadline = homeworkRepository.GetHomework(Convert.ToInt32(item.DirName.Split('_')[1])).Deadline
                        });

            vm.fileModelList = new List<Models.FileModel>();
            foreach (ServiceHelpers.Model.FileModel item in model.fileModelList)
                vm.fileModelList.Add(new BusinessLayer.Models.FileModel()
                    {
                        FileAccessed = item.FileAccessed,
                        FileName = item.FileName,
                        FileSizeText = item.FileSizeText
                    });

            vm.isFile = model.isFile;

            return vm;
        }

        public string GetFileText(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File at " + path + " not found!");
            else
            {
                string fileText = File.ReadAllText(path);
                return fileText;
            }
        }

        public void SaveFile(string path, System.Web.HttpPostedFileBase homeworkFile, int uploadID)
        {
            homeworkFile.SaveAs(path + "/" + homeworkFile.FileName + "_" + Convert.ToString(uploadID));
        }

        //public ExplorerModelVM GetExplorerModel(string path, Uri requestURL)
        //{
        //    ExplorerModel model = manager.GetExplorerModel(path, requestURL);

        //    ExplorerModelVM vm = new ExplorerModelVM();
        //    vm.dirModelList = new List<Models.DirModel>();
        //    foreach (ServiceHelpers.Model.DirModel item in model.dirModelList)
        //        vm.dirModelList.Add(
        //            new BusinessLayer.Models.DirModel()
        //            {
        //                DirAccessed = item.DirAccessed,
        //                DirName = item.DirName,
        //                Deadline = homeworkRepository.GetHomework(Convert.ToInt32(item.DirName.Split('_')[1])).Deadline
        //            });

        //    vm.fileModelList = new List<Models.FileModel>();
        //    foreach (ServiceHelpers.Model.FileModel item in model.fileModelList)
        //        vm.fileModelList.Add(new BusinessLayer.Models.FileModel()
        //        {
        //            FileAccessed = item.FileAccessed,
        //            FileName = item.FileName,
        //            FileSizeText = item.FileSizeText
        //        });

        //    vm.isFile = model.isFile;

        //    return vm;
        //}
    }
}
