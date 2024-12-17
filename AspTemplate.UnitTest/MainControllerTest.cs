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
    private List<TestCase> LoadTestCases()
    {
        // Read and deserialize the JSON file
        var json = File.ReadAllText(TestCasesFilePath);
        return JsonSerializer.Deserialize<List<TestCase>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<TestCase>();
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase(1, 3, new[] { 0, 1, 2 })]
    [TestCase(2, 5, new[] { 5, 6, 7, 8, 9 })]
    [TestCase(3, 4, new[] { 8, 9, 10, 11 })]
    [TestCase(1, 0, new int[0])]
    public void GetPage_ShouldReturnCorrectRange(int page, int size, int[] expected)
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