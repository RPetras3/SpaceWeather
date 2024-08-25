namespace RPetras.SpaceWeatherLib.Tests;

[TestClass]
public class ProgramHelpTest
{
    [TestMethod]
    public void TestNoArgs()
    {
        // Run the command line application with no args
        var exitCode = Runner.Run(out var output, "dotnet", "RPetras.SpaceWeatherCLI.dll");

        // Verify program exited correctly
        Assert.AreEqual(0, exitCode);
        Assert.IsTrue(output.Contains("Usage:"));
    }

    [TestMethod]
    public void TestHelpShort()
    {
        // Run the command line application with short help arg
        var exitCode = Runner.Run(out var output, "dotnet", "RPetras.SpaceWeatherCLI.dll", "-h");

        // Verify program exited correctly
        Assert.AreEqual(0, exitCode);
        Assert.IsTrue(output.Contains("Usage:"));
    }

    [TestMethod]
    public void TestHelpLong()
    {
        // Run the command line application with long help arg
        var exitCode = Runner.Run(out var output, "dotnet", "RPetras.SpaceWeatherCLI.dll", "--help");

        // Verify program exited correctly
        Assert.AreEqual(0, exitCode);
        Assert.IsTrue(output.Contains("Usage:"));
    }
}