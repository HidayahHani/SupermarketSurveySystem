# Entity Relationship Diagram — Supermarket Survey System

## ERD (Text Notation)

```
┌──────────────────────┐          ┌──────────────────────────┐
│        Outlets        │          │       Merchandisers       │
├──────────────────────┤          ├──────────────────────────┤
│ PK  Id               │◄─────────│ PK  Id                   │
│     OutletName        │  0..*    │     Name                 │
│     BranchCode (UQ)  │          │     Age                  │
│     Address          │          │     EmployeeId (UQ)      │
│     City             │          │     Email (UQ)           │
│     CreatedAt        │          │     PasswordHash         │
└──────────────────────┘          │ FK  AssignedOutletId     │
          ▲                       │     IsActive             │
          │ FK OutletId           │     CreatedAt            │
          │                       └──────────────────────────┘
          │                                   ▲
          │                                   │ FK MerchandiserId
          │                                   │
          └─────────────┬─────────────────────┘
                        │
               ┌────────┴────────────────────┐
               │       SurveyResponses        │
               ├─────────────────────────────┤
               │ PK  Id                      │
               │ FK  SurveyId  ──────────────┼──────────┐
               │ FK  RespondentId ───────────┼──────┐   │
               │ FK  OutletId                │      │   │
               │ FK  MerchandiserId          │      │   │
               │     SubmittedAt             │      │   │
               └─────────────────────────────┘      │   │
                          │ 1..*                     │   │
                          ▼                          │   │
          ┌───────────────────────────┐              │   │
          │   SurveyResponseDetails   │              │   │
          ├───────────────────────────┤              │   │
          │ PK  Id                    │              │   │
          │ FK  SurveyResponseId      │              │   │
          │ FK  QuestionId   ─────────┼──────┐       │   │
          │ FK  AnswerOptionId ───────┼──┐   │       │   │
          └───────────────────────────┘  │   │       │   │
                                         │   │       │   │
          ┌──────────────────────┐       │   │  ┌────┘   │
          │     AnswerOptions    │       │   │  │        │
          ├──────────────────────┤       │   │  │        │
          │ PK  Id  ◄────────────┼───────┘   │  │        │
          │ FK  QuestionId ──────┼───────┐   │  │        │
          │     OptionLabel      │       │   │  │        │
          │     OptionText       │       │   │  │        │
          └──────────────────────┘       │   │  │        │
                                         │   ▼  ▼        ▼
          ┌───────────────────────┐   ┌──────────┐  ┌──────────────────────┐
          │       Questions       │   │Questions │  │      Surveys         │
          ├───────────────────────┤   ├──────────┤  ├──────────────────────┤
          │ PK  Id  ◄─────────────┼───│ PK Id    │  │ PK  Id               │
          │ FK  SurveyId ─────────┼───┤ FK Survey│  │     Title            │
          │     QuestionText      │   │    Text  │  │     Description      │
          │     OrderNumber       │   │    Order │  │     IsActive         │
          │     CreatedAt         │   └──────────┘  │     CreatedAt        │
          └───────────────────────┘                  └──────────────────────┘
                                                              ▲
                                                              │ FK SurveyId
                                                         (Questions)

          ┌────────────────────────────┐
          │         Respondents        │
          ├────────────────────────────┤
          │ PK  Id                     │
          │     Name                   │
          │     Age                    │
          │     Gender                 │
          │     Phone (nullable)       │
          │     Occupation             │
          │     IncomeRange            │
          │     Location               │
          │     SurveyDate             │
          │     Notes (nullable)       │
          │     CreatedAt              │
          └────────────────────────────┘
```

---

## Table Definitions

### Outlets
| Column      | Type         | Constraints       |
|-------------|--------------|-------------------|
| Id          | INT          | PK, AUTO_INCREMENT|
| OutletName  | VARCHAR(150) | NOT NULL          |
| BranchCode  | VARCHAR(50)  | NOT NULL, UNIQUE  |
| Address     | VARCHAR(300) | NOT NULL          |
| City        | VARCHAR(100) | NOT NULL          |
| CreatedAt   | DATETIME     | DEFAULT NOW()     |

