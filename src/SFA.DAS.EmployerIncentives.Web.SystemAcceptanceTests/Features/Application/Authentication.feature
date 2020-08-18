Feature: Authentication
	In order to prevent unauthoried access
	As a system
	I want to only allow authorised access

Scenario: An unauthorised user is asked to log on
	Given a user of the system has not logged on
	When the user access the home page
	Then the user is asked to log on