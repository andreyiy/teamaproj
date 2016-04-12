using AccessModels.Models;
//using AccessModels.Models;
using BusinessLayer;
using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamA.Attributes;
using TeamA.Authorize;

namespace TeamA.Controllers
{

    [CookieFilter]
    public class StudentController : Controller
    {
        private StudentService _studentService = new StudentService();
        private FileSystemService _fileSystemService = new FileSystemService();
        private HomeworkService _homeworkService = new HomeworkService();

        public ActionResult Index()
        {
            //var studList = _studentService.GetStudentTeacher(teacherName);
            return View();
        }

        public ActionResult GetStudentPendingHomework(string userName)
        {
            var studentPendingHomework = _studentService.GetStudentPendingHomework((string)Session["SessionUser"]);

            return View(studentPendingHomework);
        }

        public ActionResult GetStudentCompletedHomework(string userName)
        {
            var studentCompletedHomework = _studentService.GetStudentCompletedHomework((string)Session["SessionUser"]);
            return View(studentCompletedHomework);
        }

        public ActionResult InsertStudentToHomework(string userName,int homeworkID,string fileName,string basePath)
        {
            _studentService.InsertStudentToHomework(userName, homeworkID, fileName, basePath);
            return View();
        }

        //[HttpPost]
        //public ActionResult ViewHomeworkDetails(StudentHomeworkDetailsVM model, string teacherFolder, string homeworkFolder, string studentFolder, string path)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (teacherFolder == null)
        //            return View("Error", "You do not have the rights to access root folder!");
        //        else
        //        {
        //            string teacherName = teacherFolder.Split('_')[0];
        //            var teacherList = _studentService.GetTeacherBelongingToStudent((string)Session["SessionUser"]);
        //            bool found = false;
        //            if ((teacherList != null) && (teacherList.Item3 == teacherName))
        //                found = true;
        //            if ((Request.QueryString["studentFolder"] != (string)Session["SessionUser"] + "_" + Session["SessionUserId"]) || (studentFolder == null)
        //                || (!found) || (homeworkFolder == null))
        //                return View("Error", "You do not have the rights to access root folder!");
        //            string realPath = ConfigurationManager.AppSettings["BasePath"] + teacherFolder + "/" + homeworkFolder + "/" + studentFolder + "/";
        //            if (path != null)
        //                realPath += path;
        //            StudentHomeworkBroswerDetailsVM vm = new StudentHomeworkBroswerDetailsVM();
        //            vm.Details = model;
        //            //string realPath = ConfigurationManager.AppSettings["BasePath"] +
        //            //    "/" + (string)Session["SessionUser"] + "_" + Session["SessionUserId"] + "/");
        //            vm.FolderStructure = _fileSystemService.GetExplorerModel(realPath, Request.Url);
        //            if (!vm.FolderStructure.isFile)
        //                return View(vm);
        //            else
        //            {
        //                string fileText = "";
        //                try
        //                {
        //                    fileText = _fileSystemService.GetFileText(realPath);
        //                }
        //                catch (Exception)
        //                {
        //                    return View("Error", (object)"An error ocurred");
        //                }
        //                return View("ViewStudentHomework", (object)fileText);
        //            }
        //        }
        //    }
        //    else return View("Error", "Invalid data input!");
        //}

        public ActionResult ViewHomeworkDetails(int id, string teacherFolder, string homeworkFolder, string studentFolder, string path)
        {
            if (teacherFolder == null)
                return View("Error", (object)"You do not have the rights to access root folder!");
            else
            {
                string teacherName = teacherFolder.Split('_')[0];
                var teacherList = _studentService.GetTeacherBelongingToStudent((string)Session["SessionUser"]);
                bool found = false;
                if ((teacherList != null) && (teacherList.Item3 == teacherName))
                     found = true;
                if ((Request.QueryString["studentFolder"] != (string)Session["SessionUser"] + "_" + Session["SessionUserId"]) || (studentFolder == null)
                    || (!found) || (homeworkFolder == null))
                    return View("Error", (object)"You do not have the rights to access this folder!");
                string realPath = ConfigurationManager.AppSettings["BasePath"] + teacherFolder + "/" + homeworkFolder + "/" + studentFolder + "/";
                if (path != null)
                    realPath += path;
                StudentHomeworkBroswerDetailsVM vm = new StudentHomeworkBroswerDetailsVM();
                vm.Details = _homeworkService.GetStudentHomeworkDetails((int)Session["SessionUserId"], id);

                vm.FolderStructure = _fileSystemService.GetExplorerModel(Server.MapPath(realPath), Request.Url, id);
                ViewBag.HomeworkID = id;
                if (!vm.FolderStructure.isFile)
                    return View(vm);
                else
                {
                    string fileText = "";
                    try
                    {
                        fileText =_fileSystemService.GetFileText(Server.MapPath(realPath));
                    }
                    catch (Exception)
                    {
                        return View("Error", (object)"An error ocurred");
                    }
                    return View("ViewHomeworkFile", (object)fileText);
                }
            }
        }

        [HttpPost]
        public ActionResult UploadHomework(HttpPostedFileBase homeworkFile, int id)
        {
            if ((homeworkFile != null) && (homeworkFile.ContentLength < 0))
                return View("Error", (object)"Empty file!");
            else
            {
                try
                {
                    string s = ConfigurationManager.AppSettings["BasePath"] + Request["teacherFolder"] + "/" + Request["homeworkFolder"] + "/" + Request["studentFolder"];
                    if (Request["path"] != null)
                        s += "/" + Request["path"];
                    int uploadID = _studentService.InsertStudentToHomework((string)Session["SessionUser"], id, homeworkFile.FileName, Server.MapPath(s));
                    _fileSystemService.SaveFile(Server.MapPath(s), homeworkFile, uploadID);
                    return Redirect(Request.UrlReferrer.AbsoluteUri);
                }
                catch (Exception e)
                {
                    return View("Error", (object)"Something failed with the upload...");
                }
            }
        }

        //TODO view
        public ActionResult GetCompletedHomeworkUpload(string userName, int homeworkId)
        {

            var completedHomeworkUpload = _studentService.GetCompletedHomeworkUpload(userName, homeworkId);
            return View(completedHomeworkUpload);
        }

        //De implementat in View
        //public ActionResult ViewStudentPendingHomeworkUploads(string userName, int homeworkId) 
        //{
        //    List<StudentHomeworkDetails> studentPendingHomeworkUpload = _studentService.GetPendingHomeworkUpload(userName, homeworkId);

        //    return View(studentPendingHomeworkUpload);
        //}
        [CustomAuthorize(Roles = "Student")]
        public ActionResult SRChat()
        {
            return View();
        }
    }
}
