namespace Parkhaus.Services;

public static class PricingService
{
    public static decimal CalculateFee(DateTime entry, DateTime exit)
    {
        if (exit <= entry) return 0;
        var duration = exit - entry;
        // Regel: Jede angefangene Stunde kostet 2.00 CHF
        return (decimal)Math.Ceiling(duration.TotalHours) * 2.0m;
    }
}