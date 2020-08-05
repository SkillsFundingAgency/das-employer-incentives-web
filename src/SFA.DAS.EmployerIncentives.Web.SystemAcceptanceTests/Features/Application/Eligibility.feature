@employerIncentivesApi
Feature: Eligibility
	As an employer applying for the new apprenticeship grant
	I want to be able to specify if I have eligible apprenticeships
	So that I am informed if I can apply for the grant

Scenario: An employer with a single organisation selects that they have eligible apprenticeships
	Given an employer with a single organisation applying for a grant has eligible apprenticeships
	When the employer specifies that they have eligible apprenticeships
	Then the employer is asked to select the apprenticeship

Scenario: An employer with a single organisation without any eligible apprenticeships
	Given an employer with a single organisation applying for a grant has no eligible apprenticeships
	When the employer specifies that they have eligible apprenticeships
	Then the employer is informed that they cannot apply

Scenario: An employer selects that they do not have eligible apprenticeships
	Given an employer applying for a grant does not have eligible apprenticeships
	When the employer specifies that they do not have eligible apprenticeships
	Then the employer is informed that they cannot apply yet

Scenario: An employer selects that they have eligible apprenticeships when they do not
	Given an employer applying for a grant does not have eligible apprenticeships
	When the employer specifies that they have eligible apprenticeships
	Then the employer is informed that they cannot apply

Scenario: An employer does not select whether or not they have eligible apprenticeships
	Given an employer applying for a grant has eligible apprenticeships
	When the employer does not specify whether or not they have eligible apprenticeships
	Then the employer is informed that they need to specify whether or not they have eligible apprenticeships
