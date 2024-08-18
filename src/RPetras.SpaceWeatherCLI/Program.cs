using RPetras.SpaceWeatherLib;

namespace RPetras.SpaceWeatherCLI;

public static class Program
{
    public static void Main(string[] args)
    {
        // Print usage information upon request
        if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
        {
            Console.WriteLine(
                """
                Space Weather CLI
                
                Usage: SpaceWeatherCli <command> [options]
                
                Commands: 
                  aurora <lat> <lon>        Read aurora intensity at location
                """);

            Environment.Exit(0);
        }

        try
        {
            switch (args[0])
            {
                case "aurora":
                    DoAurora(args);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown Command: {args[0]}");
            }
        }
        catch (InvalidOperationException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ResetColor();
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unhandled Exception: {e.Message}");
            Console.WriteLine(e);
            Console.ResetColor();
            throw;
        }


    }

    private static void DoAurora(string[] args)
    {
        if (args.Length != 3)
            throw new InvalidOperationException("Invalid parameters to aurora command.");
        if (!int.TryParse(args[1], out var latitude) || latitude < -90 || latitude > 90)
            throw new InvalidOperationException($"Invalid latitude: {args[1]}");
        if (!int.TryParse(args[2], out var longitude) || longitude < 0 || longitude > 359)
            throw new InvalidOperationException($"Invalid longitude: {args[2]}");
        var data = NoaaAuroraData.Get();
        if (!data.LocationData.TryGetValue(new Location(longitude, latitude), out var aurora))
            throw new InvalidOperationException("Aurora data missing for specified location.");
        Console.WriteLine($"Aurora Forecast from {data.ForecastTime} is {aurora}");
    }
}