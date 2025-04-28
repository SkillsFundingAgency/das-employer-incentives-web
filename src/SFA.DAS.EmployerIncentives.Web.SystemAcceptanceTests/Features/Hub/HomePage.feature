@ignore
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

Scenario: An employer with an application on a later agreement that needs signing is prompted to sign the agreement
	Given an employer with a later agreement version that needs signing
	When the employer is on the hub page
	Then the accept new employer agreement call to action is shown

Scenario: An employer with an application that is on the signed version is not prompted to sign the agreement
	Given an employer with an agreement version that has been signed
	When the employer is on the hub page
	Then the accept new employer agreement call to action is not shown