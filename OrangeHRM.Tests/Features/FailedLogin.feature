Feature: OrangeHRM Failed Login
	As a user
	I want to log in to the OrangeHRM system
	So that I can access the HR management features

Background:
	Given I am on the OrangeHRM login page

Scenario: Failed login with empty credentials
	When I enter username "" and password ""
	And I click the login button