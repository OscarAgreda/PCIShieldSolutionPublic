using NBomber.Contracts;
using NBomber.CSharp;

namespace PCIShield.PerformanceTests.Config;

public static class LoadSimulationProfiles
{
    public static LoadSimulation[] GetSmokeTestSimulations()
    {
        return new[]
        {
            Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))
        };
    }

    public static LoadSimulation[] GetLoadTestSimulations()
    {
        return new[]
        {
            Simulation.RampingInject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(2)),
            Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(5)),
            Simulation.RampingInject(rate: 0, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))
        };
    }

    public static LoadSimulation[] GetStressTestSimulations()
    {
        return new[]
        {
            Simulation.RampingConstant(copies: 200, during: TimeSpan.FromMinutes(3)),
            Simulation.KeepConstant(copies: 200, during: TimeSpan.FromMinutes(10)),
            Simulation.RampingConstant(copies: 0, during: TimeSpan.FromMinutes(2))
        };
    }

    public static LoadSimulation[] GetSoakTestSimulations()
    {
        return new[]
        {
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromHours(2))
        };
    }

    public static LoadSimulation[] GetIterationsTestSimulations()
    {
        return new[]
        {
            Simulation.IterationsForConstant(copies: 100, iterations: 1000)
        };
    }
}
