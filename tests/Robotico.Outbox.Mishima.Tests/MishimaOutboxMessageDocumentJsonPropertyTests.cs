using System.Globalization;
using System.Text.Json;
using CsCheck;
using Robotico.Outbox.Mishima;
using Xunit;

namespace Robotico.Outbox.Mishima.Tests;

/// <summary>Property checks for wire JSON of <see cref="MishimaOutboxMessageDocument"/> using production serializer options.</summary>
public sealed class MishimaOutboxMessageDocumentJsonPropertyTests
{
    [Fact]
    public void Serialize_deserialize_round_trips_document_fields()
    {
        Gen.Int.Sample(static n =>
        {
            MishimaOutboxMessageDocument doc = new MishimaOutboxMessageDocument
            {
                Id = n.ToString(CultureInfo.InvariantCulture),
                MessageType = "Robotico.Tests.T" + n.ToString(CultureInfo.InvariantCulture),
                Payload = "{\"k\":" + n.ToString(CultureInfo.InvariantCulture) + "}",
                CreatedAtUtc = new DateTime(2020, 6, 15, 12, 30, 0, DateTimeKind.Utc),
            };
            string json = JsonSerializer.Serialize(doc, MishimaOutboxJsonOptions.Instance);
            MishimaOutboxMessageDocument? back =
                JsonSerializer.Deserialize<MishimaOutboxMessageDocument>(json, MishimaOutboxJsonOptions.Instance);
            return back != null
                && back.Id == doc.Id
                && back.MessageType == doc.MessageType
                && back.Payload == doc.Payload
                && back.CreatedAtUtc == doc.CreatedAtUtc;
        });
    }
}
