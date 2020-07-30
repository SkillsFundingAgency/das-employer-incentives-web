@employerIncentivesApi
Feature: ApprenticeSelection
	As an employer applying for the new apprentice grant
	If there are apprenticeships eligible for the grant
	I want to be able to select them

Scenario: An employer has selected apprenticeships
	Given an employer applying for a grant has apprentices matching the eligibility requirement
	When the employer selects the apprentice the grant applies to
	Then the employer is asked to confirm the selected apprentices
	
Scenario: An employer has not selected apprenticeships
	Given an employer applying for a grant has apprentices matching the eligibility requirement
	When the employer doesn't select any apprentice the grant applies to
	Then the employer is informed that they haven't selected an apprentice
