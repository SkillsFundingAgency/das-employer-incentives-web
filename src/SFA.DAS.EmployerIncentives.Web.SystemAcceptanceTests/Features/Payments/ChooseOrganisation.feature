@ignore
@employerIncentivesApi

Feature: ChooseOrganisation
	As an employer applying for the new apprentice grant
	If I have more than one organisation associated with my account
	Then I want to choose the organisation to view my submitted applications and the incentive amounts

Scenario: Account has more than one organisation
Given the employer account has more than one organisation
When viewing payments
Then the user is prompted to choose the organisation to view payments for

Scenario: Account has one organisation
Given the employer account has a single organisation
When viewing payments
Then the payments for that organisation are shown

