# MVC Routes Reference — Supermarket Survey System

Base URL: `http://localhost:5000`

Authentication is cookie-based. All routes except `/Account/Login` require the user to be logged in — unauthenticated requests are redirected to `/Account/Login`.

---

## AccountController

### GET /Account/Login
Displays the merchandiser login form.

**Returns:** `Views/Account/Login.cshtml`

**Redirects to:** `/Dashboard/Index` if already authenticated.

---

### POST /Account/Login

Validates credentials and creates an authentication cookie.

**Form fields:**

| Field    | Required | Description        |
|----------|----------|--------------------|
| Email    | Yes      | Merchandiser email |
| Password | Yes      | Plain-text password (verified against BCrypt hash) |

**On success:** Redirect to `/Dashboard/Index`

**On failure:** Re-render login form with error message.

---

### GET /Account/Logout

Clears the session and signs out the authentication cookie.

**Redirects to:** `/Account/Login`

---

## DashboardController

### GET /Dashboard/Index  *(requires auth)*

Displays the home dashboard.

**ViewBag data:**

| Key           | Type | Description                    |
|---------------|------|--------------------------------|
| SurveyCount   | int  | Number of active surveys       |
| OutletCount   | int  | Total outlets                  |
| ResponseCount | int  | Total survey responses         |
| Surveys       | List | Active surveys for quick start |

**Returns:** `Views/Dashboard/Index.cshtml`

---

## SurveyController

Survey state is passed between steps using **server-side Session**:

| Session Key        | Type   | Set at step       |
|--------------------|--------|-------------------|
| `OutletId`         | int    | SelectOutlet POST |
| `OutletName`       | string | SelectOutlet POST |
| `SurveyId`         | int    | SelectSurvey POST |
| `SurveyTitle`      | string | SelectSurvey POST |
| `RespondentJson`   | string | Respondent POST   |
| `AnswersJson`      | string | Questions POST    |

---

### GET /Survey/SelectOutlet  *(requires auth)*

Lists all outlets for the merchandiser to choose from.

**Model:** `List<Outlet>`

**Returns:** `Views/Survey/SelectOutlet.cshtml`

---

### POST /Survey/SelectOutlet

Saves selected outlet to session, proceeds to survey selection.

**Form fields:** `outletId` (int), `outletName` (string)

**Redirects to:** `/Survey/SelectSurvey`

---

### GET /Survey/SelectSurvey  *(requires auth)*

Lists all active surveys.

**ViewBag:** `OutletName` (string)

**Requires session:** `OutletId`

**Returns:** `Views/Survey/SelectSurvey.cshtml`

---

### POST /Survey/SelectSurvey

Saves selected survey to session.

**Form fields:** `surveyId` (int), `surveyTitle` (string)

**Redirects to:** `/Survey/Respondent`

---

### GET /Survey/Respondent  *(requires auth)*

Displays the respondent details form.

**Model:** `RespondentViewModel` (empty)

**ViewBag:** `SurveyTitle`, `OutletName`

**Requires session:** `SurveyId`

**Returns:** `Views/Survey/Respondent.cshtml`

---

### POST /Survey/Respondent

Validates and saves respondent data to session.

**Model:** `RespondentViewModel`

| Field       | Required | Description                   |
|-------------|----------|-------------------------------|
| Name        | Yes      | Respondent full name          |
| Age         | Yes      | 1 – 120                       |
| Gender      | Yes      | Male / Female / Other         |
| Phone       | No       | Optional phone number         |
| Occupation  | Yes      | Selected from dropdown        |
| IncomeRange | Yes      | MYR range selected from list  |
| Location    | Yes      | Area / district               |
| SurveyDate  | Yes      | Date of survey (yyyy-MM-dd)   |
| Notes       | No       | Free-text observations        |

**On success:** Redirect to `/Survey/Questions`

**On failure:** Re-render form with validation errors.

---

### GET /Survey/Questions  *(requires auth)*

Loads the survey with all questions and answer options from the database.

**Model:** `QuestionnaireViewModel`

```csharp
public class QuestionnaireViewModel {
    public int SurveyId { get; set; }
    public string SurveyTitle { get; set; }
    public List<QuestionItemViewModel> Questions { get; set; }
}
```

**Requires session:** `SurveyId`

