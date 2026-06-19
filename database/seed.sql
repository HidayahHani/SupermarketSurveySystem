-- ============================================================
-- Supermarket Survey System - Seed Data
-- Run after schema.sql
-- ============================================================
USE supermarket_survey;

-- ============================================================
-- OUTLETS
-- ============================================================
INSERT INTO Outlets (OutletName, BranchCode, Address, City) VALUES
('SuperMart Kuala Lumpur', 'SMC-001', 'No. 12, Jalan Ampang, 50450 Kuala Lumpur',         'Kuala Lumpur'),
('SuperMart Shah Alam',    'SMB-002', 'No. 45, Persiaran Kayangan, Seksyen 7, Shah Alam', 'Shah Alam'),
('SuperMart Petaling Jaya','SMD-003', 'Lot 88, Jalan SS2/72, 47300 Petaling Jaya',         'Petaling Jaya'),
('SuperMart Johor Bahru',  'SMT-004', 'No. 22, Jalan Abdul Samad, 80100 Johor Bahru',      'Johor Bahru'),
('SuperMart Pulau Pinang', 'SMO-005', 'No. 77, Jalan Burma, 10050 Georgetown, Pulau Pinang','Pulau Pinang');

-- ============================================================
-- MERCHANDISERS  (password = "password123", BCrypt hashed)
-- ============================================================
INSERT INTO Merchandisers (Name, Age, EmployeeId, Email, PasswordHash, AssignedOutletId) VALUES
('Ahmad Hafizi bin Razak',       28, 'EMP-001', 'ahmad.hafizi@supermart.my',  '$2b$12$bzm//bhuO.53Ljn4iyoMIeJgnSZgMA.n9i9oLJSpz7xoPp0frFasi', 1),
('Nurul Aina binti Ismail',      32, 'EMP-002', 'nurul.aina@supermart.my',    '$2b$12$bzm//bhuO.53Ljn4iyoMIeJgnSZgMA.n9i9oLJSpz7xoPp0frFasi', 2),
('Muhammad Syafiq bin Zulkifli', 25, 'EMP-003', 'syafiq@supermart.my',        '$2b$12$bzm//bhuO.53Ljn4iyoMIeJgnSZgMA.n9i9oLJSpz7xoPp0frFasi', 3),
('Siti Nabilah binti Hassan',    30, 'EMP-004', 'siti.nabilah@supermart.my',  '$2b$12$bzm//bhuO.53Ljn4iyoMIeJgnSZgMA.n9i9oLJSpz7xoPp0frFasi', 4),
('admin',                        35, 'EMP-000', 'admin@supermart.my',          '$2b$12$bzm//bhuO.53Ljn4iyoMIeJgnSZgMA.n9i9oLJSpz7xoPp0frFasi', 1);

-- ============================================================
-- SURVEYS
-- ============================================================
INSERT INTO Surveys (Title, Description) VALUES
('Customer Satisfaction Survey Q2 2024',
 'Monthly survey to measure customer satisfaction with supermarket services and products.'),
('Product Preference Survey',
 'Survey to understand product preferences and buying habits of supermarket customers.');

-- ============================================================
-- QUESTIONS - Survey 1
-- ============================================================
INSERT INTO Questions (SurveyId, QuestionText, OrderNumber) VALUES
(1, 'How satisfied are you with the overall shopping experience at our supermarket?', 1),
(1, 'How would you rate the cleanliness of our store?', 2),
(1, 'How satisfied are you with the availability of products on shelves?', 3),
(1, 'How would you rate the friendliness of our staff?', 4),
(1, 'How likely are you to recommend our supermarket to friends or family?', 5);

-- ============================================================
-- ANSWER OPTIONS - Survey 1
-- ============================================================
-- Q1
INSERT INTO AnswerOptions (QuestionId, OptionLabel, OptionText) VALUES
(1, 'A', 'Very Satisfied'),
(1, 'B', 'Satisfied'),
(1, 'C', 'Neutral'),
(1, 'D', 'Dissatisfied'),
(1, 'E', 'Very Dissatisfied');
-- Q2
INSERT INTO AnswerOptions (QuestionId, OptionLabel, OptionText) VALUES
(2, 'A', 'Excellent'),
(2, 'B', 'Good'),
(2, 'C', 'Fair'),
(2, 'D', 'Poor');
-- Q3
INSERT INTO AnswerOptions (QuestionId, OptionLabel, OptionText) VALUES
(3, 'A', 'Always available'),
(3, 'B', 'Usually available'),
(3, 'C', 'Sometimes out of stock'),
(3, 'D', 'Frequently out of stock');
-- Q4
INSERT INTO AnswerOptions (QuestionId, OptionLabel, OptionText) VALUES
(4, 'A', 'Excellent – very helpful and friendly'),
(4, 'B', 'Good – generally helpful'),
(4, 'C', 'Average – sometimes unhelpful'),
(4, 'D', 'Poor – unfriendly or unhelpful');
-- Q5
INSERT INTO AnswerOptions (QuestionId, OptionLabel, OptionText) VALUES
(5, 'A', 'Definitely yes'),
(5, 'B', 'Probably yes'),
(5, 'C', 'Not sure'),
(5, 'D', 'Probably not'),
(5, 'E', 'Definitely not');

