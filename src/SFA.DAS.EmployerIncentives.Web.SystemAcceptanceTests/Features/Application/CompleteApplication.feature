@ignore
@employerIncentivesApi
Feature: CompleteApplication
As an employer applying for the new apprenticeship grant
	I want to receive confirmation that the application has completed
	So that I can complete the application journey

Scenario: An employer performs the entry of their bank details and completes the application
	Given given the employer has all the information required to process their bank details
	When the employer provides their bank details
	Then the employer completes their application journey