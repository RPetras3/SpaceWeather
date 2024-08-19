using System.Collections.Frozen;
using System.Globalization;
using System.Text.Json.Nodes;

namespace RPetras.SpaceWeatherLib;

/// <summary>
///     Integer location data
/// </summary>
/// <param name="Longitude">Longitude value</param>
/// <param name="Latitude">Latitude value</param>
public record Location(int Longitude, int Latitude);

/// <summary>
///     NOAA Aurora data
/// </summary>
/// <param name="ObservationTime">Observation UTC time</param>
/// <param name="ForecastTime">Forecast UTC time</param>
/// <param name="LocationData">Map of Location to aurora percentage</param>
public record NoaaAuroraData(
    DateTime ObservationTime,
    DateTime ForecastTime,
    FrozenDictionary<Location, int> LocationData)
{
    /// <summary>
    ///     NOAA Url for aurora data
    /// </summary>
    private const string NoaaUrl = "https://services.swpc.noaa.gov/json/ovation_aurora_latest.json";

    /// <summary>
    ///     Parse NOAA Aurora data from json text
    /// </summary>
    /// <param name="jsonText">Json text to parse</param>
    /// <returns>NoaaAuroraData instance</returns>
    /// <exception cref="ArgumentException">Thrown on invalid json data</exception>
    public static NoaaAuroraData Parse(string jsonText)
    {
        // Parse the JSON text into a JsonObject
        var json = JsonNode.Parse(jsonText) as JsonObject ??
                   throw new ArgumentException("Invalid NOAA json data", nameof(jsonText));

        // Extract and Parse the observation time
        if (!DateTime.TryParse(json["Observation Time"]?.ToString(), CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var observationTime))
            throw new ArgumentException("Invalid NOAA json observation time", nameof(jsonText));

        // Extract and Parse the forecast time
        if (!DateTime.TryParse(json["Forecast Time"]?.ToString(), CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var forecastTime))
            throw new ArgumentException("Invalid NOAA json forecast time", nameof(jsonText));

        // Extract and Parse the coordinates array
        var coordinates = json["coordinates"] as JsonArray ??
                          throw new ArgumentException("Invalid NOAA json coordinate array");
        var coordinatesDictionary = new Dictionary<Location, int>();
        foreach (var coordinate in coordinates.OfType<JsonArray>())
        {
            var data = coordinate.GetValues<int>().ToArray();
            if (data.Length != 3)
                throw new ArgumentException("Invalid NOAA json coordinate entry");
            coordinatesDictionary.Add(new Location(data[0], data[1]), data[2]);
        }

        // Returns a new instance of NoaaAuroraData with the data retrieved from NOAA
        return new NoaaAuroraData(observationTime, forecastTime, coordinatesDictionary.ToFrozenDictionary());
    }

    /// <summary>
    ///     Get the NOAA aurora data
    /// </summary>
    /// <param name="url">NOAA url or null for default</param>
    /// <returns>NoaaAuroraData instance</returns>
    public static async Task<NoaaAuroraData> GetAsync(string? url = null)
    {
        // Use default URL if none is provided
        url ??= NoaaUrl;

        // Create an HttpClient instance to send the request
        using var client = new HttpClient();
        HttpResponseMessage response;
        try
        {
            // Send the GET request and ensure the response is successful
            response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            // Throw an exception if there is an error fetching the data
            throw new InvalidOperationException("Error fetching NOAA data", e);
        }

        // Read the response content as a string
        var jsonText = await response.Content.ReadAsStringAsync();

        // Read data into NoaaAuroraData
        return Parse(jsonText);
    }
}