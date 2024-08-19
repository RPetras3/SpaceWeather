using RPetras.SpaceWeatherLib;

namespace RPetras.SpaceWeatherCLI;

/// <summary>
///     The Space Weather CLI application class
/// </summary>
public static class Program
{
    /// <summary>
    ///     The main method, which is the entry point of the application.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Main(string[] args)
    {
        // Display a welcome message
        Console.WriteLine("Welcome to the Space Weather CLI");

        // Print usage information upon request
        if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
        {
            Console.WriteLine(
                """
                Usage: SpaceWeatherCli <command> [options]

                Commands: 
                  aurora <lat> <lon>        Read aurora intensity at location
                """);

            Environment.Exit(0);
        }

        try
        {
            // Traverse the arguments for commands to run
            switch (args[0])
            {
                // Run the aurora prediction command for given location
                case "aurora":
                    DoAurora(args);
                    break;
                // Throw exception for unknown commands
                default:
                    throw new InvalidOperationException($"Unknown Command: {args[0]}");
            }
        }
        catch (InvalidOperationException e)
        {
            // Handle known exceptions and exit
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ResetColor();
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            // Handle unknown exceptions
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unhandled Exception: {e.Message}");
            Console.WriteLine(e);
            Console.ResetColor();
            throw;
        }
    }

    /// <summary>
    ///     Handle the aurora command from the CLI
    /// </summary>
    /// <param name="args">command line arguments passed</param>
    /// <exception cref="InvalidOperationException">Thrown on invalid arguments or NOAA data errors</exception>
    private static void DoAurora(string[] args)
    {
        // Ensure we have the correct number of arguments for this command
        if (args.Length != 3)
            throw new InvalidOperationException("Invalid parameters to aurora command.");

        // Ensure we have a proper latitude value
        if (!int.TryParse(args[1], out var latitude) || latitude < -90 || latitude > 90)
            throw new InvalidOperationException($"Invalid latitude: {args[1]}");

        // Ensure we have a proper longitude value
        if (!int.TryParse(args[2], out var longitude) || longitude < 0 || longitude > 359)
            throw new InvalidOperationException($"Invalid longitude: {args[2]}");

        // Retrieve the NOAA data from their server
        var data = NoaaAuroraData.GetAsync().Result;

        // Check the retrieved NOAA data for a valid aurora prediction value
        if (!data.LocationData.TryGetValue(new Location(longitude, latitude), out var aurora))
            throw new InvalidOperationException("Aurora data missing for specified location.");

        // Print out our aurora prediction results
        Console.WriteLine($"Aurora Forecast from {data.ForecastTime} is {aurora}");
    }
}