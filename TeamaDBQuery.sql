create table UserProfile(
		ID int identity(1,1),
		Username nvarchar(50) UNIQUE not null,
		[Password] nvarchar(128) not null,
		Email nvarchar(128) not null,
		constraint PK_UserProfile primary key(ID)
	)

create table Administrator
	(
		UserID int,
		constraint PK_Administrator primary key(UserID),
		constraint FK_Administrator_UserProfile foreign key(UserID) references UserProfile(ID)
	)

create table Student
	(
		UserID int,
		registeredTeacherID int,
		constraint PK_Student primary key(UserID),
		constraint FK_Student_UserProfile foreign key(UserID) references UserProfile(ID),
		constraint FK_Student_Teacher foreign key(UserID) references Teacher(UserID)
	)

create table Teacher
	(
		UserID int,
		constraint PK_Teacher primary key(UserID),
		constraint FK_Teacher_UserProfile foreign key(UserID) references UserProfile(ID)
	)

create table Homework
	(
		HomeworkID int,
		TeacherUserID int,
		[Description] nvarchar(511),
		Deadline datetime,
		constraint PK_Homework primary key(HomeworkID),
		constraint FK_Homework_Teacher foreign key(TeacherUserID) references Teacher(UserID)
	)

create table StudentToHomework
	(
		StudentUserID int,
		HomeworkID int,
		UploadID int identity(1,1),
		UploadDate datetime,
		Comment nvarchar(255),
		[Status] nvarchar(12),
		Grade int,
		constraint PK_StudentToHomework primary key(UploadID),
		constraint FK_StudentToHomework_Student foreign key(StudentUserID) references Student(UserID),
		constraint FK_StudentToHomework_Homework foreign key(HomeworkID) references Homework(HomeworkID)
	)

create table HomeworkFile
	(
		UploadID int,
		NumeFisier nvarchar(50),
		constraint PK_HomeworkFiles primary key(UploadID),
		constraint FK_HomeworkFiles_StudentToHomework foreign key(UploadID) references StudentToHomework(UploadID)
	)