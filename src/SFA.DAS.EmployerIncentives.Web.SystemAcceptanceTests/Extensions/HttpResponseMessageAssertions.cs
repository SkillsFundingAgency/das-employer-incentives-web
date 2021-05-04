using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System.Linq;
using System.Net.Http;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Extensions
{
    public class HttpResponseMessageAssertions : ReferenceTypeAssertions<HttpResponseMessage, HttpResponseMessageAssertions>
    {
        private readonly IHtmlDocument _document;
        public HttpResponseMessageAssertions(HttpResponseMessage instance)
        {
            var parser = new HtmlParser();
            _document = parser.ParseDocument(instance.Content.ReadAsStringAsync().Result);

            Subject = instance;
        }

        protected override string Identifier => "HttpResponseMessage";

        public AndConstraint<HttpResponseMessageAssertions> HaveTitle(string title, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
             .BecauseOf(because, becauseArgs)
             .ForCondition(!string.IsNullOrEmpty(title))
             .FailWith("Title to assert on not provided")
             .Then
             .Given(() => _document.Title)
             .ForCondition(t => t.Equals(title))
             .FailWith("Expected {context:Title} to contain {0} but found {1}",
                 _ => title, item => item);

            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        public AndConstraint<HttpResponseMessageAssertions> HaveButton(string buttonText, string label, string because = "", params object[] becauseArgs)
        {
            var selector = $"button[type=submit]:contains('{buttonText}')";
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(!string.IsNullOrEmpty(buttonText))
                .FailWith("Button to assert on not provided")
                .Then
                .Given(() => _document.DocumentElement.QuerySelector(selector).Attributes["aria-label"].Value)
                .ForCondition(t => _document.DocumentElement.QuerySelector(selector).Attributes["aria-label"].Value == label)
                .FailWith("Expected {context:DocumentElement} to contain {0} but found {1}",
                    _ => label, item => item);

            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        public AndConstraint<HttpResponseMessageAssertions> HaveBackLink(string link, string because = "", params object[] becauseArgs)
        {
            return HaveLink(".govuk-back-link", link, because, becauseArgs);
        }

        public AndConstraint<HttpResponseMessageAssertions> HaveLink(string selector, string link, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrEmpty(link))
            .FailWith("Link to assert on not provided")
            .Then
            .Given(() => _document.DocumentElement.QuerySelector(selector).Attributes["href"].Value)
            .ForCondition(t => _document.DocumentElement.QuerySelector(selector).Attributes["href"].Value == link)
            .FailWith("Expected {context:DocumentElement} to contain {0} but found {1}",
                _ => link, item => item);

            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }
        
        public AndConstraint<HttpResponseMessageAssertions> NotHaveLink(string link, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrEmpty(link))
            .FailWith("Link to assert on not provided")
            .Then
            .Given(() => _document.DocumentElement.QuerySelectorAll(".govuk-link").Select(i => i.Attributes["href"].Value))
            .ForCondition(t => !_document.DocumentElement.QuerySelectorAll(".govuk-link").Select(i => i.Attributes["href"].Value).Contains(link))
            .FailWith("Expected {context:DocumentElement} to not contain {0} link but one was found", _ => link);

            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        public AndConstraint<HttpResponseMessageAssertions> HavePathAndQuery(string pathAndQuery, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
             .BecauseOf(because, becauseArgs)
             .ForCondition(!string.IsNullOrEmpty(pathAndQuery))
             .FailWith("PathAndQuery to assert on not provided")
             .Then
             .Given(() => Subject.RequestMessage.RequestUri.PathAndQuery)
             .ForCondition(t => t.Equals(pathAndQuery))
             .FailWith("Expected {context:PathAndQuery} to contain {0} but found {1}",
                 _ => pathAndQuery, item => item);

            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }
    }
}
