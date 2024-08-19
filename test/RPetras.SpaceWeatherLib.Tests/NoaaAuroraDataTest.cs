using System.Text.Json;

namespace RPetras.SpaceWeatherLib.Tests;

/// <summary>
///     Unit tests for the NoaaAuroraData class
/// </summary>
[TestClass]
public class NoaaAuroraDataTest
{
    /// <summary>
    ///     Test invalid json data
    /// </summary>
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

    /// <summary>
    ///     Test for incompatible json data
    /// </summary>
    [TestMethod]
    public void TestParseIncorrectType()
    {
        Assert.ThrowsException<ArgumentException>(() => NoaaAuroraData.Parse("[]"));
    }

    /// <summary>
    ///     Test for badly formatted observation time
    /// </summary>
    [TestMethod]
    public void TestParseInvalidObservationTime()
    {
        Assert.ThrowsException<ArgumentException>(
            () => NoaaAuroraData.Parse(
                """
                {
                  "Observation Time": "Invalid-Value",
                  "Forecast Time": "2024-08-18T19:07:00Z",
                  "coordinates": []
                }
                """));
    }

    /// <summary>
    ///     Test for badly formatted forecast time
    /// </summary>
    [TestMethod]
    public void TestParseInvalidForecastTime()
    {
        Assert.ThrowsException<ArgumentException>(
            () => NoaaAuroraData.Parse(
                """
                {
                  "Observation Time": "2024-08-18T18:02:00Z",
                  "Forecast Time": "Invalid-Value",
                  "coordinates": []
                }
                """));
    }

    /// <summary>
    ///     Test parsing of valid data
    /// </summary>
    [TestMethod]
    public void TestParse()
    {
        var data = NoaaAuroraData.Parse(
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
            """);
        Assert.AreEqual(2024, data.ObservationTime.Year);
        Assert.AreEqual(2024, data.ForecastTime.Year);
        Assert.AreEqual(7, data.LocationData.Count);
        Assert.IsTrue(data.LocationData.TryGetValue(new Location(0, -88), out var aurora));
        Assert.AreEqual(9, aurora);
    }

    /// <summary>
    ///     Test reading from official NOAA website
    /// </summary>
    [TestMethod]
    public void TestGet()
    {
        var data = NoaaAuroraData.GetAsync().Result;
        Assert.AreEqual(360 * 181, data.LocationData.Count);
    }
}