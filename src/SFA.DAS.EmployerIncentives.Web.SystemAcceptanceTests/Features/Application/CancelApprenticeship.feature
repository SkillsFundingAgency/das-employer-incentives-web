@employerIncentivesApi
Feature: CancelApprenticeship
	As an employer applying for the new apprentice grant
	If there are existing apprenticeships to be withdrawn
	I want to be able to select them

Scenario: An employer wants to cancel selected apprenticeships
	Given an employer applying for a grant has existing applied for apprenticeship incentives
	When the employer views the cancel apprenticeships page
	Then the employer is asked to select the apprenticeships to cancel

Scenario: An employer without applied for apprenticeships wants to cancel selected apprenticeships
	Given an employer applying for a grant has no existing applied for apprenticeship incentives
	When the employer views the cancel apprenticeships page
	Then the employer is redirected to the view applications page

Scenario: An employer has selected apprenticeships to cancel
	Given an employer applying for a grant has existing applied for apprenticeship incentives
	When the employer selects the apprenticeships to cancel
	Then the employer is asked to confirm the selected apprenticeships	

Scenario: An employer has not selected apprenticeships to cancel
	Given an employer applying for a grant has existing applied for apprenticeship incentives
	When the employer doesn't select any apprenticeships to cancel
	Then the employer is informed that they haven't selected any apprenticeships