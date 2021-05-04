@employerIncentivesApi
Feature: AmendApprenticeSelection
	As an employer applying for the new apprentice grant
	If I choose to amend my previously selected eligible apprenticeships
	I want to see the these apprentices checked 
	And be able to change my previous selection
	
Scenario: An employer is changing the previously selected apprenticeships
	Given there are eligible apprenticeships for the grant
	And a initial application has been created
	When the employer returns to the select apprentices page
	Then the employer can see the previous apprentices checked

Scenario: An employer is changing the previously selected apprenticeships, but it contains an additional apprentice
	Given there are eligible apprenticeships for the grant
	And a initial application has been created and it includes an apprentice who is no longer eligible
	When the employer returns to the select apprentices page
	Then the employer can see the previous apprentices checked
	But the additional apprentice is not on the list

Scenario: An employer is changing the previously selected apprenticeships and submits the application without any apprentices selected
	Given there are eligible apprenticeships for the grant
	When the employer updates application with no apprentices selected
	Then the employer will receive an error

Scenario: An employer is changing the previously selected apprenticeships and submits the application with apprentices selected
	Given there are eligible apprenticeships for the grant
	And a initial application has been created
	When the employer updates application with apprentices selected
	Then the user is directed to the confirmation page
