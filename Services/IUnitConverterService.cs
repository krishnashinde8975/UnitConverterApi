namespace UnitConverterApi.Services;

public interface IUnitConverterService
{
    ConversionResult Convert(double value, string fromUnit, string toUnit);
}

public sealed record ConversionResult(double Value, string Category);
