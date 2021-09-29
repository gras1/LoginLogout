Feature: UserControllerTests
	In order to buy products from an e-commerce website
	As a customer
	I need to be able to login and logout from my account

Scenario: Unauthorised application calls authenticate method of user controller
	Given I am unauthorised
	When I post a request to authenticate with a valid email address and password
	Then I expect to receive a 401 response

Scenario: Authorised application calls authenticate method of user controller with invalid email address
	Given I am authorised
	When I post a request to authenticate with an invalid email address and valid password
	Then I expect to receive a 400 response

Scenario: Authorised application calls authenticate method of user controller with invalid password
	Given I am authorised
	When I post a request to authenticate with a valid email address and an invalid password
	Then I expect to receive a 400 response

Scenario: Authorised application calls authenticate method of user controller
	Given I am authorised
	And The number of failed login attempts for user 1 is 0 and status is 2
	When I post a request to authenticate with a valid email address and a valid but incorrect password
	Then I expect to receive a 404 response
	And The number of failed login attempts for user 1 is 1
	And There is 1 failed login attempt audit record for user 1 where before status is 1 and after status is 1

