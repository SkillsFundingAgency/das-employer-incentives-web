@ignore
@employerIncentivesApi
Feature: ReadyToEnterBankDetails
	As an employer applying for the new apprentice grant
	I want to confirm whether I can provide my bank details now
	So that I can proceed with the application or come back at a later date


Scenario: An employer has confirmed their apprenticeship details
	When the employer has not previously supplied bank details
	And the employer has confirmed their apprenticeship details
	Then the employer is asked whether they can provide their organisation's bank details now

Scenario: An employer confirms they can provide their bank details
	When the employer confirms they can provide their bank details
	Then the employer is requested to enter their bank details
	And the employer is sent an email with details of how to enter their bank details in case they are unable to complete the journey

Scenario: An employer cannot provide their bank details now
	When the employer states that they are unable to provide bank details now
	Then the employer is requested to enter their bank details at a later date
	And the employer is sent an email reminding them to supply their bank details to complete the application

Scenario: An employer does not confirm whether they can provide bank details
	When the employer does not confirm whether they can provide bank details now
	Then the employer is prompted to confirm with an answer

Scenario: Any employer has supplied bank details in a previous application
	When the employer has already provided bank details
	And the employer has confirmed their apprenticeship details
	Then the employer is shown the application complete page
