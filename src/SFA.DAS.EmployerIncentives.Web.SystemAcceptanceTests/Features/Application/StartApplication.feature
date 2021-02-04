@employerIncentivesApi
Feature: StartApplication
	As an employer applying for the new apprenticeship grant
	I want to be able to start an application
	So that I can get the apprenticeship grant

Scenario: An employer with a selected organisation wants to apply
	Given an employer with a selected organisation wants to apply for a grant
	When the employer starts the application
	Then the employer is shown the start application information page

Scenario: An employer with a single organisation starts an application
	Given an employer with a single organisation wants to apply for a grant
	When the employer applies for the grant
	Then the employer is shown the EI hub

Scenario: An employer with a multiple organisations starts an application
	Given an employer with a multiple organisations wants to apply for a grant
	When the employer applies for the grant
	Then the employer is informed that they need to select an organisation
