Feature: CompleteApplication
As an employer applying for the new apprenticeship grant
	I want to receive confirmation that the application has completed
	So that I can complete the application journey

Scenario: An employer performs the entry of their bank details and completes the application
	Given the employer has entered all the information required to process their bank details
	When the employer is shown the confirmation page
	Then the employer has the option to return to their accounts page