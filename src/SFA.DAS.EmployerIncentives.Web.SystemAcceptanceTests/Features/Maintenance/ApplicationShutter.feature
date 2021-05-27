@employerIncentivesApi
Feature: ApplicationShutter
	As an employer applying for a phase 1 apprenticeship payment in the phase 2 period
	I want to be prevented from applying for the application
	So that I can get the correct apprenticeship grant

@applyApplicationShutterPage
Scenario: An employer is prevented from applying for the a new apprenticeship payment
	Given the application is configured to prevent applications
	When the employer applies for the hire a new apprenticeship payment
	Then the employer is shown the application shutter page

Scenario: An employer is allowed to apply for the a new apprenticeship payment
	Given the application is configured to prevent applications
	When the employer applies for the hire a new apprenticeship payment
	Then the employer is shown the start page

@applyApplicationShutterPage
Scenario: An employer is prevented from submitting a new apprenticeship payment
	Given the application is configured to prevent applications
	When the employer submits an application for the new apprenticeship payment
	Then the employer is shown the application shutter page