-- ============================================================
-- QUESTIONS - Survey 2
-- ============================================================
INSERT INTO Questions (SurveyId, QuestionText, OrderNumber) VALUES
(2, 'Do you like to eat chocolate?', 1),
(2, 'How often do you visit the supermarket per week?', 2),
(2, 'What category of products do you purchase most frequently?', 3),
(2, 'Do you use our loyalty card / membership program?', 4);

-- ANSWER OPTIONS - Survey 2
-- Q6 (Do you like chocolate?)
INSERT INTO AnswerOptions (QuestionId, OptionLabel, OptionText) VALUES
(6, 'A', 'Yes'),
(6, 'B', 'No');
-- Q7 (Visit frequency)
INSERT INTO AnswerOptions (QuestionId, OptionLabel, OptionText) VALUES
(7, 'A', 'Every day'),
(7, 'B', '2-3 times a week'),
(7, 'C', 'Once a week'),
(7, 'D', 'Less than once a week');
-- Q8 (Product category)
INSERT INTO AnswerOptions (QuestionId, OptionLabel, OptionText) VALUES
(8, 'A', 'Fresh produce & vegetables'),
(8, 'B', 'Packaged food & snacks'),
(8, 'C', 'Beverages'),
(8, 'D', 'Household & cleaning products'),
(8, 'E', 'Personal care & health');
-- Q9 (Loyalty card)
INSERT INTO AnswerOptions (QuestionId, OptionLabel, OptionText) VALUES
(9, 'A', 'Yes, I use it regularly'),
(9, 'B', 'Yes, but I rarely use it'),
(9, 'C', 'No, but I am interested'),
(9, 'D', 'No, I am not interested');

-- ============================================================
-- SAMPLE RESPONDENT + RESPONSE
-- ============================================================
INSERT INTO Respondents (Name, Age, Gender, Phone, Occupation, IncomeRange, Location, SurveyDate, Notes) VALUES
('Farah Aishah binti Kamaruddin', 34, 'Female', '011-2345 6789', 'Suri Rumah',      'RM 3,001 - RM 5,000',  'Shah Alam',    '2024-06-01', 'Pelanggan tetap'),
('Rizwan bin Othman',             28, 'Male',   '012-8765 4321', 'Pekerja Swasta',  'RM 5,001 - RM 10,000', 'Kuala Lumpur', '2024-06-01', NULL),
('Khairunnisa binti Abdullah',    22, 'Female', NULL,             'Pelajar',         'Bawah RM 3,000',       'Petaling Jaya','2024-06-02', 'Responden baharu');

INSERT INTO SurveyResponses (SurveyId, RespondentId, OutletId, MerchandiserId) VALUES
(1, 1, 2, 2),
(1, 2, 1, 1),
(2, 3, 3, 3);

-- Response details for respondent 1 (Survey 1)
INSERT INTO SurveyResponseDetails (SurveyResponseId, QuestionId, AnswerOptionId) VALUES
(1, 1, 1),  -- Very Satisfied
(1, 2, 6),  -- Excellent
(1, 3, 9),  -- Always available
(1, 4, 13), -- Excellent staff
(1, 5, 16); -- Definitely yes

-- Response details for respondent 2 (Survey 1)
INSERT INTO SurveyResponseDetails (SurveyResponseId, QuestionId, AnswerOptionId) VALUES
(2, 1, 2),  -- Satisfied
(2, 2, 7),  -- Good
(2, 3, 10), -- Usually available
(2, 4, 14), -- Good staff
(2, 5, 17); -- Probably yes

-- Response details for respondent 3 (Survey 2)
INSERT INTO SurveyResponseDetails (SurveyResponseId, QuestionId, AnswerOptionId) VALUES
(3, 6, 21),  -- Yes (chocolate)
(3, 7, 23),  -- Once a week
(3, 8, 27),  -- Packaged food
(3, 9, 30);  -- Yes, rarely use loyalty card
