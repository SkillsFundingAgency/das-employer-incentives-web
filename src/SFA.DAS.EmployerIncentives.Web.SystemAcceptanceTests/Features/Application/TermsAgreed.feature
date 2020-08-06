@employerIncentivesApi
Feature: TermsAgreed
	As an employer applying for an incentive
	I want to be told if I need to sign a new legal agreement
	
Scenario: An employer has signed an agreement with incentive terms
	Given an employer who has signed the incentive terms applying for a grant
	When the employer completes eligibility
	Then the employer is asked to select the apprenticeship

Scenario: An employer has not signed an agreement with incentive terms
	Given an employer who has not signed the incentive terms applying for a grant
	When the employer completes eligibility
	Then the employer is asked to sign a new legal agreement