Feature: Authentication
	In order to prevent unauthoried access
	As a system
	I want to only allow authorised access

Scenario Outline: An unauthorised user is asked to log on
	Given a user of the system has not logged on
	When the user access the <url> page
	Then the user is asked to log on

  Examples:
    | url                                                                                         |
    | "/login"                                                                                    |
    | "/VBKBLD/apply"                                                                             |
    | "/VBKBLD/apply/choose-organisation"                                                         |
    | "/VBKBLD/apply/MLP7DD/validate-terms-signed"                                                |
    | "/VBKBLD/apply/MLP7DD/select-apprentices"                                                   |
    | "/VBKBLD/apply/MLP7DD/eligible-apprentices"                                             |
    | "/VBKBLD/apply/select-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3"                     |
    | "/VBKBLD/apply/confirm-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3"                    |
    | "/VBKBLD/apply/MLP7DD/eligible-apprentices"                                             |
    | "/VBKBLD/apply/declaration/fd0f5a2d-b45b-4a73-8750-ddd167b270c3"                            |
    | "/VBKBLD/apply/cannot-apply"                                                                |
    | "/VBKBLD/apply/cannot-apply-yet"                                                            |
    | "/VBKBLD/apply/bankdetails/fd0f5a2d-b45b-4a73-8750-ddd167b270c3"                            |
    | "/VBKBLD/apply/bankdetails/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/need-bank-details"          |
    | "/VBKBLD/apply/bankdetails/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/add-bank-details"           |
    | "/VBKBLD/apply/bankdetails/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/complete/need-bank-details" |

	