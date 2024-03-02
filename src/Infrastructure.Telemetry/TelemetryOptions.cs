namespace Infrastructure.Telemetry;

public class TelemetryOptions
{
    public const string SectionName = "Telemetry";

    public Uri ZipkinEndpoint { get; set; } = new ("http://localhost:9411/api/v2/spans");
}
