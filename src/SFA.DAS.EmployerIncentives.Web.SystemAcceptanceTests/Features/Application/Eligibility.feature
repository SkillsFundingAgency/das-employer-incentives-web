@employerIncentivesApi
Feature: ApplicationEligibility
	As an employer applying for the new apprentice grant
	I want to be made aware if there are no apprentices eligible for the grant
	So that I can stop my application

Scenario: An employer has no eligible apprentices
	Given an employer applying for a grant has no apprentices matching the eligibility requirement
	When the employer tries to make a grant application
	Then the employer is informed they cannot apply for the grant

Scenario: An employer has eligible apprentices
	Given an employer applying for a grant has apprentices matching the eligibility requirement
	When the employer tries to make a grant application
	Then the employer is asked to select the apprentice the grant is for

