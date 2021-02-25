@employerIncentivesApi
Feature: HomePage
	As an employer applying for the new apprenticeship grant
	I want to be able to start an application
	So that I can get the apprenticeship grant

Scenario: An employer with a single organisation visits the home page
	Given an employer with a single organisation wants to view the home page
	When the employer visits the home page
	Then the employer is shown the EI hub

Scenario: An employer with a multiple organisations visits the home page
	Given an employer with a multiple organisations wants to view the home page
	When the employer visits the home page
	Then the employer is informed that they need to select an organisation
