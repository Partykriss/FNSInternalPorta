CREATE DATABASE FNSDb
	COLLATE Cyrillic_General_CI_AS
GO

USE FNSDb
GO

CREATE TABLE News (
    Id INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255),
    Content NVARCHAR(MAX),
    PublishDate DATETIME
);
GO

CREATE TABLE Feedback (
    ID INT PRIMARY KEY IDENTITY,
    Message NVARCHAR(MAX),
    UserName NVARCHAR(255),
    FeedDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Calls (
    Id INT PRIMARY KEY IDENTITY,
    UserName NVARCHAR(255),
    CallDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Links (
    ID INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255) NOT NULL,
    Url NVARCHAR(2048) NOT NULL,
    DisplayOrder INT NOT NULL
);

CREATE TABLE Votings (
    Id INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255),
    Description NVARCHAR(MAX),
    StartDate DATETIME,
	EndDate DateTime,
	IsActive BIT
);
GO

ALTER TABLE Votings
	ADD CONSTRAINT CHK_IsActive CHECK (IsActive IN (0, 1))
GO

ALTER TABLE Votings
	ADD CONSTRAINT CHK_VotingDates CHECK (StartDate < EndDate)
GO

CREATE TABLE Answers (
    Id INT PRIMARY KEY IDENTITY,
    VotingId INT,
	Title NVARCHAR (50),
	VotesCount INT DEFAULT 0
);
GO

ALTER TABLE Answers
	ADD CONSTRAINT FK_Answers_Votings FOREIGN KEY(VotingId)
	REFERENCES Votings(Id)
	ON DELETE NO ACTION
GO

CREATE TABLE VotedUsers (
    Id INT PRIMARY KEY IDENTITY,
    VotingId INT,
	AnswerId INT,
	UserLogin NVARCHAR(50)
);
GO

ALTER TABLE VotedUsers
	ADD CONSTRAINT FK_VotedUsers_Votings FOREIGN KEY(VotingId)
	REFERENCES Votings(Id)
	ON DELETE CASCADE
GO

ALTER TABLE VotedUsers
	ADD CONSTRAINT FK_VotedUsers_Answers FOREIGN KEY(AnswerId)
	REFERENCES Answers(Id)
	ON DELETE NO ACTION
GO

ALTER TABLE VotedUsers
	ADD CONSTRAINT UniqueVotePerUserPerVoting UNIQUE(VotingId, UserLogin)
GO

CREATE PROCEDURE AddVote
    @VotingId INT,
    @AnswerId INT,
    @UserLogin NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Проверка, проголосовал ли пользователь
    IF NOT EXISTS (SELECT * FROM VotedUsers 
		WHERE VotingId = @VotingId AND UserLogin = @UserLogin)
    BEGIN
        -- Добавление записи о голосе
        INSERT INTO VotedUsers (VotingId, AnswerId, UserLogin)
        VALUES (@VotingId, @AnswerId, @UserLogin);

        -- Обновление счетчика голосов
        UPDATE Answers
        SET VotesCount = VotesCount + 1
        WHERE Id = @AnswerId;
    END
END;
GO