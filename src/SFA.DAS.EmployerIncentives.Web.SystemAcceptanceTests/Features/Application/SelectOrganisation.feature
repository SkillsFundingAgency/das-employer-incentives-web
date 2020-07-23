@employerIncentivesApi
Feature: SelectOrganisation
	As an employer applying for the new apprenticeship grant with more than one legal entity organisation
	I want to be able to select the leagl entity the application is for
	So that the correct legal entity is used in my application

Scenario: An employer selects a legal entity
	Given an employer applying for a grant has multiple legal entities
	When the employer selects the legal entity the application is for
	Then the employer is asked if they have taken on qualifying apprenticeships	

Scenario: An employer does not select a legal entity
	Given an employer applying for a grant has multiple legal entities
	When the employer does not select the legal entity the application is for
	Then the employer is informed that a legal entity needs to be selected
