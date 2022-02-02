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
	And the employer has selected apprentices for the application
	When the employer supplies valid start dates for the selected apprentices
	Then the employer is asked to confirm their selected apprentices	

Scenario: An employer provides invalid employment start dates for the apprentices included in the application
	Given an initial application has been created
	And the employer has selected apprentices for the application
	When the employer supplies invalid start dates for the selected apprentices
	Then the employer is asked to change their submitted dates

Scenario: An employer provides some ineligible employment start dates for the apprentices included in the application
	Given an initial application has been created
	And the employer has selected apprentices for the application
	When the employer supplies some ineligible start dates for the selected apprentices
	Then the employer is informed one more of their selected apprentices are ineligible
	And the employer is offered the option to change their employment start dates

Scenario: An employer provides all ineligible employment start dates for the apprentices included in the application
	Given an initial application has been created
	And the employer has selected apprentices for the application
	When the employer supplies all ineligible start dates for the selected apprentices
	Then the employer is informed all of their selected apprentices are ineligible
	And the employer is offered the option to change their employment start dates

Scenario: An employer accepts to remove ineligible apprenticeships from the application
	Given an initial application has been created
	And the employer has selected apprentices for the application
	And the employer is informed one or more of their selected apprentices are ineligible
	When the employer accepts to remove ineligible apprenticeships from the application
	Then the employer is asked to confirm their apprenticeships selection

Scenario: An employer provides supplies employment start dates that fall into next phase window
	Given an initial application has been created
	And the employer has selected apprentices for the application
	When the employer supplied employment start dates fall into next phase window
	Then the employer is asked to sign the agreement variation
