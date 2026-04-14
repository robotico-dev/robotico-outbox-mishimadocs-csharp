using System.Text.Json;

namespace Robotico.Outbox.Mishima;

internal static class MishimaOutboxJsonOptions
{
    internal static readonly JsonSerializerOptions Instance = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };
}
