# das-employer-incentives-web

## Requirements

* DotNet Core 3.1 and any supported IDE for DEV running.
* Azure Storage Account

## Local running

* In your Azure Storage Account, create a table called Configuration and add the following rows:

PartitionKey: LOCAL  
RowKey: SFA.DAS.EmployerIncentives.Web_1.0  
Data: Contents of   
https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-employer-incentives-web/SFA.DAS.EmployerIncentives.Web.json (private repo)  

Modify 

    "EmployerIncentivesApi": {
        "ApiBaseUrl": "http://localhost:8081/",

to http://localhost:8083 to run against the local mock server  
to https://localhost:5121 to run against the das-apim-endpoints repo running locally  

PartitionKey: LOCAL  
RowKey: SFA.DAS.Encoding_1.0  
Data: Contents of  
https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-shared-config/SFA.DAS.Encoding.json (private repo)  

Set the following projects as startup projects:

SFA.DAS.EmployerIncentives.Web  
SFA.DAS.EmployerIncentives.Web.MockServer  

For running the acceptance tests, create a JSON file in the SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests project called local.encoding.json.
Populate this with the contents of the JSON in the SFA.DAS.Encoding_1.0 RowKey

