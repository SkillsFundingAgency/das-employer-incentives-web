@employerIncentivesApi
Feature: PreApplication
	As an employer applying for the new apprentice grant
	Before I apply
	I want to see the pre application information
	So I can ensure I have all the information required during the application
	
Scenario: An employer is unable to start the application process
	Given the employer is on the hub page	
	Then the employer is not shown the Hire a new apprentice payment link

Scenario: An employer continues after viewing the before you start information
	Given the employer is on the before you start page
	When the employer selects the Start now button
	Then the employer is on the application information page

Scenario: An employer continues after viewing the application information
	Given the employer is on the application information page
	When the employer selects the Start now button
	Then the employer is on the eligible apprentices page