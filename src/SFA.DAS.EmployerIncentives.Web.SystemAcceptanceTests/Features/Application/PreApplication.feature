@employerIncentivesApi
Feature: PreApplication
	As an employer applying for the new apprentice grant
	Before I apply
	I want to see the pre application information
	So I can ensure I have all the information required during the application
	
Scenario: An employer is starting the application process
	Given the employer is on the hub page
	When the employer selects the Hire a new apprentice payment link
	Then the employer is on the before you start page

Scenario: An employer continues after viewing the before you start information
	Given the employer is on the before you start page
	When the employer selects the Start now button
	Then the employer is on the application information page

Scenario: An employer continues after viewing the application information
	Given the employer is on the application information page
	When the employer selects the Start now button
	Then the employer is on the eligible apprentices page