namespace AspTemplate.UnitTest;

using System.Text.Json;
using AspTemplate.Controllers;
using Microsoft.AspNetCore.Mvc;

public class MainControllerTest
{
    private const string TestCasesFilePath = "../../../Cases/MainController_GetPage.json";

    public class TestCase
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public List<int> Expected { get; set; }
    }

    // TestCaseSource provider
    public static IEnumerable<TestCaseData> GetPageTestCases()
    {
        var json = File.ReadAllText(TestCasesFilePath);
        var testCases = JsonSerializer.Deserialize<List<TestCase>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (testCases == null) yield break;

        foreach (var testCase in testCases)
        {
            yield return new TestCaseData(testCase.Page, testCase.Size, testCase.Expected);
        }
    }
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCaseSource(nameof(GetPageTestCases))]
    public void GetPage_ShouldReturnCorrectRange(int page, int size, List<int> expected)
    {
        // Arrange
        var controller = new MainController();
        var request = new OffsetPagingRequest
        {
            Page = page,
            Size = size
        };

        // Act
        var result = controller.GetPage(request) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null, "Expected an OkObjectResult.");
        Assert.That(result!.Value, Is.Not.Null, "Expected a non-null result value.");
        var range = result.Value as IEnumerable<int>;
        Assert.That(range, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(1, -3)]
    [TestCase(2, -5)]
    [TestCase(2, -8)]
    [TestCase(0, -1)]
    public void GetPage_ShouldHandleInvalidRequest(int page, int size)
    {
        // Arrange
        var controller = new MainController();
        var request = new OffsetPagingRequest
        {
            Page = page, // Invalid page
            Size = size // Invalid size
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.GetPage(request));
    }
    // [Test]
    // public void GetError_ShouldThrowFormatException()
    // {
    //     var controller = new MainController();
    //     Assert.Throws<FormatException>(() => controller.GetError());
    // }
}