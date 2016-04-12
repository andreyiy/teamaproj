using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamA.Repository;
using AccessModels.Models;
using System.Data.SqlClient;
using System.Web;
using Helpers.Mail;
using DAL.Repository;
using ServiceHelpers;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp;


namespace BusinessLayer
{
    public class UserService
    {
        private UserRepository _userRepository = new UserRepository();
        //private StudentService _studentService = new StudentService();
        

        public IEnumerable<UserProfile> GetAllUsers()
        {
            try
            {
                IEnumerable<UserProfile> users = _userRepository.GetAllUsers();
                return users;
            }
            catch (SqlException e)
            {
                Console.WriteLine("Service error " + e);
            }
            return null;
        }

        public IEnumerable<UserProfile> GetAllStudents()
        {
            var users = _userRepository.GetAllUsers();
            var students = users.Where(x => x.RoleName == "Student");
            return students;
        }

        public IEnumerable<UserProfile> GetAllTeachers()
        {
            var users = _userRepository.GetAllUsers();
            var teachers = users.Where(x => x.RoleName == "Teacher");
            return teachers;
        }

        public Tuple<int, string, string> GetUser(string username)
        {
            var a = GetAllUsers();
            var user = a.Where(q => q.Username == username).SingleOrDefault();
            return new Tuple<int, string, string>(user.ID, user.Username, user.Email);
        }

        public void CreateStudentUser(string username, string password, string email, string teacherName)
        {
            _userRepository.CreateStudentUser(username, email, password, teacherName);
            var guidstring = _userRepository.GetGuid(username);
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;
            if (appUrl != "/") appUrl += "/";
            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);
            string body = "Click the link to confirm the mail:\n";
            MailHelper.SendMail(new List<string>() { email }, "admin@admin.com", "Confirmation mail", body + baseUrl + "Account/ConfirmRegistration?GUID=" + guidstring);
        }

        public int CheckGuid(string guid)
        {
            return _userRepository.CheckGuid(guid);
        }

        public bool Login(string username, string password)
        {
            if (_userRepository.Login(username, password))
                return true;
            else
                return false;
        }

        public bool IsInRole(string username, string givenRole)
        {
            var role =_userRepository.GetRole(username);
            if (role == givenRole)
                return true;
            else return false;
        }
       
        public string GetRole(string username)
        {

            var role = _userRepository.GetRole(username);
            return role;
        }

        public IEnumerable<UserProfile> GetAllUnassignedStudents()
        {
            return _userRepository.GetAllUnassignedStudents();
        }

        public string GetTeacherName(int studentID)
        {
            int index = _userRepository.GetTeacherID(studentID);
            if (index == -1)
            {
                return "Not found";
            }
            var a = GetAllUsers();
            var user = a.Where(q => q.ID == index).SingleOrDefault();
            return user.Username;
        }
    }
}
