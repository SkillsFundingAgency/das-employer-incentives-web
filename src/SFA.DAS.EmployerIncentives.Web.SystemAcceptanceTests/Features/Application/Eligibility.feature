@employerIncentivesApi
Feature: ApplicationEligibility
	As an employer applying for the new apprenticeship grant
	I want to be made aware if there are no apprenticeships eligible for the grant
	So that I can stop my application

Scenario: An employer has no eligible apprentices
	Given an employer applying for a grant has no apprenticeships matching the eligibility requirement
	When the employer tries to make a grant application
	Then the employer is informed they cannot apply for the grant

Scenario: An employer has eligible apprentices
	Given an employer applying for a grant has apprenticeships matching the eligibility requirement
	When the employer tries to make a grant application
	Then the employer is asked if they have taken on qualifying apprenticeships

Scenario: An employer has multiple legal entities
	Given an employer applying for a grant has multiple legal entities
	When the employer tries to make a grant application
	Then the employer is asked to select the legal entity the grant applies to

Scenario: An employer has no legal entities
	Given an employer applying for a grant has no legal entities
	When the employer tries to make a grant application
	Then the employer is informed they cannot apply for the grant yet


