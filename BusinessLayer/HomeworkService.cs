using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamA.Repository;
using AccessModels.Models;
using System.Data.SqlClient;
using System.IO;
using BusinessLayer.Models;
using Helpers.Mail;

namespace BusinessLayer
{
    public class HomeworkService
    {
        private HomeworkRepository hwRepository = new HomeworkRepository();
        private UserRepository userRepository = new UserRepository();
        private UserService userService = new UserService();
        private StudentService studentService = new StudentService();



        public IEnumerable<Homework> GetAllHomework()
        {
            try
            {
                IEnumerable<Homework> hw = hwRepository.GetAllHomework();
                return hw;
            }
            catch (SqlException e)
            {
                Console.WriteLine("Service error " + e);
            }
            return null;
        }

        public int CreateHomework(int TeacherUserID, string name, string description, DateTime deadline, string basePath)
        {
            var teachers = userService.GetAllTeachers();
            UserProfile teacher = teachers.Where(t => t.ID == TeacherUserID).FirstOrDefault();
            int homeworkID = hwRepository.CreateHomework(TeacherUserID, name, description, deadline);
            string getStudEmail = userService.GetAllStudents().Select(x => x.Email).FirstOrDefault();

            if (teacher != null)
            {
                Directory.CreateDirectory(basePath + teacher.Username + '_' + Convert.ToString(teacher.ID) + '/' + name + '_' + Convert.ToString(homeworkID));
                if (getStudEmail != null)
                {
                    MailHelper.SendMail(new List<string> { getStudEmail }, "account@account.com", "New Homework" + deadline, description);
                }
            }
            else { }

            return homeworkID;
        }

        public IEnumerable<HomeworkVM> GetOneTeacherHomework(string username)
        {
            var teacherHomework = hwRepository.GetOneTeacherHomework(username);
            List<HomeworkVM> vmList = new List<HomeworkVM>();
            foreach (var item in teacherHomework)
                vmList.Add(new HomeworkVM()
                    {
                        Deadline = item.Deadline,
                        Description = item.Description,
                        HomeworkID = item.HomeworkID,
                        Name = item.HomeworkName,
                        TeacherID = item.TeacherUserID
                    });

            return vmList;
        }


        public void InsertCommentOrGradeOrStatus(int uploadId, int? grade = null, string comment = null)
        {

          int  commandStatus=  hwRepository.InsertCommentOrGradeOrStatus(uploadId, grade, comment);
            string getStudEmail = userService.GetAllStudents().Select(x => x.Email).FirstOrDefault();

          if ((commandStatus == 0) && (grade!= null) && (comment==null))
          {
              if ((grade <= 10) && (grade >= 1 && grade != null))
              {

                  throw new Exception("Grade already added!");

              }
              else
              {
                  throw new Exception("Failed to add grade!");
              }
          }
          else if ((commandStatus == 0) && (grade== null) && (comment!= null))
          {
              throw new Exception("Failed to add comment!");
          }
          else if ((commandStatus == 0) && (grade == null) && (comment == null))
          {
              throw new Exception("Failed to reject command!");
          }
          else
          {
              if (getStudEmail != null)
              {
                  MailHelper.SendMail(new List<string> { getStudEmail }, "account@account.com", "Homework review", "Your teacher has reviewd your homework");
              }
          }
         

           
        }

        public void CheckHomeworkDeadLine()            
        {
            hwRepository.CheckHomeworkDeadLine();
        }

        public Models.StudentHomeworkDetailsVM GetStudentHomeworkDetails(int studentID, int homeworkID)
        {
            var model = hwRepository.GetStudentHomeworkDetails(studentID, homeworkID);
            return new Models.StudentHomeworkDetailsVM()
            {
                HomeworkId = model.HomeworkId,
                HomeWorkName = model.HomeWorkName,
                StudentGrade = model.StudentGrade,
                Description = model.Description,
                Status = model.Status,
                TeacherId = model.TeacherId,
                TeacherName = model.TeacherName,
                Deadline = model.Deadline,
                Comment = model.Comment
            };
        }

          public List<StudentToHomework> GetStudentsAvgGradeByTeacher(string userName)
        {
             List<StudentToHomework>  studentAvgGradeByTeacher = hwRepository.GetStudentsAvgGradeByTeacher(userName);
       
              return studentAvgGradeByTeacher;
         }
        public List<StudentToHomework> GetStudentsGradeByTeacherAndHomework(string userName, int homeworkID)
        {
            List<StudentToHomework> studentGradeByTeacherAndHomework = hwRepository.GetStudentsGradeByTeacherAndHomework(userName,homeworkID);

            return studentGradeByTeacherAndHomework;
        }




}
    }

