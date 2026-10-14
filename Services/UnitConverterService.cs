namespace UnitConverterApi.Services;

public sealed class UnitConverterService : IUnitConverterService
{
    // Factors convert each unit to a common base unit: meters or kilograms.
    private static readonly Dictionary<string, double> LengthFactors =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["meters"] = 1,
            ["kilometers"] = 1000,
            ["centimeters"] = 0.01,
            ["millimeters"] = 0.001,
            ["feet"] = 0.3048,
            ["inches"] = 0.0254,
            ["miles"] = 1609.344
        };

    private static readonly Dictionary<string, double> MassFactors =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["kilograms"] = 1,
            ["grams"] = 0.001,
            ["milligrams"] = 0.000001,
            ["pounds"] = 0.45359237,
            ["ounces"] = 0.028349523125,
            ["tonnes"] = 1000
        };

    private static readonly HashSet<string> TemperatureUnits =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "celsius", "fahrenheit", "kelvin"
        };

    public ConversionResult Convert(double value, string fromUnit, string toUnit)
    {
        if (!double.IsFinite(value))
            throw new ArgumentException("Value must be a finite number.");

        if (string.IsNullOrWhiteSpace(fromUnit) || string.IsNullOrWhiteSpace(toUnit))
            throw new ArgumentException("FromUnit and ToUnit are required.");

        fromUnit = fromUnit.Trim();
        toUnit = toUnit.Trim();

        if (LengthFactors.TryGetValue(fromUnit, out var fromLength) &&
            LengthFactors.TryGetValue(toUnit, out var toLength))
        {
            return new ConversionResult(value * fromLength / toLength, "Length");
        }

        if (MassFactors.TryGetValue(fromUnit, out var fromMass) &&
            MassFactors.TryGetValue(toUnit, out var toMass))
        {
            return new ConversionResult(value * fromMass / toMass, "Weight");
        }

        if (TemperatureUnits.Contains(fromUnit) && TemperatureUnits.Contains(toUnit))
        {
            var celsius = fromUnit.ToLowerInvariant() switch
            {
                "celsius" => value,
                "fahrenheit" => (value - 32) * 5 / 9,
                "kelvin" => value - 273.15,
                _ => throw new ArgumentException("Unsupported temperature unit.")
            };

            var result = toUnit.ToLowerInvariant() switch
            {
                "celsius" => celsius,
                "fahrenheit" => celsius * 9 / 5 + 32,
                "kelvin" => celsius + 273.15,
                _ => throw new ArgumentException("Unsupported temperature unit.")
            };

            if (toUnit.Equals("kelvin", StringComparison.OrdinalIgnoreCase) && result < 0)
                throw new ArgumentException("Temperature cannot be below absolute zero.");

            return new ConversionResult(result, "Temperature");
        }

        throw new ArgumentException(
            "Unsupported units or units from different categories. Check the supported unit names in the README.");
    }
}