### Merchandisers
| Column           | Type         | Constraints           |
|------------------|--------------|-----------------------|
| Id               | INT          | PK, AUTO_INCREMENT    |
| Name             | VARCHAR(150) | NOT NULL              |
| Age              | INT          | NOT NULL              |
| EmployeeId       | VARCHAR(50)  | NOT NULL, UNIQUE      |
| Email            | VARCHAR(200) | NOT NULL, UNIQUE      |
| PasswordHash     | VARCHAR(300) | NOT NULL (BCrypt)     |
| AssignedOutletId | INT          | FK → Outlets, nullable|
| IsActive         | TINYINT(1)   | DEFAULT 1             |
| CreatedAt        | DATETIME     | DEFAULT NOW()         |

### Surveys
| Column      | Type         | Constraints       |
|-------------|--------------|-------------------|
| Id          | INT          | PK, AUTO_INCREMENT|
| Title       | VARCHAR(300) | NOT NULL          |
| Description | TEXT         | nullable          |
| IsActive    | TINYINT(1)   | DEFAULT 1         |
| CreatedAt   | DATETIME     | DEFAULT NOW()     |

### Questions
| Column       | Type    | Constraints            |
|--------------|---------|------------------------|
| Id           | INT     | PK, AUTO_INCREMENT     |
| SurveyId     | INT     | FK → Surveys, CASCADE  |
| QuestionText | TEXT    | NOT NULL               |
| OrderNumber  | INT     | DEFAULT 1              |
| CreatedAt    | DATETIME| DEFAULT NOW()          |

### AnswerOptions
| Column      | Type         | Constraints              |
|-------------|--------------|--------------------------|
| Id          | INT          | PK, AUTO_INCREMENT       |
| QuestionId  | INT          | FK → Questions, CASCADE  |
| OptionLabel | VARCHAR(10)  | NOT NULL (A, B, C…)      |
| OptionText  | VARCHAR(500) | NOT NULL                 |
| CreatedAt   | DATETIME     | DEFAULT NOW()            |

### Respondents
| Column      | Type         | Constraints       |
|-------------|--------------|-------------------|
| Id          | INT          | PK, AUTO_INCREMENT|
| Name        | VARCHAR(150) | NOT NULL          |
| Age         | INT          | NOT NULL          |
| Gender      | VARCHAR(20)  | NOT NULL          |
| Phone       | VARCHAR(30)  | nullable          |
| Occupation  | VARCHAR(150) | NOT NULL          |
| IncomeRange | VARCHAR(100) | NOT NULL          |
| Location    | VARCHAR(200) | NOT NULL          |
| SurveyDate  | DATE         | NOT NULL          |
| Notes       | TEXT         | nullable          |
| CreatedAt   | DATETIME     | DEFAULT NOW()     |

### SurveyResponses
| Column          | Type    | Constraints                 |
|-----------------|---------|-----------------------------|
| Id              | INT     | PK, AUTO_INCREMENT          |
| SurveyId        | INT     | FK → Surveys                |
| RespondentId    | INT     | FK → Respondents            |
| OutletId        | INT     | FK → Outlets                |
| MerchandiserId  | INT     | FK → Merchandisers          |
| SubmittedAt     | DATETIME| DEFAULT NOW()               |

### SurveyResponseDetails
| Column          | Type | Constraints                      |
|-----------------|------|----------------------------------|
| Id              | INT  | PK, AUTO_INCREMENT               |
| SurveyResponseId| INT  | FK → SurveyResponses, CASCADE    |
| QuestionId      | INT  | FK → Questions                   |
| AnswerOptionId  | INT  | FK → AnswerOptions               |

---

## Relationships Summary

| Relationship                                  | Type    |
|-----------------------------------------------|---------|
| Outlets → Merchandisers                       | 1 : *   |
| Outlets → SurveyResponses                    | 1 : *   |
| Merchandisers → SurveyResponses              | 1 : *   |
| Surveys → Questions                          | 1 : *   |
| Questions → AnswerOptions                    | 1 : *   |
| Respondents → SurveyResponses               | 1 : *   |
| SurveyResponses → SurveyResponseDetails     | 1 : *   |
| Questions → SurveyResponseDetails           | 1 : *   |
| AnswerOptions → SurveyResponseDetails       | 1 : *   |
