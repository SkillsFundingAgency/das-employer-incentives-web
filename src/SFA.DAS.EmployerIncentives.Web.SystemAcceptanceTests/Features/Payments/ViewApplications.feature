@employerIncentivesApi
Feature: ViewApplications
	As an employer applying for the new apprentice grant
	If I have previously submitted apprentice grant applications
	Then I want to view my submitted applications and the incentive amounts

Scenario: An employer has a single apprentice submitted application
	Given an employer has a single submitted application
	When the employer views their applications
	Then the employer is shown a single submitted application

Scenario: An employer had multiple apprentice submitted applications
	Given an employer has multiple submitted applications
	When the employer views their applications
	Then the employer is shown submitted applications

Scenario: An employer has no apprentice applications
Given an employer has no applications
	When the employer views their applications
	Then the employer is shown no applications

Scenario: An employer without bank details has a single apprentice submitted application
	Given an employer without bank details has a single submitted application
	When the employer views their applications
	Then the add bank details call to action is shown

Scenario: An employer with vrf rejected status has a single apprentice submitted application
	Given an employer with vrf rejected status has a single submitted application
	When the employer views their applications
	Then the add bank details call to action is shown

Scenario: An employer with accepted bank details has a single apprentice submitted application
	Given an employer with accepted bank details has a single submitted application
	When the employer views their applications
	Then the add bank details call to action is not shown