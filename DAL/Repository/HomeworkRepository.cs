using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using AccessModels.Models;
namespace DAL.Repository
{
    public class HomeworkRepository
    {
        string cs = ConfigurationManager.ConnectionStrings["TeamAConnection"].ConnectionString;
        
        public int CreateHomework(int TeacherUserID, string name, string description, DateTime deadline)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spCreateHomework", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@teacherID", TeacherUserID);
                cmd.Parameters.AddWithValue("@homeWorkName", name);
                cmd.Parameters.AddWithValue("@homeWorkDescription", description);
                cmd.Parameters.AddWithValue("@deadlineInDays", deadline);
                con.Open();

                return (int)cmd.ExecuteScalar();
            }
        }
        public IEnumerable<Homework> GetAllHomework()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                List<Homework> homeworkList = new List<Homework>();
                SqlCommand cmd = new SqlCommand("spGetAllHomework", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    

                    Homework u = new Homework();
                    u.HomeworkID = Convert.ToInt32(rdr["HomeworkID"]);
                    u.HomeworkName = rdr["HomeworkName"].ToString();
                    u.TeacherUserID = Convert.ToInt32(rdr["TeacherUserID"].ToString());
                    u.Description = rdr["Description"].ToString();

                    u.Deadline = Convert.ToDateTime(rdr["Deadline"]);

                    homeworkList.Add(u);
                }
                return homeworkList;
            }
        }








        public List<Homework> GetOneTeacherHomework(string username)
        {
            try {

                List<Homework> teacherHomeworkList = new List<Homework>();
            using (SqlConnection con = new SqlConnection(cs))
            {

                

                SqlCommand cmd = new SqlCommand("spGetOneTeacherHomework", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@usernameTeacher", username);
              
                con.Open();

                    SqlDataReader rdr =cmd.ExecuteReader();
               
                    while(rdr.Read())
                    {
                        Homework teacherHomework = new Homework();

                        teacherHomework.HomeworkID = Convert.ToInt32(rdr["HomeworkId"]);
                        teacherHomework.HomeworkName = rdr["HomeWorkName"].ToString();
                        teacherHomework.TeacherUserID = Convert.ToInt32(rdr["TeacherUserId"]);
                        teacherHomework.Description = rdr["Description"].ToString();
                        teacherHomework.Deadline = Convert.ToDateTime(rdr["Deadline"]);

                        teacherHomeworkList.Add(teacherHomework);
                        
                    }

                    
            }

            return teacherHomeworkList;
            }
            catch (SqlException)
            {
                return new List<Homework>();
            }
            catch(Exception)
            {
                return new List<Homework>();
            }



        }

        public int InsertCommentOrGradeOrStatus(int uploadID, int? grade=null, string comment=null )
        {
            
            try
            {
                int commandStatus=0;
                using (SqlConnection con = new SqlConnection(cs))
                {

                    SqlCommand cmd = new SqlCommand("spInsertCommentAndGrade", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@uploadId", uploadID);
                    cmd.Parameters.AddWithValue("@Grade", grade);
                    cmd.Parameters.AddWithValue("@Comment", comment);

                   
                	con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while(rdr.Read())
                    {
                        commandStatus = Convert.ToInt32(rdr["col"]);
                    }
                    
            	}

                return commandStatus;
             }
            catch (SqlException)
            {
                return 0;
            }    
        }

        public void CheckHomeworkDeadLine()
        {
             try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {

                    SqlCommand cmd = new SqlCommand("spCheckDeadline", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                	con.Open();
                	cmd.ExecuteNonQuery();
            	}
             }
            catch (SqlException)
            {
                         
            
            }       
        }

        public StudentHomeworkDetails GetStudentHomeworkDetails(int studentID, int homeworkID)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetStudentHomeworkDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@studentID", studentID);
                cmd.Parameters.AddWithValue("@homeworkID", homeworkID);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    StudentHomeworkDetails model = new StudentHomeworkDetails()
                    {
                        HomeworkId = (int)rdr["HomeworkId"],
                        HomeWorkName = (string)rdr["HomeWorkName"],
                        StudentGrade = (int)rdr["StudentGrade"],
                        Description = (string)rdr["Description"],
                        Status = (string)rdr["Status"],
                        TeacherId = (int)rdr["TeacherID"],
                        TeacherName = (string)rdr["TeacherName"],
                        Deadline = Convert.ToDateTime(rdr["Deadline"]),
                        Comment = Convert.IsDBNull(rdr["Comment"]) ? "" : (string)rdr["Comment"]
                    };
                    return model;
                }
                return new StudentHomeworkDetails();
            }
        }
        public List<StudentToHomework> GetStudentsAvgGradeByTeacher(string userName)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                List<StudentToHomework> studentAvgGradeByTeacherList = new List<StudentToHomework>();

                SqlCommand cmd = new SqlCommand("spGetStudentsAvgGradeByTeacher", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@teachername", userName);

                con.Open();
                 SqlDataReader rdr =cmd.ExecuteReader();

                while(rdr.Read())
                {
                    StudentToHomework studentAvgGradeByTeacher = new StudentToHomework();

                    studentAvgGradeByTeacher.StudentName = rdr["Username"].ToString();
                    studentAvgGradeByTeacher.StudentUserID = Convert.ToInt32(rdr["StudentID"]);
                    
                    studentAvgGradeByTeacher.Comment = rdr["Comment"].ToString();
                    studentAvgGradeByTeacher.UploadDate = Convert.ToDateTime(rdr["UploadDate"]);
                    studentAvgGradeByTeacher.Grade = Convert.ToInt32(rdr["AvgGrade"]);
       
                    studentAvgGradeByTeacherList.Add(studentAvgGradeByTeacher);


                }
                return studentAvgGradeByTeacherList;


            }
        }

        public List<StudentToHomework> GetStudentsGradeByTeacherAndHomework(string userName, int homeworkID)
        {
            List<StudentToHomework> studentAvgGradeByTeacherAndHomeworkList = new List<StudentToHomework>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetStudentsGradeByTeacherAndHomework", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@teachername", userName);
                cmd.Parameters.AddWithValue("@homeworkId", homeworkID);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    StudentToHomework studentAvgGradeByTeacherAndHomework = new StudentToHomework();

                    studentAvgGradeByTeacherAndHomework.StudentName = rdr["StudentName"].ToString();
                    studentAvgGradeByTeacherAndHomework.StudentUserID = Convert.ToInt32(rdr["StudentID"]);
 
                    studentAvgGradeByTeacherAndHomework.Comment = rdr["Comment"].ToString();
                    studentAvgGradeByTeacherAndHomework.UploadDate = Convert.ToDateTime(rdr["UploadDate"]);
                    studentAvgGradeByTeacherAndHomework.UploadID = Convert.ToInt32(rdr["UploadId"]);
                    studentAvgGradeByTeacherAndHomework.Grade = Convert.ToInt32(rdr["Grade"]);

                    studentAvgGradeByTeacherAndHomeworkList.Add(studentAvgGradeByTeacherAndHomework);


                }
            }
                return studentAvgGradeByTeacherAndHomeworkList;

        }

        public Homework GetHomework(int homeworkID)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                List<StudentToHomework> studentAvgGradeByTeacherList = new List<StudentToHomework>();

                SqlCommand cmd = new SqlCommand("spGetHomework", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@homeworkID", homeworkID);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                Homework h = new Homework();

                while (rdr.Read())
                {
                    h.Deadline = Convert.ToDateTime(rdr["Deadline"]);
                    h.Description = (string)rdr["Description"];
                    h.HomeworkID = homeworkID;
                    h.HomeworkName = (string)rdr["HomeworkName"];
                    h.TeacherUserID = (int)rdr["TeacherUserID"];
                }

                return h;
            }
        }
            
    }
}
