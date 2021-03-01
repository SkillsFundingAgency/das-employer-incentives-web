@employerIncentivesApi
Feature: AmendBankDetails
	As an employer with an application for the new apprentice grant
	If I want to amend the bank details for my organisation
	Then I am shown the employer amend bank details journey
	
Scenario: An employer is amending their bank details
	Given an employer has submitted an application
	When the employer wishes to update their bank details
	Then the employer is shown the employer amend bank details journey
	And the employer can go back to the EI hub page