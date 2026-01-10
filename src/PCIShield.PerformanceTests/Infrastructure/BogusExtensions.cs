using System;
using Bogus;

namespace PCIShield.PerformanceTests.Infrastructure;

/// <summary>
/// Provides extension methods for the Bogus library to support more advanced data generation.
/// </summary>
public static class BogusExtensions
{
    // Method for Gaussian (Normal) distribution of numbers
    public static double Gaussian(this Randomizer randomizer, double mean, double stdDev)
    {
        double u1 = 1.0 - randomizer.Double();
        double u2 = 1.0 - randomizer.Double();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * randStdNormal;
    }

    public static int GaussianInt(this Randomizer randomizer, double mean, double stdDev, int min, int max)
    {
        var value = randomizer.Gaussian(mean, stdDev);
        var clampedValue = Math.Max(min, Math.Min(max, value));
        return (int)Math.Round(clampedValue);
    }

    /// <summary>
    /// Generates a realistic vendor code with a prefix and a numeric part.
    /// </summary>
    public static string MerchantCode(this Faker faker)
    {
        return $"VEN-{faker.Random.Number(10000, 99999)}";
    }

    /// <summary>
    /// Generates a realistic annual revenue based on the number of employees.
    /// </summary>
    public static decimal RealisticAnnualRevenue(this Faker faker, int numberOfEmployees)
    {
        // Assume revenue per employee is between $50k and $400k, with some randomness.
        var revenuePerEmployee = faker.Random.Decimal(50000, 400000);
        var baseRevenue = numberOfEmployees * revenuePerEmployee;
        // Add or subtract up to 20% to make it less linear.
        var fuzzyFactor = faker.Random.Decimal(0.8m, 1.2m);
        return Math.Round(baseRevenue * fuzzyFactor, 2);
    }

    /// <summary>
    /// Generates a random date that occurs between a start date and an end date.
    /// </summary>
    public static DateTime DateBetween(this Faker faker, DateTime start, DateTime end)
    {
       if (start >= end) return end; // Ensure start is before end
       return faker.Date.Between(start, end);
    }
}
