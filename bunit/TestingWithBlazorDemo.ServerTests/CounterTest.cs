using Xunit;
using Bunit;
using TestingWithBlazorDemo.Server.Pages;

namespace TestingWithBlazorDemo.Tests
{
  public class CounterTest
  {
    [Fact]
    public void CounterComponentRendersCorrectly()
    {
      // Arrange
      using var ctx = new TestContext();

      // Act
      var cut = ctx.RenderComponent<Counter>();

      // Assert
      cut.MarkupMatches("<h1>Counter</h1>\n<p role=\"status\">Current count: 0</p>\n<button class=\"btn btn-primary\" >Click me</button>");
    }

    [Fact]
    public void CounterComponentRendersCorrectlyAfterClicks()
    {
      // Arrange
      using var ctx = new TestContext();

      // Act
      var cut = ctx.RenderComponent<Counter>();

      // Assert
      cut.MarkupMatches("<h1>Counter</h1>\n<p role=\"status\">Current count: 0</p>\n<button class=\"btn btn-primary\" >Click me</button>");

      var buttonElement = cut.Find("button");
      buttonElement.Click();

      cut.MarkupMatches("<h1>Counter</h1>\n<p role=\"status\">Current count: 1</p>\n<button class=\"btn btn-primary\" >Click me</button>");

      buttonElement.Click();
      buttonElement.Click();

      cut.MarkupMatches("<h1>Counter</h1>\n<p role=\"status\">Current count: 3</p>\n<button class=\"btn btn-primary\" >Click me</button>");
    }

    public void CounterComponentRendersCorrectlyAfterClicksAndReset()
    {
      // Arrange
      using var ctx = new TestContext();

      // Act
      var cut = ctx.RenderComponent<Counter>();

      // Assert
      cut.MarkupMatches("<h1>Counter</h1>\n<p role=\"status\">Current count: 0</p>\n<button class=\"btn btn-primary\" >Click me</button>");

      var buttonElement = cut.Find("button");
      buttonElement.Click();

      cut.MarkupMatches("<h1>Counter</h1>\n<p role=\"status\">Current count: 1</p>\n<button class=\"btn btn-primary\" >Click me</button>");

      cut.Instance.ResetCount();

      cut.MarkupMatches("<h1>Counter</h1>\n<p role=\"status\">Current count: 0</p>\n<button class=\"btn btn-primary\" >Click me</button>");
    }
  }
}