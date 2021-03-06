USE [TeamADatabase]
GO
/****** Object:  StoredProcedure [dbo].[spCheckDeadline]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spCheckDeadline]
  	 as
	 	 begin
	 
		
	Declare @totalRows int
	Declare @count int
	set @count=1
	set @totalRows= (Select   Count(*) from(Select  StudentID, f.HomeworkID,f.TeacherUserID,f.Deadline from
		                  (Select  TeacherUserID,Homework.Deadline,HomeworkID from Homework where Deadline<GetDate()) f
		
		                      left join
		                      StudentsToTeachers
		                      on StudentsToTeachers.TeacherID=f.TeacherUserID) t)

		
		/*Loops through every row and selects the specific homeid and studentid*/
		
			While(@count <=@totalRows)
			begin

			/*Homework id*/
			Declare   @homeId int 
								
		 set @homeId=   (select  Cast(( Select y.HomeworkID from( Select top(@count) * From (Select  StudentID, f.HomeworkID,f.TeacherUserID,f.Deadline from
		                  (Select  TeacherUserID,Homework.Deadline,HomeworkID from Homework where Deadline<GetDate()) f
		
		      left join
		    StudentsToTeachers
		    on StudentsToTeachers.TeacherID=f.TeacherUserID)t
                EXCEPT
               Select top (@count-1) * From (Select  StudentID, f.HomeworkID,f.TeacherUserID,f.Deadline from
		                  (Select  TeacherUserID,Homework.Deadline,HomeworkID from Homework where Deadline<GetDate()) f
		
		      left join
		    StudentsToTeachers
		    on StudentsToTeachers.TeacherID=f.TeacherUserID)t)y)as int))
	



/*Student id */
			Declare   @studId int 		
								
		     set @studId= (select  Cast(( Select y.StudentID from( Select top(@count) * From (Select  StudentID, f.HomeworkID,f.TeacherUserID,f.Deadline from
		                  (Select  TeacherUserID,Homework.Deadline,HomeworkID from Homework where Deadline<GetDate()) f
		
		      left join
		    StudentsToTeachers
		    on StudentsToTeachers.TeacherID=f.TeacherUserID)t
                EXCEPT
               Select top (@count-1) * From (Select  StudentID, f.HomeworkID,f.TeacherUserID,f.Deadline from
		                  (Select  TeacherUserID,Homework.Deadline,HomeworkID from Homework where Deadline<GetDate()) f
		
		      left join
		    StudentsToTeachers
		    on StudentsToTeachers.TeacherID=f.TeacherUserID)t)y)as int))



			/*Checks if the status is there*/
                 if((select Count(*) from (Select [Status] from StudentToHomework where StudentUserID=@studId and HomeworkID=@homeId)t)=0)
				 begin
				 
				 Insert into StudentToHomework values(@studId,@homeId,null,GetDate(),null,'Rejected',0)

				 end
				 
		 set	@count=@count+1
		 end

/*End while loop*/


		 	 Declare @Counter int 
			 	 set @Counter=(SELECT Count(Rowid)as totalRows FROM vwdeadlinestatus)
	 Declare @i int
	 	 set @i=1
		 	 while(@i <=@Counter)
			 	 Begin
	 
update StudentToHomework set [Status]='Rejected', Grade=0 where [status]!='Accepted' and Uploadid=(Select Uploadid from vwdeadlinestatus where RowId=@i)

	set @i=@i+1
