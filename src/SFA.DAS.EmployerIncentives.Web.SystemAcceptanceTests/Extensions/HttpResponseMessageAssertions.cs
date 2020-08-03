using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
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

        public AndConstraint<HttpResponseMessageAssertions> HaveBackLink(string link, string because = "", params object[] becauseArgs)
        {
             Execute.Assertion
             .BecauseOf(because, becauseArgs)
             .ForCondition(!string.IsNullOrEmpty(link))
             .FailWith("Link to assert on not provided")
             .Then
             .Given(() => _document.DocumentElement.QuerySelector(".govuk-back-link").Attributes["href"].Value)
             .ForCondition(t => _document.DocumentElement.QuerySelector(".govuk-back-link").Attributes["href"].Value == link)
             .FailWith("Expected {context:DocumentElement} to contain {0} but found {1}",
                 _ => link, item => item);

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
