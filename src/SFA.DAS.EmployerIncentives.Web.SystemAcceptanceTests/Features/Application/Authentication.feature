@ignore

Feature: Authentication
	In order to prevent unauthoried access
	As a system
	I want to only allow authorised access

Scenario Outline: An unauthorised user is asked to log on
	Given a user of the system has not logged on
	When the user access the <url> page
	Then the user is asked to log on

  Examples:
    | url                                                                                       |
    | /login                                                                                    |
    | /VBKBLD/application-complete/fd0f5a2d-b45b-4a73-8750-ddd167b270c3                    |
    | /VBKBLD/apply                                                                        |
    | /VBKBLD/apply/MLP7DD/select-apprentices                                              |
    | /VBKBLD/apply/select-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3                |
    | /VBKBLD/apply/confirm-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3               |
    | /VBKBLD/apply/declaration/fd0f5a2d-b45b-4a73-8750-ddd167b270c3                       |
    | /VBKBLD/apply/MLP7DD/no-eligible-apprentices                                         |
    | /VBKBLD/apply/cannot-apply                                                           |
    | /VBKBLD/apply/MLP7DD/cannot-apply                                                    |
    | /VBKBLD/apply/MLP7DD/problem-with-service                                            |
    | /VBKBLD/apply/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/join-organisation                 |
    | /VBKBLD/apply/choose-organisation                                                    |
    | /VBKBLD/apply/MLP7DD/validate-terms-signed                                           |
    | /VBKBLD/apply/MLP7DD/eligible-apprentices                                            |
    | /VBKBLD/apply/MLP7DD/taken-on-new-apprentices                                        |
    | /VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/need-bank-details          |
    | /VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/add-bank-details           |
    | /VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/complete/application-saved |
    | /VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/change-bank-details        |
    | /VBKBLD/cancel/MLP7DD/cancel-application                                             |
    | /VBKBLD                                                                              |
    | /VBKBLD/MLP7DD                                                                       |
    | /VBKBLD/MLP7DD/hire-new-apprentice-payment                                           |
    | /VBKBLD/payments/payment-applications                                                |
    | /VBKBLD/payments/MLP7DD/payment-applications                                         |
    | /VBKBLD/payments/MLP7DD/no-applications                                              |
    | /VBKBLD/payments/choose-organisation                                                 |
    	