@employerIncentivesApi
Feature: ApplicationShutter
	As an employer applying for an apprenticeship payment after the phase 2 period has closed
	I want to be prevented from applying for the application
	So that I can get the correct apprenticeship grant

@applyApplicationShutterPage
Scenario Outline: An employer is prevented from applying for a new apprenticeship payment after the phase 2 period has closed
	Given the application is configured to prevent applications
	When the employer access the <url> page
	Then the employer is shown the application shutter page

Examples:
    | url                                                                                  |
    | /VBKBLD/application-complete/fd0f5a2d-b45b-4a73-8750-ddd167b270c3                    |
    | /VBKBLD/apply                                                                        |
    | /VBKBLD/apply/MLP7DD/select-apprentices                                              |
    | /VBKBLD/apply/select-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3                |
    | /VBKBLD/apply/confirm-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3               |
    | /VBKBLD/apply/declaration/fd0f5a2d-b45b-4a73-8750-ddd167b270c3                       |
    | /VBKBLD/apply/MLP7DD/no-eligible-apprentices                                         |
    | /VBKBLD/apply/MLP7DD/problem-with-service                                            |
    | /VBKBLD/apply/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/join-organisation                 |
    | /VBKBLD/apply/MLP7DD/validate-terms-signed                                           |
    | /VBKBLD/apply/MLP7DD/eligible-apprentices                                            |
    | /VBKBLD/apply/MLP7DD/taken-on-new-apprentices                                        |
    | /VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/need-bank-details          |
    | /VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/add-bank-details           |
    | /VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/complete/application-saved |
    | /VBKBLD/cancel/MLP7DD/cancel-application                                             |
    | /VBKBLD/MLP7DD                                                                       |
    
@applyApplicationShutterPage
Scenario: An employer is prevented from submitting a new apprenticeship application
	Given the application is configured to prevent applications
	When the employer submits an application for the new apprenticeship payment
	Then the employer is shown the application shutter page

@applyApplicationShutterPage
Scenario Outline: An employer is allowed to use some areas of the site after the phase 2 period has closed
	Given the application is configured to prevent applications
	When the employer access the <url> page
	Then the employer is not shown the application shutter page
    
Examples:
    | url                                                                                  |
    | /VBKBLD/apply/cannot-apply                                                           |
    | /VBKBLD/apply/choose-organisation                                                    |
    | /VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/change-bank-details        |
    | /error/403                                                                           |
    | /error/404                                                                           |
    | /error/500                                                                           |
    | /                                                                                    |
    | /login                                                                               |
    | /VBKBLD                                                                              |
    | /VBKBLD/MLP7DD/hire-new-apprentice-payment                                           |
    | /VBKBLD/payments/payment-applications                                                |
    | /VBKBLD/payments/MLP7DD/payment-applications                                         |
    | /VBKBLD/payments/MLP7DD/no-applications                                              |
    | /VBKBLD/payments/choose-organisation                                                 |

Scenario Outline: An employer is allowed to use all areas of the site before the phase 2 period has closed
	Given the application is configured to allow applications
	When the employer access the <url> page
	Then the employer is not shown the application shutter page
    
Examples:
    | url                                                                                  |
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
    | /error/403                                                                           |
    | /error/404                                                                           |
    | /error/500                                                                           |
    | /                                                                                    |
    | /login                                                                               |
    | /VBKBLD                                                                              |
    | /VBKBLD/MLP7DD                                                                       |
    | /signout                                                                             |
    | /signoutcleanup                                                                      |
    | /VBKBLD/MLP7DD/hire-new-apprentice-payment                                           |
    | /VBKBLD/payments/payment-applications                                                |
    | /VBKBLD/payments/MLP7DD/payment-applications                                         |
    | /VBKBLD/payments/MLP7DD/no-applications                                              |
    | /VBKBLD/payments/choose-organisation                                                 |
