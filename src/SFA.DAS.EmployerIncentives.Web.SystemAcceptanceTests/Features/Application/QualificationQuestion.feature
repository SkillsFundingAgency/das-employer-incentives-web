@employerIncentivesApi
Feature: QualificationQuestion
	As an employer applying for the new apprenticeship grant
	I want to be able to specify if I have qualifying apprenticeships
	So that I am informed if I can apply for the grant

Scenario: An employer selects that they have qualifying apprenticeships
	Given an employer applying for a grant has qualifying apprenticeships
	When the employer specifies that they have qualifying apprenticeships
	Then the employer is asked to select the apprenticeship

Scenario: An employer selects that they do not have qualifying apprenticeships
	Given an employer applying for a grant does not have qualifying apprenticeships
	When the employer specifies that they do not have qualifying apprenticeships
	Then the employer is informed that they cannot apply

Scenario: An employer does not select whether or not they have qualifying apprenticeships
	Given an employer applying for a grant has qualifying apprenticeships
	When the employer does not specify whether or not they have qualifying apprenticeships
	Then the employer is informed that they need to specify whether or not they have qualifying apprenticeships
