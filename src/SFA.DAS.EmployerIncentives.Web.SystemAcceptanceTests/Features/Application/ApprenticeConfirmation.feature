@employerIncentivesApi
Feature: ApprenticeConfirmation
	As an employer applying for the new apprentice grant
	If I have selected 2 eligible apprenticeships
	I want to be able to confirm the apprentices and expected amounts

Scenario: An employer is confirming selected apprenticeships
	Given an employer applying for a grant has already selected 2 eligible apprentices
	When the employer arrives on the confirm apprentices page
	Then the employer is asked to confirm the apprentices and expected amounts

Scenario: An employer has confirmed selected apprenticeships
	Given an employer applying for a grant has already selected 2 eligible apprentices
	When the employer confirms their selection
	Then the employer is asked to read and accept a declaration