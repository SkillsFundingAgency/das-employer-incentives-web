@employerIncentivesApi
Feature: ApplicationConfirmation
	As an employer applying for the new apprentice grant
	If I have selected and confirmed apprenticeship selection
	I want to be able to view and accept the declaration

Scenario: An employer is viewing and accepting declaration
	Given an employer applying for a grant is asked to agree a declaration
	When the employer has not submitted bank details
	And the employer understands and confirms the declaration
	Then the employer application declaration is accepted
	And the employer is asked to enter bank details

Scenario: An employer is viewing and accepting a subsequent declaration
	Given an employer applying for a grant is asked to agree a declaration
	When the employer has previously submitted bank details
	And the employer understands and confirms the declaration
	Then the employer application declaration is accepted
	And the employer is shown confirmation that the application is complete