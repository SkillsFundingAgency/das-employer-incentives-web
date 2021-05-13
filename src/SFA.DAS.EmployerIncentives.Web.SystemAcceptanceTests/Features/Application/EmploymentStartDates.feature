@employerIncentivesApi
Feature: EmploymentStartDates
	As an apprenticeship service 
	I want employers to confirm that their apprentices are eligible for the incentive by supplying their employment start dates
	So that employer only apply for eligible apprentices

Scenario: An employer is prompted to supply employment start dates for the apprentices included in the application
	Given an initial application has been created
	When the employer has selected apprentices for the application
	Then the employer is asked to supply employment start dates for the selected apprentices

Scenario: An employer provides valid employment start dates for the apprentices included in the application
	Given an initial application has been created
	When the employer has selected apprentices for the application
	And the employer supplies valid start dates for the selected apprentices
	Then the employer is asked to confirm their selected apprentices

Scenario: An employer provides invalid employment start dates for the apprentices included in the application
	Given an initial application has been created
	When the employer has selected apprentices for the application
	And the employer supplies invalid start dates for the selected apprentices
	Then the employer is asked to change their submitted dates
