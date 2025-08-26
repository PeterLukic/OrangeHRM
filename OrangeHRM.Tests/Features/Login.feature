Feature: OrangeHRM Login
	As a user
	I want to log in to the OrangeHRM system
	So that I can access the HR management features

Background:
	Given I am on the OrangeHRM login page

Scenario: Successful login with valid credentials
	When I enter valid username and password
	And I click the login button
	#Then I should be logged in successfully

Scenario: Failed login with invalid credentials
	When I enter username "invalid_user" and password "invalid_password"
	And I click the login button
	#Then I should see an error message

#Scenario: Failed login with empty credentials
	#When I enter username "" and password ""
	#And I click the login button
	#Then I should see an error message for empty fields

#Scenario Outline: Login with different credentials
	#When I enter username "<username>" and password "<password>"
	#And I click the login button
	#Then the login should be "<result>"

#Examples:
	#| username | password    | result    |
	#| Admin    | admin123    | successful|
	#| admin    | admin123    | failed    |
	#| Admin    | wrongpass   | failed    |
	#| admin    | wrongpass   | failed    |