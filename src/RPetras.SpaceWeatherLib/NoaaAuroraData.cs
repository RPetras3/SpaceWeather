﻿using System.Collections.Frozen;
using System.Globalization;
using System.Text.Json.Nodes;

namespace RPetras.SpaceWeatherLib;

/// <summary>
/// Integer location data
/// </summary>
/// <param name="Longitude">Longitude value</param>
/// <param name="Latitude">Latitude value</param>
public record Location(int Longitude, int Latitude);

/// <summary>
/// NOAA Aurora data
/// </summary>
/// <param name="ObservationTime">Observation UTC time</param>
/// <param name="ForecastTime">Forecast UTC time</param>
/// <param name="LocationData">Map of Location to aurora percentage</param>
public record NoaaAuroraData(DateTime ObservationTime, DateTime ForecastTime, FrozenDictionary<Location, int> LocationData)
{
    /// <summary>
    /// NOAA Url for aurora data
    /// </summary>
    private const string NoaaUrl = "https://services.swpc.noaa.gov/json/ovation_aurora_latest.json";

    /// <summary>
    /// Parse NOAA Aurora data from json text
    /// </summary>
    /// <param name="jsonText">Json text to parse</param>
    /// <returns>NoaaAuroraData instance</returns>
    /// <exception cref="ArgumentException">Thrown on invalid json data</exception>
    public static NoaaAuroraData Parse(string jsonText)
    {
        var json = JsonNode.Parse(jsonText) as JsonObject ?? throw new ArgumentException("Invalid NOAA json data", nameof(jsonText));
        var observationTimeText = json["Observation Time"]?.ToString() ?? throw new ArgumentException("Invalid NOAA json observation time", nameof(jsonText));
        var forecastTimeText = json["Forecast Time"]?.ToString() ?? throw new ArgumentException("Invalid NOAA json forecast time", nameof(jsonText));
        var observationTime = DateTime.Parse(observationTimeText, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        var forecastTime = DateTime.Parse(forecastTimeText, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        var coordinates = json["coordinates"] as JsonArray ?? throw new ArgumentException("Invalid NOAA json coordinate array");
        var coordinatesDictionary = new Dictionary<Location, int>();
        foreach (var coordinate in coordinates.OfType<JsonArray>())
        {
            var data = coordinate.GetValues<int>().ToArray();
            if (data.Length != 3)
                throw new ArgumentException("Invalid NOAA json coordinate entry");
            coordinatesDictionary.Add(new Location(data[0], data[1]), data[2]);
        }

        return new NoaaAuroraData(observationTime, forecastTime, coordinatesDictionary.ToFrozenDictionary());
    }

    /// <summary>
    /// Get the NOAA aurora data
    /// </summary>
    /// <param name="url">NOAA url or null for default</param>
    /// <returns>NoaaAuroraData instance</returns>
    public static NoaaAuroraData Get(string? url = null)
    {
        url ??= NoaaUrl;
        using var client = new HttpClient();
        var response = client.GetAsync(url).Result;
        // Read data into NoaaAuroraData
        return Parse(response.Content.ReadAsStringAsync().Result);
    }
}