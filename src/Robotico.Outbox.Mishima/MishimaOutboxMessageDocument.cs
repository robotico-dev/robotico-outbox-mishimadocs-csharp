namespace Robotico.Outbox.Mishima;

/// <summary>
/// Wire shape for one outbox row stored as a Mishima JSON document.
/// </summary>
internal sealed class MishimaOutboxMessageDocument
{
    public string Id { get; set; } = "";

    public string MessageType { get; set; } = "";

    public string Payload { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; }
}
