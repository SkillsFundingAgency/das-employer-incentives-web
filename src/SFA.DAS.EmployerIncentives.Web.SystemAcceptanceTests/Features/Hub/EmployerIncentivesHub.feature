@employerIncentivesApi
Feature: EmployerIncentivesHub
	As an employer
	I want to access the EI hub page
	So that all the incentive related tasks are available in one place

Scenario: An employer with a single legal entity visits the EI hub page
Given an employer has a single legal entity in their account
And the employer has previously supplied their bank details
When the employer accesses the hub page
Then they can apply for the employer incentive
And they can view previous applications
And they are presented with a link to change their bank details
And the back link goes to the manage apprenticeships page

Scenario: An employer with a multiple legal entities visits the EI hub page
Given an employer has a multiple legal entities in their account
And the employer has previously supplied their bank details
When the employer accesses the hub page
Then they can apply for the employer incentive
And they can view previous applications
And the back link goes to the choose organisation page

Scenario: An employer with no submitted bank details visits the EI hub page
Given an employer has a single legal entity in their account
And the employer has not yet supplied bank details
When the employer accesses the hub page
Then they can apply for the employer incentive
And they can view previous applications
And they are prompted to enter their bank details