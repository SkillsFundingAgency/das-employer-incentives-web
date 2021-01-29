@employerIncentivesApi
Feature: EmployerIncentivesHub
	As an employer
	I want to access the EI hub page
	So that all the incentive related tasks are available in one place

Scenario: An employer with a single legal entity visits the EI hub page
Given an employer has a single legal entity in their account
When the employer accesses the hub page
Then they are presented with a link to apply for the employer incentive
And they are presented with a link to view previous applications
And the back link goes to the manage apprenticeships page

Scenario: An employer with a multiple legal entities visits the EI hub page
Given an employer has a multiple legal entities in their account
When the employer accesses the hub page
Then they are presented with a link to apply for the employer incentive
And they are presented with a link to view previous applications
And the back link goes to the choose organisation page