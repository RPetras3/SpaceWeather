namespace RPetras.SpaceWeatherLib.Tests;

[TestClass]
public class ProgramArguments
{
    [TestMethod]
    public void TestBadArgument()
    {
        // Run the command line application with bad argument
        var exitCode = Runner.Run(out var output, "dotnet", "RPetras.SpaceWeatherCLI.dll", "invalid-parameter");

        // Verify program failed
        Assert.AreNotEqual(0, exitCode);
        Assert.IsTrue(output.Contains("Unknown Command:"));
    }
}