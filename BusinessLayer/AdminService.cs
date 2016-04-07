using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccessModels.Models;
using DAL.Repository;
using System.Data.SqlClient;
using System.IO;
using Helpers.Mail;
using TeamA.Repository;

namespace BusinessLayer
{
    public class AdminService
    {
        private AdminRepository adminRepository = new AdminRepository();
        private UserService userSerivce = new UserService();
        private UserRepository _userRepository = new UserRepository();

        public AdminService(AdminRepository adminRepository)
        {
            this.adminRepository=adminRepository;
        }

        public AdminService()
        {

        }



        public void AddTeachersFromAdmin(string username,string email, string basePath) {

             try
            {
                Random rndm = new Random();
                string password = rndm.Next(100000, 999999).ToString();
                List<string> list = new List<string>() { email };
                MailHelper.SendMail(list, "admin@admin.com", "Registration Details", password);

                UserProfile up = new UserProfile(); 
                up.Email = email;
                up.Username = username;
                up.Password = password;

                
                
                adminRepository.AddTeachersFromAdmin(up);

                Directory.CreateDirectory(basePath + username + '_' + Convert.ToString(userSerivce.GetUser(username).Item1));
            }
             catch (SqlException e)
             {
                 //Console.WriteLine("admin service" + e);
                 e.Message.ToString();


             }
        }

        public void InsertTeacherToStudent(string teacherName, int studentID)
        {
            try
            {
                adminRepository.InsertTeacherToStudent(teacherName, studentID);
            }
            catch (SqlException e)
            {

                e.Message.ToString(); 
            }
            
        }
        

        public void ResetPasswordSendMail(string username)
        {
            string password = _userRepository.ResetPassword(username);
            string getUserEmail = _userRepository.GetAllUsers().Where(x => x.Username == username).Select(x => x.Email).FirstOrDefault();
            if (getUserEmail!=null)
            {
                MailHelper.SendMail(new List<string> { getUserEmail }, "account@account.com", "New Password", password);
            }
        }
      
    }
}
