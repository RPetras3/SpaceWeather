namespace RPetras.SpaceWeatherLib.Tests;

[TestClass]
public class ProgramAuroraTest
{
    [TestMethod]
    public void TestAuroraCommand()
    {
        // Run the command line application with bad argument
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "RPetras.SpaceWeatherCLI.dll",
            "aurora",
            "45",
            "45");

        // Verify program succeeded
        Assert.AreEqual(0, exitCode);
        Assert.IsTrue(output.Contains("Aurora Forecast from"));
    }

    [TestMethod]
    public void TestAuroraCommandMissingArguments()
    {
        // Run the command line application with no arguments for aurora command
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            "RPetras.SpaceWeatherCLI.dll",
            "aurora");

        // Verify program failed
        Assert.AreNotEqual(0, exitCode);
        Assert.IsTrue(output.Contains("Invalid parameters"));
    }
}