**Returns:** `Views/Survey/Questions.cshtml`

---

### POST /Survey/Questions

Receives all answers submitted from the single-page questionnaire form.

**Model:** `QuestionnaireViewModel`

Each question binds as:
```
Questions[0].Id               → Question ID
Questions[0].SelectedOptionId → Selected AnswerOption ID
```

**On success:** Saves answers JSON to session → redirect to `/Survey/Review`

**On validation fail (unanswered question):** Redirect back to `/Survey/Questions`

---

### GET /Survey/Review  *(requires auth)*

Builds a full summary from session data and database lookups.

**Model:** `ReviewViewModel`

```csharp
public class ReviewViewModel {
    public string SurveyTitle { get; set; }
    public string OutletName { get; set; }
    public string MerchandiserName { get; set; }
    public RespondentViewModel Respondent { get; set; }
    public List<ReviewAnswerItem> Answers { get; set; }
}
```

**Requires session:** `SurveyId`, `OutletId`, `RespondentJson`, `AnswersJson`

**Returns:** `Views/Survey/Review.cshtml`

---

### POST /Survey/Submit

Writes the complete survey response to the database.

**Database writes:**
1. Insert `Respondents` row
2. Insert `SurveyResponses` row (header)
3. Insert one `SurveyResponseDetails` row per answered question

**Clears from session:** `SurveyId`, `SurveyTitle`, `RespondentJson`, `AnswersJson`

**Keeps in session:** `OutletId`, `OutletName` (for convenience of next survey)

**Redirects to:** `/Survey/Success`

---

### GET /Survey/Success

Displays the submission confirmation screen.

**ViewBag:** `ResponseId`, `RespondentName`, `OutletName`

**Returns:** `Views/Survey/Success.cshtml`

---

## ReportsController

### GET /Reports/Index  *(requires auth)*

**Report 1 — Survey Participation Summary**

Groups survey responses by survey, outlet, and merchandiser.

**Model:** `IEnumerable<ParticipationReportViewModel>`

**Equivalent SQL:**
```sql
SELECT
    s.Title          AS SurveyTitle,
    o.OutletName,
    o.City,
    m.Name           AS MerchandiserName,
    COUNT(sr.Id)     AS TotalResponses,
    MIN(sr.SubmittedAt) AS FirstSubmission,
    MAX(sr.SubmittedAt) AS LastSubmission
FROM SurveyResponses sr
JOIN Surveys      s ON sr.SurveyId       = s.Id
JOIN Outlets      o ON sr.OutletId       = o.Id
JOIN Merchandisers m ON sr.MerchandiserId = m.Id
GROUP BY s.Id, s.Title, o.Id, o.OutletName, o.City, m.Id, m.Name
ORDER BY s.Title, o.OutletName;
```

**Returns:** `Views/Reports/Index.cshtml`

---

### GET /Reports/Statistics?surveyId={id}  *(requires auth)*

**Report 2 — Question Answer Statistics**

Shows answer count and percentage per option for every question in the selected survey.

**Query parameter:** `surveyId` (int, optional — shows picker if omitted)

**Model:** `StatisticsReportViewModel`

**Equivalent SQL:**
```sql
SELECT
    q.OrderNumber,
    q.QuestionText,
    ao.OptionLabel,
    ao.OptionText,
    COUNT(srd.Id)   AS AnswerCount,
    ROUND(
        COUNT(srd.Id) * 100.0 /
        NULLIF((
            SELECT COUNT(*) FROM SurveyResponseDetails srd2
            WHERE srd2.QuestionId = q.Id
        ), 0), 1
    )               AS Percentage
FROM Questions q
JOIN AnswerOptions ao ON ao.QuestionId = q.Id
LEFT JOIN SurveyResponseDetails srd ON srd.AnswerOptionId = ao.Id
    AND srd.SurveyResponseId IN (
        SELECT Id FROM SurveyResponses WHERE SurveyId = @surveyId
    )
WHERE q.SurveyId = @surveyId
GROUP BY q.Id, q.OrderNumber, q.QuestionText, ao.Id, ao.OptionLabel, ao.OptionText
ORDER BY q.OrderNumber, ao.OptionLabel;
```

**Returns:** `Views/Reports/Statistics.cshtml`
