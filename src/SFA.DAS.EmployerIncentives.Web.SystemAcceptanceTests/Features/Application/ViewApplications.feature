﻿@employerIncentivesApi
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

Scenario: An employer has submitted and in progress apprentice applications
Given an employer has submitted and in progress applications
	When the employer views their applications
	Then the employer is shown only submitted applications

Scenario: An employer has only in progress apprentice applications
Given an employer has in progress applications
	When the employer views their applications
	Then the employer is shown no applications

Scenario: An employer has no apprentice applications
Given an employer has no applications
	When the employer views their applications
	Then the employer is shown no applications
