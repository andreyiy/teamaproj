create table UserRole
	(
		RoleID int UNIQUE not null,
		RoleName nvarchar(50) not null,
		constraint PK_Role primary key(RoleID)
	)

create table UserProfile(
		ID int identity(1,1),
		Username nvarchar(50) UNIQUE not null,
		[Password] nvarchar(128) not null,
		Email nvarchar(128)UNIQUE not null,
		RoleID int,
		IsConfirmed int,
		constraint PK_UserProfile primary key(ID),
		constraint FK_UserProfile_Role foreign key(RoleID) references UserRole(RoleID)
	)

create table StudentsToTeachers
	(
		StudentID int,
		TeacherID int,
		constraint PK_StudentsToTeachers primary key(StudentID),
		constraint FK_StudentsToTeachers_UserProfileStudent foreign key(StudentID) references UserProfile(ID),
		constraint FK_StudentsToTeachers_UserProfileTeacher foreign key(TeacherID) references UserProfile(ID)
	)

create table Homework
	(
		HomeworkID int identity,
		TeacherUserID int,
		Name nvarchar(20),
		[Description] nvarchar(511),
		Deadline datetime,
		constraint PK_Homework primary key(HomeworkID),
		constraint FK_Homework_Teacher foreign key(TeacherUserID) references UserProfile(ID)
	)

Create table StudentToHomework
	(
		StudentUserID int,
		HomeworkID int,
		UploadID int identity(1,1),
		[FileName] nvarchar(50),
		UploadDate datetime,
		Comment nvarchar(255),
		[Status] nvarchar(12),
		Grade int
		constraint PK_StudentToHomework primary key(UploadID),
		constraint FK_StudentToHomework_Student foreign key(StudentUserID) references UserProfile(ID),
		constraint FK_StudentToHomework_Homework foreign key(HomeworkID) references Homework(HomeworkID)
	)


Create table RegistrationHash
(
UserID int primary key,
HashConfirmationCode nvarchar(max),
constraint FK_RegistrationHash_UserID foreign key(UserID) references UserProfile(ID)
)