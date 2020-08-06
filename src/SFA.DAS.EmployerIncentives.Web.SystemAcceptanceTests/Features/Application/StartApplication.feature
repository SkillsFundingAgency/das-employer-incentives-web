@employerIncentivesApi
Feature: StartApplication
	As an employer applying for the new apprenticeship grant
	I want to be able to start an application
	So that I can get the apprenticeship grant

Scenario: An employer with a single organisation starts an application
	Given an employer with a single organisation wants to apply for a grant
	When the employer applies for the grant
	Then the employer is informed that they need to specify whether or not they have eligible apprenticeships

Scenario: An employer with a multiple organisations starts an application
	Given an employer with a multiple organisations wants to apply for a grant
	When the employer applies for the grant
	Then the employer is asked to choose the organisation

Scenario: An employer without any organisations starts an application
	Given an employer without any organisations wants to apply for a grant
	When the employer applies for the grant
	Then the employer is informed that they cannot apply