﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Features.Maintenance
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("ApplicationShutter")]
    [NUnit.Framework.CategoryAttribute("employerIncentivesApi")]
    public partial class ApplicationShutterFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "employerIncentivesApi"};
        
#line 1 "ApplicationShutter.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/Maintenance", "ApplicationShutter", "\tAs an employer applying for an apprenticeship payment after the phase 3 period h" +
                    "as closed\r\n\tI want to be prevented from applying for the application\r\n\tSo that I" +
                    " can get the correct apprenticeship grant", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("An employer is prevented from applying for a new apprenticeship payment after the" +
            " phase 3 period has closed")]
        [NUnit.Framework.CategoryAttribute("applyApplicationShutterPage")]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/application-complete/fd0f5a2d-b45b-4a73-8750-ddd167b270c3", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/select-apprentices", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/select-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/confirm-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/declaration/fd0f5a2d-b45b-4a73-8750-ddd167b270c3", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/no-eligible-apprentices", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/problem-with-service", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/join-organisation", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/validate-terms-signed", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/eligible-apprentices", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/taken-on-new-apprentices", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/MLP7DD", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/MLP7DD/before-you-start", null)]
        public void AnEmployerIsPreventedFromApplyingForANewApprenticeshipPaymentAfterThePhase3PeriodHasClosed(string url, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "applyApplicationShutterPage"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("url", url);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An employer is prevented from applying for a new apprenticeship payment after the" +
                    " phase 3 period has closed", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 8
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
 testRunner.Given("the application is configured to prevent applications", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 10
 testRunner.When(string.Format("the employer access the {0} page", url), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 11
 testRunner.Then("the employer is shown the application shutter page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("An employer is prevented from submitting a new apprenticeship application")]
        [NUnit.Framework.CategoryAttribute("applyApplicationShutterPage")]
        public void AnEmployerIsPreventedFromSubmittingANewApprenticeshipApplication()
        {
            string[] tagsOfScenario = new string[] {
                    "applyApplicationShutterPage"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An employer is prevented from submitting a new apprenticeship application", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 31
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 32
 testRunner.Given("the application is configured to prevent applications", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 33
 testRunner.When("the employer submits an application for the new apprenticeship payment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 34
 testRunner.Then("the employer is shown the application shutter page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("An employer is allowed to use some areas of the site after the phase 3 period has" +
            " closed")]
        [NUnit.Framework.CategoryAttribute("applyApplicationShutterPage")]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/cannot-apply", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/choose-organisation", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/need-bank-details", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/add-bank-details", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/complete/application-sa" +
            "ved", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/change-bank-details", null)]
        [NUnit.Framework.TestCaseAttribute("/error/403", null)]
        [NUnit.Framework.TestCaseAttribute("/error/404", null)]
        [NUnit.Framework.TestCaseAttribute("/error/500", null)]
        [NUnit.Framework.TestCaseAttribute("/", null)]
        [NUnit.Framework.TestCaseAttribute("/login", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/MLP7DD/hire-new-apprentice-payment", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/payments/payment-applications", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/payments/MLP7DD/payment-applications", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/payments/MLP7DD/no-applications", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/payments/choose-organisation", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/cancel/MLP7DD/cancel-application", null)]
        public void AnEmployerIsAllowedToUseSomeAreasOfTheSiteAfterThePhase3PeriodHasClosed(string url, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "applyApplicationShutterPage"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("url", url);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An employer is allowed to use some areas of the site after the phase 3 period has" +
                    " closed", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 37
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 38
 testRunner.Given("the application is configured to prevent applications", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 39
 testRunner.When(string.Format("the employer access the {0} page", url), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 40
 testRunner.Then("the employer is not shown the application shutter page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("An employer is allowed to use all areas of the site before the phase 3 period has" +
            " closed")]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/application-complete/fd0f5a2d-b45b-4a73-8750-ddd167b270c3", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/select-apprentices", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/select-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/confirm-apprentices/fd0f5a2d-b45b-4a73-8750-ddd167b270c3", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/declaration/fd0f5a2d-b45b-4a73-8750-ddd167b270c3", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/no-eligible-apprentices", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/cannot-apply", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/cannot-apply", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/problem-with-service", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/join-organisation", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/choose-organisation", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/validate-terms-signed", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/eligible-apprentices", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/apply/MLP7DD/taken-on-new-apprentices", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/need-bank-details", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/add-bank-details", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/complete/application-sa" +
            "ved", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/bank-details/fd0f5a2d-b45b-4a73-8750-ddd167b270c3/change-bank-details", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/cancel/MLP7DD/cancel-application", null)]
        [NUnit.Framework.TestCaseAttribute("/error/403", null)]
        [NUnit.Framework.TestCaseAttribute("/error/404", null)]
        [NUnit.Framework.TestCaseAttribute("/error/500", null)]
        [NUnit.Framework.TestCaseAttribute("/", null)]
        [NUnit.Framework.TestCaseAttribute("/login", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/MLP7DD", null)]
        [NUnit.Framework.TestCaseAttribute("/signout", null)]
        [NUnit.Framework.TestCaseAttribute("/signoutcleanup", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/MLP7DD/hire-new-apprentice-payment", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/payments/payment-applications", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/payments/MLP7DD/payment-applications", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/payments/MLP7DD/no-applications", null)]
        [NUnit.Framework.TestCaseAttribute("/VBKBLD/payments/choose-organisation", null)]
        public void AnEmployerIsAllowedToUseAllAreasOfTheSiteBeforeThePhase3PeriodHasClosed(string url, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("url", url);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An employer is allowed to use all areas of the site before the phase 3 period has" +
                    " closed", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 63
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 64
 testRunner.Given("the application is configured to allow applications", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 65
 testRunner.When(string.Format("the employer access the {0} page", url), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 66
 testRunner.Then("the employer is not shown the application shutter page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("An employer is not shown the apply link after the phase 3 period has closed")]
        [NUnit.Framework.CategoryAttribute("applyApplicationShutterPage")]
        public void AnEmployerIsNotShownTheApplyLinkAfterThePhase3PeriodHasClosed()
        {
            string[] tagsOfScenario = new string[] {
                    "applyApplicationShutterPage"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An employer is not shown the apply link after the phase 3 period has closed", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 106
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 107
 testRunner.Given("the application is configured to prevent applications", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 108
 testRunner.When("the employer is on the hub page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 109
 testRunner.Then("the employer is not shown the apply link", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 110
    testRunner.And("the employer is shown a link to the guidance page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 111
    testRunner.And("the heading text does not indicate that they can apply for incentive payments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
