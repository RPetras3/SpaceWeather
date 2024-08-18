using System.Text.Json;

namespace RPetras.SpaceWeatherLib.Tests;

[TestClass]
public class NoaaAuroraDataTest
{
    private const string TestData =
        """
        {
          "Observation Time": "2024-08-18T18:02:00Z",
          "Forecast Time": "2024-08-18T19:07:00Z",
          "Data Format": "[Longitude, Latitude, Aurora]",
          "coordinates": [
            [0, -90, 7],
            [0, -89, 0],
            [0, -88, 9],
            [0, -87, 11],
            [0, -86, 12],
            [0, -85, 12],
            [0, -84, 12]
          ]
        }
        """;

    [TestMethod]
    public void TestParseInvalid()
    {
        try
        {
            NoaaAuroraData.Parse("[");
            Assert.Fail("Exception not generated on invalid json data.");
        }
        catch (JsonException)
        {
            // Expected exception detected
        }
    }

    [TestMethod]
    public void TestParseIncorrectType()
    {
        Assert.ThrowsException<ArgumentException>(() => NoaaAuroraData.Parse("[]"));
    }

    [TestMethod]
    public void TestParse()
    {
        var data = NoaaAuroraData.Parse(TestData);
        Assert.AreEqual(2024, data.ObservationTime.Year);
        Assert.AreEqual(2024, data.ForecastTime.Year);
        Assert.AreEqual(7, data.LocationData.Count);
        Assert.IsTrue(data.LocationData.TryGetValue(new Location(0,-88), out var aurora));
        Assert.AreEqual(9, aurora);
    }

    [TestMethod]
    public void TestGet()
    {
        var data = NoaaAuroraData.Get();
        Assert.AreEqual(360*181,data.LocationData.Count);
    }
}