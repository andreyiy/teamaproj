using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using TeamA.Models;
//using AccessModels.Models;
using System.Configuration;
using BusinessLayer.Models;
using TeamA.Authorize;


namespace TeamA.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private AdminService adminService = new AdminService();
        private UserService userService = new UserService();
        private StudentService studentService = new StudentService();

        public ActionResult Index()
        {
            return View();
        }       
        
        public ActionResult CreateTeacher()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult CreateTeacher(TeacherVM tcr)
        {
            string str="";
            try { 
            adminService.AddTeachersFromAdmin(tcr.Username,tcr.Email, Server.MapPath(ConfigurationManager.AppSettings["BasePath"]));
            ViewBag.Success = "";            return View();
                }
            catch(SqlException ex)
            {
               
                str = "Source:" + ex.Source;
                str += "\n" + "Number:" + ex.Number.ToString();
                str += "\n" + "Message:" + ex.Message;
                str += "\n" + "Class:" + ex.Class.ToString();
                str += "\n" + "Procedure:" + ex.Procedure.ToString();
                str += "\n" + "Line Number:" + ex.LineNumber.ToString();
                str += "\n" + "Server:" + ex.Server.ToString();
               
            }
                
            finally
            {


               Response.Write(str);
              
            }
            return View("Error", (object)"Invalid input for Teacher");


        }

        public ActionResult ViewAllStudents()
        {
            var listStudents = userService.GetAllStudents().ToList();
            var listTeachers = userService.GetAllTeachers();
            AccountTeacherListVm vm = new AccountTeacherListVm();
            vm.AccountList = new List<AccountVM>();
            vm.TeacherList = new List<string>() { "" };
            foreach (var item in listStudents)
            {
                vm.AccountList.Add(new AccountVM{
                    UserName=item.Username,
                    Email=item.Email,
                    TeacherName= studentService.GetTeacherBelongingToStudent(item.Username).Item3,
                    IsConfirmed = item.IsConfirmed
                });
            }
            foreach (var item in listTeachers)
            {
                vm.TeacherList.Add(item.Username);
            }
            vm.TeacherList.Insert(0, null);       
            return View(vm);
        }

        public PartialViewResult ViewAllStudentsBack()
        {
            var listStudents = userService.GetAllStudents().ToList();
            var listTeachers = userService.GetAllTeachers();
            AccountTeacherListVm vm = new AccountTeacherListVm();
            vm.AccountList = new List<AccountVM>();
            vm.TeacherList = new List<string>();
            foreach (var item in listStudents)
            {
                vm.AccountList.Add(new AccountVM
                {
                    UserName = item.Username,
                    Email = item.Email,
                    TeacherName = studentService.GetTeacherBelongingToStudent(item.Username).Item3,
                    IsConfirmed = item.IsConfirmed
                });
            }
            foreach (var item in listTeachers)
            {
                vm.TeacherList.Add(item.Username);
            }

            return PartialView("_ViewAllStudents", vm);
        }

        [HttpPost]
        public PartialViewResult ViewAllUnassignedStudents()
        {
            var listStudents = userService.GetAllUnassignedStudents().ToList();
            var listTeachers = userService.GetAllTeachers();
            AccountTeacherListVm vm = new AccountTeacherListVm();
            vm.AccountList = new List<AccountVM>();
            vm.TeacherList = new List<string>();
            foreach (var item in listStudents)
            {
                vm.AccountList.Add(new AccountVM
                {
                    UserName = item.Username,
                    Email = item.Email,
                    TeacherName = studentService.GetTeacherBelongingToStudent(item.Username).Item3,
                    IsConfirmed = item.IsConfirmed
                });
            }
            foreach (var item in listTeachers)
            {
                vm.TeacherList.Add(item.Username);
            }
            return PartialView("_ViewAllStudents", vm);
        }

        public ActionResult ViewAllTeachers()
        {
            var lista = userService.GetAllTeachers().ToList();
            List<TeacherVM> VMList = new List<TeacherVM>();
            foreach (var item in lista)
            {
                VMList.Add(new TeacherVM()
                    {
                        Username = item.Username,
                        Email = item.Email
                        //IsConfirmed=item.IsConfirmed
                    });
            }
            return View(VMList.ToList());
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendMailWithPassword(string username)
        {
            adminService.ResetPasswordSendMail(username);
            return new EmptyResult();
        }

        public ActionResult UpdateTeacherOfStudent(string studentName, string teacherName)
        {
            var getStudentId = userService.GetUser(studentName);
             try
                {
                    adminService.InsertTeacherToStudent(teacherName, getStudentId.Item1);
                }
                catch (Exception)
                {

                    return View("Error",(object)"Try again!");
                }
		        
	                  
            return new EmptyResult();
        }
       
    }
}



    


