@ignore
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

Scenario: An employer views information about payment statuses
	Given an employer has multiple submitted applications
	When the employer views their applications
	Then the payment status help call to action is shown

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

Scenario: An employer with an application on a later agreement that needs signing is prompted to sign the agreement
	Given an employer with a later agreement version that needs signing
	When the employer views their applications
	Then the accept new employer agreement call to action is shown

Scenario: An employer with an application that is on the signed version is not prompted to sign the agreement
	Given an employer with an agreement version that has been signed
	When the employer views their applications
	Then the accept new employer agreement call to action is not shown

Scenario: An employer with an stopped application is shown the stopped status
	Given an employer with a stopped application
	When the employer views their applications
	Then the message showing the application is stopped is shown
	
Scenario: An employer with a clawed back payment is shown the payment withdrawn status
	Given an employer with an application with a clawed back payment
	When the employer views their applications
	Then the message showing the payment is reclaimed is shown

Scenario: An employer with a clawed back payment that has not been sent
	Given an employer with an application with a clawed back payment that has not been sent
	When the employer views their applications
	Then the message showing the payment is reclaimed is shown

Scenario: An employer with an application withdrawn by compliance is shown the rejected status
	Given an employer with an application withdrawn by compliance
	When the employer views their applications
	Then the message showing the application is rejected is shown

Scenario: An employer who has withdrawn an application is shown the rejected status
	Given an employer has withdrawn an application
	When the employer views their applications
	Then the message showing the application is cancelled is shown

Scenario: An employer who has an ineligible application due to a failed employment check
	Given an employer with an application with a failed employment check
	When the employer views their applications
	Then the message showing the application is ineligible is shown

Scenario: An employer with a completed employer incentive application
	Given an employer with a completed application
	When the employer views their applications
	Then the message showing the application is stopped is not shown