-- ============================================================
-- Supermarket Survey System - Database Schema
-- MySQL 8.0+
-- ============================================================

CREATE DATABASE IF NOT EXISTS supermarket_survey CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE supermarket_survey;

-- ============================================================
-- OUTLETS
-- ============================================================
CREATE TABLE Outlets (
    Id         INT           NOT NULL AUTO_INCREMENT,
    OutletName VARCHAR(150)  NOT NULL,
    BranchCode VARCHAR(50)   NOT NULL UNIQUE,
    Address    VARCHAR(300)  NOT NULL,
    City       VARCHAR(100)  NOT NULL,
    CreatedAt  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);

-- ============================================================
-- MERCHANDISERS
-- ============================================================
CREATE TABLE Merchandisers (
    Id           INT          NOT NULL AUTO_INCREMENT,
    Name         VARCHAR(150) NOT NULL,
    Age          INT          NOT NULL,
    EmployeeId   VARCHAR(50)  NOT NULL UNIQUE,
    Email        VARCHAR(200) NOT NULL UNIQUE,
    PasswordHash VARCHAR(300) NOT NULL,
    AssignedOutletId INT      NULL,
    IsActive     TINYINT(1)   NOT NULL DEFAULT 1,
    CreatedAt    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    CONSTRAINT FK_Merchandisers_Outlets FOREIGN KEY (AssignedOutletId) REFERENCES Outlets(Id)
);

-- ============================================================
-- SURVEYS
-- ============================================================
CREATE TABLE Surveys (
    Id          INT          NOT NULL AUTO_INCREMENT,
    Title       VARCHAR(300) NOT NULL,
    Description TEXT         NULL,
    IsActive    TINYINT(1)   NOT NULL DEFAULT 1,
    CreatedAt   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);

-- ============================================================
-- QUESTIONS
-- ============================================================
CREATE TABLE Questions (
    Id           INT          NOT NULL AUTO_INCREMENT,
    SurveyId     INT          NOT NULL,
    QuestionText TEXT         NOT NULL,
    OrderNumber  INT          NOT NULL DEFAULT 1,
    CreatedAt    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    CONSTRAINT FK_Questions_Surveys FOREIGN KEY (SurveyId) REFERENCES Surveys(Id) ON DELETE CASCADE
);

-- ============================================================
-- ANSWER OPTIONS
-- ============================================================
CREATE TABLE AnswerOptions (
    Id           INT          NOT NULL AUTO_INCREMENT,
    QuestionId   INT          NOT NULL,
    OptionLabel  VARCHAR(10)  NOT NULL,   -- A, B, C, D …
    OptionText   VARCHAR(500) NOT NULL,
    CreatedAt    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    CONSTRAINT FK_AnswerOptions_Questions FOREIGN KEY (QuestionId) REFERENCES Questions(Id) ON DELETE CASCADE
);

-- ============================================================
-- RESPONDENTS
-- ============================================================
CREATE TABLE Respondents (
    Id          INT          NOT NULL AUTO_INCREMENT,
    Name        VARCHAR(150) NOT NULL,
    Age         INT          NOT NULL,
    Gender      VARCHAR(20)  NOT NULL,   -- Male / Female / Other
    Phone       VARCHAR(30)  NULL,
    Occupation  VARCHAR(150) NOT NULL,
    IncomeRange VARCHAR(100) NOT NULL,
    Location    VARCHAR(200) NOT NULL,
    SurveyDate  DATE         NOT NULL,
    Notes       TEXT         NULL,
    CreatedAt   DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
);

-- ============================================================
-- SURVEY RESPONSES  (header)
-- ============================================================
CREATE TABLE SurveyResponses (
    Id              INT      NOT NULL AUTO_INCREMENT,
    SurveyId        INT      NOT NULL,
    RespondentId    INT      NOT NULL,
    OutletId        INT      NOT NULL,
    MerchandiserId  INT      NOT NULL,
    SubmittedAt     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    CONSTRAINT FK_SR_Surveys      FOREIGN KEY (SurveyId)       REFERENCES Surveys(Id),
    CONSTRAINT FK_SR_Respondents  FOREIGN KEY (RespondentId)   REFERENCES Respondents(Id),
    CONSTRAINT FK_SR_Outlets      FOREIGN KEY (OutletId)       REFERENCES Outlets(Id),
    CONSTRAINT FK_SR_Merchandisers FOREIGN KEY (MerchandiserId) REFERENCES Merchandisers(Id)
);

-- ============================================================
-- SURVEY RESPONSE DETAILS  (one row per answered question)
-- ============================================================
CREATE TABLE SurveyResponseDetails (
    Id               INT NOT NULL AUTO_INCREMENT,
    SurveyResponseId INT NOT NULL,
    QuestionId       INT NOT NULL,
    AnswerOptionId   INT NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_SRD_SurveyResponses FOREIGN KEY (SurveyResponseId) REFERENCES SurveyResponses(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SRD_Questions       FOREIGN KEY (QuestionId)       REFERENCES Questions(Id),
    CONSTRAINT FK_SRD_AnswerOptions   FOREIGN KEY (AnswerOptionId)   REFERENCES AnswerOptions(Id)
);

-- ============================================================
-- INDEXES
-- ============================================================
CREATE INDEX IX_Questions_SurveyId     ON Questions(SurveyId);
CREATE INDEX IX_AnswerOptions_QuestionId ON AnswerOptions(QuestionId);
CREATE INDEX IX_SurveyResponses_SurveyId ON SurveyResponses(SurveyId);
CREATE INDEX IX_SurveyResponses_OutletId ON SurveyResponses(OutletId);
CREATE INDEX IX_SurveyResponseDetails_SurveyResponseId ON SurveyResponseDetails(SurveyResponseId);
