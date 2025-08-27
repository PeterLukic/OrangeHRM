Feature: OrangeHRM Failed Login
	As a user
	I want to log in to the OrangeHRM system
	So that I can access the HR management features

Background:
	Given I am on the OrangeHRM login page

@failedLogin
Scenario: Failed login
	When I enter username "Admin" and password "admin123"
	And I click the login button with failed locator