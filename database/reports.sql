-- ============================================================
-- Supermarket Survey System - Report Queries
-- ============================================================
USE supermarket_survey;

-- ============================================================
-- REPORT 1: Survey Participation Summary
-- Shows total responses per survey, per outlet, per date range
-- ============================================================
SELECT
    s.Title                          AS SurveyTitle,
    o.OutletName                     AS Outlet,
    o.City                           AS City,
    m.Name                           AS Merchandiser,
    COUNT(sr.Id)                     AS TotalResponses,
    MIN(sr.SubmittedAt)              AS FirstSubmission,
    MAX(sr.SubmittedAt)              AS LastSubmission
FROM SurveyResponses sr
JOIN Surveys     s ON sr.SurveyId       = s.Id
JOIN Outlets     o ON sr.OutletId       = o.Id
JOIN Merchandisers m ON sr.MerchandiserId = m.Id
GROUP BY s.Id, s.Title, o.Id, o.OutletName, o.City, m.Id, m.Name
ORDER BY s.Title, o.OutletName;


-- ============================================================
-- REPORT 2: Question Answer Statistics
-- Shows answer distribution for each question in a survey
-- Usage: replace ? with the survey ID
-- ============================================================
SELECT
    q.OrderNumber                                    AS QuestionNo,
    q.QuestionText                                   AS Question,
    ao.OptionLabel                                   AS OptionLabel,
    ao.OptionText                                    AS OptionText,
    COUNT(srd.Id)                                    AS AnswerCount,
    ROUND(
        COUNT(srd.Id) * 100.0 /
        NULLIF((
            SELECT COUNT(*)
            FROM SurveyResponseDetails srd2
            WHERE srd2.QuestionId = q.Id
        ), 0),
        2
    )                                                AS Percentage
FROM Questions q
JOIN AnswerOptions          ao  ON ao.QuestionId      = q.Id
LEFT JOIN SurveyResponseDetails srd ON srd.AnswerOptionId = ao.Id
WHERE q.SurveyId = 1   -- << change survey ID here
GROUP BY q.Id, q.OrderNumber, q.QuestionText, ao.Id, ao.OptionLabel, ao.OptionText
ORDER BY q.OrderNumber, ao.OptionLabel;


-- ============================================================
-- REPORT 3 (Bonus): Respondent Demographics Summary
-- ============================================================
SELECT
    Gender,
    CASE
        WHEN Age BETWEEN 17 AND 24 THEN '17-24'
        WHEN Age BETWEEN 25 AND 34 THEN '25-34'
        WHEN Age BETWEEN 35 AND 44 THEN '35-44'
        WHEN Age BETWEEN 45 AND 54 THEN '45-54'
        ELSE '55+'
    END                AS AgeGroup,
    IncomeRange,
    Occupation,
    COUNT(*)           AS Count
FROM Respondents
GROUP BY Gender, AgeGroup, IncomeRange, Occupation
ORDER BY Count DESC;
