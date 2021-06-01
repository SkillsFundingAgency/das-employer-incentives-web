@employerIncentivesApi
Feature: ApprenticeSelection
	As an employer applying for the new apprentice grant
	If there are apprenticeships eligible for the grant
	I want to be able to select them

Scenario: An employer has selected apprenticeships
	Given an employer applying for a grant has apprentices matching the eligibility requirement
	When the employer selects the apprentice the grant applies to
	Then the employer is asked to provide employment start dates for the apprentices
	
Scenario: An employer has not selected apprenticeships
	Given an employer applying for a grant has apprentices matching the eligibility requirement
	When the employer doesn't select any apprentice the grant applies to
	Then the employer is informed that they haven't selected an apprentice

Scenario: An employer has additional apprenticeships to show
	Given an employer applying for a grant has more apprentices than can be displayed on one page
	When there are more apprentices to show
	Then the employer is offered the choice of viewing more apprentices

Scenario: An employer has no additional apprenticeships to show and has viewed previous apprenticeships
	Given an employer applying for a grant has more apprentices than can be displayed on one page
	When there are no more apprentices to show
	And the employer has viewed more apprentices
	Then the employer is offered the choice of viewing previous apprentices	

Scenario: An employer has additional apprenticeships to show and has viewed previous apprenticeships
	Given an employer applying for a grant has more apprentices than can be displayed on one page
	When the employer has viewed more apprentices
	And there are more apprentices to show
	Then the employer is offered the choice of viewing previous apprentices	
	And the employer is offered the choice of viewing more apprentices