end
end
GO
/****** Object:  StoredProcedure [dbo].[spCheckGuid]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spCheckGuid] 
@guid nvarchar(50)
as
begin
if((Select UserId from RegistrationHash where HashConfirmationCode=@guid) is not null )
begin
    if((select isconfirmed from UserProfile where id=(Select UserId from RegistrationHash where HashConfirmationCode=@guid))=0 )
     begin
     Update UserProfile set IsConfirmed=1 where id=(Select UserId from RegistrationHash where HashConfirmationCode=@guid) 
      Select 1 as number from UserProfile where RoleID=1
	 end
    else
    begin
	Select 0 as number from UserProfile where RoleID=1
    end
end
else
begin
Select -1 as number from UserProfile where RoleID=1
end
end
GO
/****** Object:  StoredProcedure [dbo].[spCreateHomework]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spCreateHomework]
@teacherID nvarchar(50),
@homeworkName nvarchar(20),
@homeWorkDescription nvarchar(50),
@deadlineInDays datetime
as
begin

Insert into Homework values (@teacherID,@homeworkName,@homeWorkDescription,@deadlineInDays)
select Homework.HomeworkID from Homework where TeacherUserID = @teacherID AND name = @homeworkName AND [Description] = @homeWorkDescription AND Homework.Deadline = @deadlineInDays

end


GO
/****** Object:  StoredProcedure [dbo].[spCreateStudent]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spCreateStudent] 
@username nvarchar(50),
@password nvarchar(max),
@email nvarchar(128),
@teacherName  nvarchar(50) = null
as
begin
Insert into UserProfile values(@username,@password,@email,3,0)
if(not @teacherName is null)
Begin
Insert into StudentsToTeachers values((Select Id from UserProfile where Username=@username),(Select id from UserProfile where Username=@teacherName))
 end
 else 
 begin
 Insert into StudentsToTeachers values((Select Id from UserProfile where Username=@UserName),null)
 end
  
 if((Select Id from UserProfile where Username=@UserName) Is Not Null)
 	  Begin
	     --If username exists
		 Declare @GUID UniqueIdentifier
		 Set @GUID = NewID()
		 Insert into RegistrationHash values ((Select Id from UserProfile where Username=@UserName),@GUID)
    End

end

GO
/****** Object:  StoredProcedure [dbo].[spCreateTeacher]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spCreateTeacher]
@teacherName nvarchar(50),
@password nvarchar(max),
@email nvarchar(128)
as
begin
Insert into UserProfile values(@teacherName,@password,@email,2,1)
End
GO
/****** Object:  StoredProcedure [dbo].[spGetAllHomework]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[spGetAllHomework]
as
begin
  Select HomeworkID,TeacherUserID,Name,Description,Deadline from Homework
  
end
GO
/****** Object:  StoredProcedure [dbo].[spGetAllUnassignedStudents]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spGetAllUnassignedStudents]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * 
	from StudentsToTeachers st
	right join
	(
		select up.ID, up.Username, up.Email, up.IsConfirmed, ur.RoleName
		from UserProfile up
		inner join 
		UserRole ur
		on up.RoleID = ur.RoleID
		where ur.RoleName = 'Student'
	) upur
	on st.StudentID = upur.ID
	where st.TeacherID is NULL
END

GO
/****** Object:  StoredProcedure [dbo].[spGetAllUploads]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 Create proc [dbo].[spGetAllUploads]
 as
 begin

 Select StudentUserID,HomeworkID,UploadID,[FileName],UploadDate,Comment,[Status],Grade  from StudentToHomework

 end
GO
/****** Object:  StoredProcedure [dbo].[spGetAllUsers]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spGetAllUsers]
as
begin
  Select Id,Username,[Password],Email,RoleName,IsConfirmed from UserProfile 
  join UserRole
  on UserProfile.RoleID=UserRole.RoleID
end
GO
/****** Object:  StoredProcedure [dbo].[spGetCompletedHomeworkUpload]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spGetCompletedHomeworkUpload]  
@username nvarchar(50),
@homeworkID int
as
begin
select d.HomeworkID,d.HomeworkName,d.TeacherUserID,d.TeacherName,d.[Description],d.Deadline,d.Comment,d.UploadDate,d.[Status],d.UploadID,d.Grade from (Select p.HomeworkID,p.HomeworkName,p.TeacherName,p.[Description],p.TeacherUserID,p.Deadline,h.[Status],h.Comment,h.UploadDate,h.UploadID,h.Grade from
(Select HomeworkID,TeacherUserID,(select username from userprofile where id=(select TeacherID from StudentsToTeachers where StudentID=(Select id from UserProfile where Username=@username)))as TeacherName,Name as HomeworkName,[Description],Deadline from  homework 
where teacherUserId=(Select TeacherId from StudentsToTeachers where StudentID=(Select id from UserProfile where Username = @username))and HomeworkID=@homeworkID) p
left join  (Select Grade,UploadDate,HomeworkID,[Status],Comment,UploadID from StudentToHomework 
where StudentUserID=(Select id from UserProfile where username=@username) and  ([Status]='Accepted' or [Status]='Rejected'))h
on p.HomeworkID=h.HomeworkID) d where d.[Status]= 'Accepted' or d.[Status]='Rejected'
end
GO
/****** Object:  StoredProcedure [dbo].[spGetGUID]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[spGetGUID]
@username nvarchar(50)
as
begin 
Select HashConfirmationCode from RegistrationHash where UserID=(Select Id from UserProfile where Username=@username)
end
GO
/****** Object:  StoredProcedure [dbo].[spGetHomework]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spGetHomework]
	-- Add the parameters for the stored procedure here
	@homeworkID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Homework.Deadline, [Description], Homework.HomeworkID, Homework.Name as HomeworkName, Homework.TeacherUserID from Homework where HomeworkID = @homeworkID
END

GO
/****** Object:  StoredProcedure [dbo].[spGetOneTeacherHomework]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[spGetOneTeacherHomework]
@usernameTeacher nvarchar(50)
as
begin

Select HomeworkId,TeacherUserId,Name as HomeworkName,[Description],Deadline from Homework where TeacherUserID=(Select id from UserProfile where Username=@usernameTeacher)

end
GO
/****** Object:  StoredProcedure [dbo].[spGetPendingHomeworkUpload]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spGetPendingHomeworkUpload]  

@username nvarchar(50),

@homeworkID int

as

begin



Select p.HomeworkID,p.HomeworkName,p.TeacherName,p.TeacherUserID,p.[Description],p.Deadline,h.[Status],h.Comment,h.UploadDate,h.UploadID from

(Select HomeworkID,(select username from userprofile where id=(select TeacherID from StudentsToTeachers where StudentID=(Select id from UserProfile where Username=@username)))as TeacherName,TeacherUserID,Name as HomeworkName,[Description],Deadline from  homework 

where teacherUserId=(Select TeacherId from StudentsToTeachers where StudentID=(Select id from UserProfile where Username = @username))and HomeworkID=@homeworkID) p

left join  (Select UploadDate,HomeworkID,[Status],Comment,UploadID from StudentToHomework 

where StudentUserID=(Select id from UserProfile where username=@username) and  ([Status]='Commented' or [Status]='Uploaded'))h

on p.HomeworkID=h.HomeworkID

end
GO
/****** Object:  StoredProcedure [dbo].[spGetRole]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spGetRole] 
@username nvarchar(50)
as
begin
select RoleName from UserRole where RoleID = (select RoleID from UserProfile where Username=@username)
end
GO
/****** Object:  StoredProcedure [dbo].[spGetStudentHomeworkDetails]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetStudentHomeworkDetails]
	-- Add the parameters for the stored procedure here
	@studentID int,
	@homeworkID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @studentID int;
	--declare @homeworkID int;
	--set @studentID = 50;
	--set @homeworkID = 11;
	SELECT top 1 h.HomeworkID as HomeworkId, h.Name as HomeWorkName, Grade as StudentGrade, [Status], stupsh.Comment as Comment, [Description], Deadline, stupsh.[Status], stupsh.StudentID, h.TeacherUserID as TeacherID, (select UserProfile.Username from UserProfile where ID = h.TeacherUserID) as TeacherName
	--select Distinct *
	from 
	homework h
	inner join 
	(
		select HomeworkID, [Status], Grade, TeacherID, StudentID, Comment
		from StudentToHomework sh
		left join
		(
			select TeacherID, Username as TeacherName, StudentID
			from StudentsToTeachers st
			inner join UserProfile up
			on st.TeacherID = up.ID
			where st.StudentID = @studentID
		) stup
		on sh.StudentUserID = stup.StudentID
	) stupsh
	on h.HomeworkID = stupsh.HomeworkID
	where (h.HomeworkID = @homeworkID) AND (StudentID = @studentID)

END

GO
/****** Object:  StoredProcedure [dbo].[spGetStudentsAvgGradeByTeacher]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spGetStudentsAvgGradeByTeacher] 
@teachername nvarchar(50)
as
begin
declare @a int
set  @a=(select count(*) from homework where Homework.TeacherUserID=(select id from userprofile where username =@teachername ))

Select s.StudentID,s.Username,s.AvgGrade,e.Comment,e.UploadDate  
from 
	(
		Select f.StudentID,f.Username,g.AvgGrade 
		from  
			(
				Select k.StudentID,UserProfile.Username 
				from 
					(
						select studentId 
						from StudentsToTeachers 
						where TeacherID=(Select id from UserProfile where Username=@teachername)
					) k
				left join 
				userProfile 
				on k.StudentID=UserProfile.ID
			) f
		Left join
		(
			Select StudentUserID,(sum(Grade)/@a) as AvgGrade 
			from  StudentToHomework 
			where ([status]='Accepted' or [status]='Rejected') Group by StudentUserID
		) g
		on f.StudentID=g.StudentUserID
	)s 
	left join  
	(
		Select d.StudentUserID,UploadDate,[Status],Comment  
		from 
		(
			select StudentUserID,max(UploadDate)as AvgDate 
			from StudentToHomework group by StudentUserID 
		)d 
		left join StudentToHomework 
		on d.AvgDate= StudentToHomework.UploadDate 
	)e 
	on e.StudentUserID=s.StudentID 
	where (e.[Status]='Accepted' or e.[Status]='Rejected')
end
GO
/****** Object:  StoredProcedure [dbo].[spGetStudentsGradeByTeacherAndHomework]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spGetStudentsGradeByTeacherAndHomework] 
@teacherName nvarchar(50),
@homeworkID int
as
begin
select k.StudentID,d.StudentName,d.[Status],d.Comment,d.Grade,d.UploadDate,d.UploadID  from 
(select studentId,TeacherID from StudentsToTeachers where TeacherID=(Select id from UserProfile where Username=@teacherName))k

Inner join

(Select StudentUserID,(Select username from UserProfile where id=StudentUserID)as StudentName,Grade,[Status],Comment,UploadDate,UploadID 
from StudentToHomework where homeworkId=@homeworkID and ([Status]='Accepted' or [Status]='Rejected') ) d
on k.StudentID=d.StudentUserID
end
GO
/****** Object:  StoredProcedure [dbo].[spGetStudentsToTeacher]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spGetStudentsToTeacher]
as
begin
Select  Student,StudentEmail,Username from vwStudentToTeacher 
left join  Userprofile
on vwStudentToTeacher.teacherId=Userprofile.Id 
end
GO
/****** Object:  StoredProcedure [dbo].[spGetStudentUploadPath]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE proc [dbo].[spGetStudentUploadPath] 
 @username nvarchar(50),
@homeworkID nvarchar(50)
as
begin

 select k.TeacherId as TeacherId,k.TeacherName as TeacherName,k.StudentUserId as StudentId,k.UploadId as uploadId, p.Name as HomeWorkName  from

    (Select top 1 N.TeacherId as TeacherId, N.TeacherName as TeacherName,N.StudentUserId as StudentUserId,N.UploadId as UploadId   
	from ( Select E.TeacherId as TeacherId,E.TeacherName as TeacherName,M.StudentUserId as StudentUserId ,M.UploadId as UploadId  
	from ( Select  1 as ID,(Select TeacherUserID from Homework where HomeworkID=1) as TeacherId,Username as TeacherName,null as StudentUserId,null as UploadId 
    from userprofile  where  id=(Select TeacherUserID from Homework where HomeworkID=@homeworkID)

    union   

          Select  1 as ID,null as col1,null as col2,StudentUserId,UploadId 
		  from StudentToHomework 
		  where StudentUserID=(Select id from UserProfile where username=@username)) E 

		  right join 
		  ( Select  1 as ID,(Select TeacherUserID from Homework where HomeworkID=@homeworkID) as TeacherId,Username as TeacherName,null as StudentUserId,null as UploadId 
		  from userprofile  
		  where  id=(Select TeacherUserID from Homework where HomeworkID=@homeworkID)

    union   

          Select  1 as ID,null as col1,null as col2,StudentUserId,UploadId 
		  from StudentToHomework 
		  where StudentUserID=(Select id from UserProfile where username=@username)) M 

		   on E.ID=M.ID and isnull(E.StudentUserId,M.StudentUserId) =M.StudentUserId 
		   and ISNULL(E.UploadId,m.UploadId)=m.UploadId and isnull(M.TeacherId,E.TeacherId)=e.TeacherId and isnull(M.TeacherName,E.TeacherName)=e.TeacherName   ) N ) k

		    join ((select (Select UploadId from StudentToHomework where HomeworkID=@homeworkID )as UploadId,HomeworkID, Name from  Homework where HomeworkID=@homeworkID)    ) p

		   on k.UploadId=p.UploadId

end
GO
/****** Object:  StoredProcedure [dbo].[spInsertCommentAndGrade]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spInsertCommentAndGrade] 

@uploadId int,

@grade int = null,

@Comment nvarchar(255) = null

as

begin

	if ((not 'Accepted' in 		
				(
					select distinct [Status]from StudentToHomework sh	where sh.StudentUserID = (select sh1.StudentUserID			
					from StudentToHomework sh1
					where sh1.UploadID = @uploadId
				)

				AND sh.HomeworkID =

				(
					select sh2.HomeworkID
					from StudentToHomework sh2
					where sh2.UploadID = @uploadId

				)

		)

		) AND (not 'Rejected' in

		(

			select distinct [Status]

			from StudentToHomework sh

			where sh.StudentUserID = 

				(

					select sh1.StudentUserID

					from StudentToHomework sh1

					where sh1.UploadID = @uploadId

				)

				AND sh.HomeworkID =

				(

					select sh2.HomeworkID

					from StudentToHomework sh2

					where sh2.UploadID = @uploadId

				)

		)

		))

	begin

		if(((Select @grade)is not null) and  ((Select @grade) >=1) and((Select @grade) <=10) )

			begin

			Update StudentToHomework set [Status]='Accepted' , Grade=@grade  where UploadID=@uploadId and [Status]!='Rejected' and [Status]!='Accepted'
			Select 1 as col

			end

		else 
			begin
			if(((Select @Comment) is not null) and ((Select @grade )is null))

				begin

				Update StudentToHomework set Comment=@Comment, [Status]='Commented' where UploadID=@uploadId and [Status]!='Rejected' and [Status]!='Accepted'
				Select 1 as col
				end

			else 
				begin
				if(((Select @Comment) is null) and ((Select @grade )is null))

					begin

					Update StudentToHomework set [Status]='Rejected',Grade=0 where UploadID=@uploadId and [Status]!='Rejected' and [Status]!='Accepted'
					Select 1 as col
					end
				else
					begin
					select 0 as col
					end
				end
			end

	end

else

	begin

	select 0 as col

	end

end

GO
/****** Object:  StoredProcedure [dbo].[spInsertStudentToHomework]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spInsertStudentToHomework]
@username nvarchar(50),
@filename nvarchar(20),
@homeworkID int

as
begin
if((Select id from UserProfile where Username=@username)is not null  )
begin
Insert  into StudentToHomework values((Select id from UserProfile where Username=@username),@homeworkID,@filename,GetDate(),null,'Uploaded',0)
Select UploadID from StudentToHomework where StudentUserID=(Select id from UserProfile where Username=@username) and HomeworkID=@homeworkID and UploadDate=(Select Max(UploadDate)as UploadDate from StudentToHomework where HomeworkID=@homeworkID and StudentUserID=(Select id from UserProfile where Username=@username))
end
end
GO
/****** Object:  StoredProcedure [dbo].[spInsertTeacherToStudent]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spInsertTeacherToStudent]

	 @teacherName nvarchar(50),

	 @studentId int

	 as

	 begin



	 if((Select TeacherID from StudentsToTeachers where StudentID=@studentId) is null)

	 begin

	       Update StudentsToTeachers set TeacherID=(Select id from UserProfile where Username=@teacherName) where StudentID=@studentId

	 end

	 end
GO
/****** Object:  StoredProcedure [dbo].[spLogin]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spLogin]
@username nvarchar(50),
@password nvarchar(max)
as
begin
Declare @Count int

Select @Count= Count(UserName) from UserProfile where Username=@username and
[Password]=@password and IsConfirmed = 1

if(@Count=1)
	begin
	   select 1 as ReturnCode
	end
	else
		begin
		Select @Count= Count(UserName) from UserProfile where Username=@username and
		[Password]=@password
		if (@Count=1)
			Begin
			select 0 as ReturnCode
			end
			else
				begin
				select -1 as ReturnCode
				end
		end
end

GO
/****** Object:  StoredProcedure [dbo].[spResetPassword]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[spResetPassword]
@username nvarchar(50),
@password nvarchar(max)
as
begin
If((Select Username from UserProfile where Username=@username) is not null )
begin

Update UserProfile set [Password]=@password where Username=@username

end
end
GO
/****** Object:  StoredProcedure [dbo].[spStudentHomeworkCompleted]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spStudentHomeworkCompleted] 

@username nvarchar(50)

as

begin

Select p.HomeworkID,p.HomeworkName,p.TeacherUserID,p.TeacherName,p.[Description],p.Deadline,h.[Status],h.Comment,h.UploadDate,h.Grade from

(Select HomeworkID,TeacherUserID,(Select username from UserProfile where id=TeacherUserID)as TeacherName,Name as HomeworkName,[Description],Deadline from  homework 

where teacherUserId=(Select TeacherId from StudentsToTeachers where StudentID=(Select id from UserProfile where Username = @username))) p

inner join  (Select UploadDate,q.Grade, q.HomeworkID,[Status],Comment 

from StudentToHomework q

	inner join (Select Max(UploadDate)as max, HomeworkID 
					from StudentToHomework q

				inner join UserProfile up on q.StudentUserID = up.ID 
				where up.username=@username
				group by HomeworkID) w
	on (q.HomeworkID = w.HomeworkID) and (q.UploadDate = w.max)
		inner join UserProfile up on q.StudentUserID = up.ID
 where 
  up.username=@username) h
  	on p.HomeworkID=h.HomeworkID
	where ([Status] = 'Accepted') OR ([Status] = 'Rejected')
	
end
GO
/****** Object:  StoredProcedure [dbo].[spStudentHomeworkPending]    Script Date: 01-Apr-16 9:09:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[spStudentHomeworkPending]  

 @username nvarchar(50)

 as



begin



Select p.HomeworkID,p.HomeworkName,p.TeacherName,p.TeacherUserID,p.[Description],p.Deadline,h.[Status],h.Comment,h.UploadDate 

from

	(Select HomeworkID,TeacherUserID,(Select username from UserProfile where id=TeacherUserID)as TeacherName,Name as HomeworkName,[Description],Deadline 

	from  homework 

	where teacherUserId = (Select TeacherId 

							from StudentsToTeachers 

							where StudentID= (Select id 

												from UserProfile 

												where Username = @username))) p



	left join (Select UploadDate, q.HomeworkID,[Status],Comment 

from StudentToHomework q

	inner join (Select Max(UploadDate)as max, HomeworkID 

				from StudentToHomework q

				inner join UserProfile up on q.StudentUserID = up.ID 

				where up.username=@username

				group by HomeworkID) w

	on (q.HomeworkID = w.HomeworkID) and (q.UploadDate = w.max)

	inner join UserProfile up on q.StudentUserID = up.ID

 where 

 up.username=@username) h

	on p.HomeworkID=h.HomeworkID where (p.Deadline>(Select GetDate()))

end
GO
