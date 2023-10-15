Feature: UIAutomationChallenge
	  	As a user
		I want to use Selenium
		Navigate to a url and perform actions on the webpage

Background:
	Given I initialize the Selenium Driver with parameters
		| hideCommandPromptWindow | headless | driverFilePath |
		| true                    | false    |                |
	When I navigate to url "https://www.bolttech.co.th/en/ascend/device-protection?utm_source=ascend"

Scenario: TC01 - Confirm device price
	Then I expand the Purchase Price dropdown
	And I stop the Selenium Driver
	
Scenario: TC02 - Choose a random device price
	Then I expand the Purchase Price dropdown
	And I click on device price "RANDOM"
	And I validate the combobox contains the chosen value
	And I stop the Selenium Driver
	
Scenario: TC03 - Choose a specific device price
	Then I expand the Purchase Price dropdown
	And I click on device price "THB 10,001 - 15,000"
	And I validate the combobox contains the chosen value
	And I stop the Selenium Driver
	
Scenario: TC04 - Validate dynamic price on table
	Then I expand the Purchase Price dropdown
	And I click on device price "THB 10,001 - 15,000"
	And I stop the Selenium Driver
	
Scenario: TC05 - Navigate to checkout
	Then I expand the Purchase Price dropdown
	And I click on device price "THB 10,001 - 15,000"
	And I click Select on the Plan Card
	And I stop the Selenium Driver
	
Scenario: TC06 - Validate Checkout page
	Then I expand the Purchase Price dropdown
	And I click on device price "THB 10,001 - 15,000"
	And I click Select on the Plan Card
	And I validate the page URL contains "/device-protection/checkout/payment"
	And I validate the utm_source was successfully carried from the previous page
	And I stop the Selenium Driver
	
Scenario: TC07 - Validate Checkout page content
	Then I expand the Purchase Price dropdown
	And I click on device price "THB 10,001 - 15,000"
	And I click Select on the Plan Card
	And I validate the plan price
	And I validate the Product Name
	And I validate the Provider is "bolttech"
	And I validate the Contract Start date
	And I validate the Contract Renewal is "Monthly"
	And I stop the Selenium Driver
	
Scenario: TC08 - Fill and Validate Payment Page - Valid Answers
	Then I expand the Purchase Price dropdown
	And I click on device price "THB 10,001 - 15,000"
	And I click Select on the Plan Card
	And I enter IMEI "123456789012345"
	And I answer the Questionnaire with "Yes" and "Yes" and ""
	And I validate that there is no error shown
	And I stop the Selenium Driver
	
Scenario: TC09 - Fill and Validate Payment Page - Valid Answers
	Then I expand the Purchase Price dropdown
	And I click on device price "THB 10,001 - 15,000"
	And I click Select on the Plan Card
	And I enter IMEI "123456789012345"
	And I answer the Questionnaire with "Yes" and "No" and "Yes"
	And I validate that there is no error shown
	And I stop the Selenium Driver
	
Scenario: TC10 - Fill and Validate Payment Page - Inalid Answers
	Then I expand the Purchase Price dropdown
	And I click on device price "THB 10,001 - 15,000"
	And I click Select on the Plan Card
	And I enter IMEI "123456789012345"
	And I answer the Questionnaire with "No" and "Yes" and ""
	And I validate that an error message is shown
	And I stop the Selenium Driver
	
Scenario: TC11 - Fill and Validate Payment Page - Inalid Answers
	Then I expand the Purchase Price dropdown
	And I click on device price "THB 10,001 - 15,000"
	And I click Select on the Plan Card
	And I enter IMEI "123456789012345"
	And I answer the Questionnaire with "Yes" and "No" and "No"
	And I validate that an error message is shown
	And I stop the Selenium Driver
