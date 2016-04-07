using AccessModels.Models;
using BusinessLayer.Models;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class StudentService
    {
        private StudentRepository _studentRepository = new StudentRepository();
        private UserService _userService = new UserService();

        public int InsertStudentToHomework(string userName, int homeworkID, string fileName, string path)
        {
             int uploadID = _studentRepository.InsertStudentToHomework(userName, fileName, homeworkID);

             if (!Directory.Exists(path))
             {
                 Directory.CreateDirectory(path);
             }

             return uploadID;
        }

        
        public IEnumerable<Tuple<string, string, string>> GetStudentsBelongingToTeacher(string teacherName)
        {
            var studentsToTeachers = _studentRepository.GetStudentsToTeachers();
            var studentsFromSameTeacher = studentsToTeachers.Where(x => x.Item3 == teacherName);
            return studentsFromSameTeacher.ToList();
        }

        public Tuple<string, string, string> GetTeacherBelongingToStudent(string studentName)
        {
            var studentsToTeachers = _studentRepository.GetStudentsToTeachers();
            var teachersOfStudent = studentsToTeachers.Where(x => x.Item1 == studentName).FirstOrDefault();
            return teachersOfStudent;
        }

        public IEnumerable<Tuple<string, string, string>> GetStudentsByTeacher(string username)
        {
            var students = _userService.GetAllStudents();
            var teacher = _userService.GetUser(username);
            var studentsToTeachers = GetStudentsBelongingToTeacher(username);

            return studentsToTeachers;

        }

        public IEnumerable<StudentHomeworkDetailsVM> GetStudentPendingHomework(string userName)
        {
            List<StudentHomeworkDetails> studentPendingHomeworkList = _studentRepository.GetStudentPendingHomework(userName);
            List<StudentHomeworkDetailsVM> studentPendingHomeworkListVM = new List<StudentHomeworkDetailsVM>();
            foreach (var item in studentPendingHomeworkList)
                studentPendingHomeworkListVM.Add(new StudentHomeworkDetailsVM()
                    {
                        Comment = item.Comment,
                        Deadline = item.Deadline,
                        Description = item.Description,
                        HomeworkId = item.HomeworkId,
                        HomeWorkName = item.HomeWorkName,
                        Status = item.Status,
                        StudentGrade = item.StudentGrade,
                        TeacherId = item.TeacherId,
                        TeacherName = item.TeacherName,
                        UploadDate = item.UploadDate,
                        UploadId = item.UploadId
                    });

            return studentPendingHomeworkListVM;

        }

        public IEnumerable<StudentHomeworkDetails> GetStudentCompletedHomework(string userName)
        {
            List<StudentHomeworkDetails> studentCompletedHomeworkList = _studentRepository.GetStudentCompletedHomework(userName);

            return studentCompletedHomeworkList;
        }

        public List<StudentHomeworkDetails> GetCompletedHomeworkUpload(string userName, int homeworkId)
        {
            List<StudentHomeworkDetails> completedHomeworkUploadList = _studentRepository.GetCompletedHomeworkUpload(userName, homeworkId);
            return completedHomeworkUploadList;
        }
        public List<StudentHomeworkDetails>  GetPendingHomeworkUpload(string userName, int homeworkId)
        {
            List<StudentHomeworkDetails> studentPendingHomeworkUpload = _studentRepository.GetPendingHomeworkUpload(userName, homeworkId);
            return studentPendingHomeworkUpload;
        }



    }



}
