using System.Text.Json;
using MishimaDocs;
using MishimaDocs.IO;
using Robotico.Result.Errors;
using Xunit;

namespace Robotico.Outbox.Mishima.Tests;

/// <summary>Tests for <see cref="MishimaOutbox"/>.</summary>
public sealed class MishimaOutboxTests
{
    private static IMishimaDatabase CreateDatabase()
    {
        string path = Path.Combine(Path.GetTempPath(), "robotico-mishima-outbox-" + Guid.NewGuid().ToString("N", null) + ".mishima");
        MishimaOpenOptions options = new() { DatabaseFilePath = path, CreateIfNotExists = true };
        return MishimaDatabaseFactory.OpenOrCreate(options, new PhysicalFileAccess());
    }

    [Fact]
    public async Task EnqueueAsync_without_batch_inserts_document()
    {
        using IMishimaDatabase db = CreateDatabase();
        string collectionName = "outbox_" + Guid.NewGuid().ToString("N", null);
        MishimaOutbox outbox = new MishimaOutbox(db, null, collectionName);
        OutboxTestMessage message = new OutboxTestMessage { Id = 42, Name = "test" };

        Robotico.Result.Result result = await outbox.EnqueueAsync(message);

        Assert.True(result.IsSuccess());
        IMishimaCollection col = db.GetCollection(collectionName);
        IReadOnlyList<string> ids = col.EnumerateDocumentIds();
        Assert.Single(ids);
        JsonElement? doc = col.TryGetById(ids[0]);
        Assert.NotNull(doc);
        string? payload = doc.Value.GetProperty("payload").GetString();
        Assert.NotNull(payload);
        Assert.Contains("42", payload, StringComparison.Ordinal);
    }

    [Fact]
    public async Task EnqueueAsync_with_batch_CommitAsync_persists()
    {
        using IMishimaDatabase db = CreateDatabase();
        string collectionName = "outbox_batch_" + Guid.NewGuid().ToString("N", null);
        using IMishimaWriteBatch batch = db.BeginWriteBatch();
        MishimaOutbox outbox = new MishimaOutbox(db, batch, collectionName);

        Robotico.Result.Result enqueue = await outbox.EnqueueAsync(new OutboxTestMessage { Id = 7, Name = "tx" });
        Assert.True(enqueue.IsSuccess());
        Robotico.Result.Result commit = await outbox.CommitAsync();
        Assert.True(commit.IsSuccess());

        IMishimaCollection col = db.GetCollection(collectionName);
        Assert.Single(col.EnumerateDocumentIds());
    }

    [Fact]
    public async Task CommitAsync_without_batch_returns_success()
    {
        using IMishimaDatabase db = CreateDatabase();
        MishimaOutbox outbox = new MishimaOutbox(db, null, "Outbox");

        Robotico.Result.Result result = await outbox.CommitAsync();

        Assert.True(result.IsSuccess());
    }

    [Fact]
    public async Task EnqueueAsync_throws_when_message_null()
    {
        using IMishimaDatabase db = CreateDatabase();
        MishimaOutbox outbox = new MishimaOutbox(db, null, "Outbox");

        await Assert.ThrowsAsync<ArgumentNullException>(() => outbox.EnqueueAsync(null!));
    }

    [Fact]
    public async Task EnqueueAsync_throws_when_canceled()
    {
        using IMishimaDatabase db = CreateDatabase();
        MishimaOutbox outbox = new MishimaOutbox(db, null, "Outbox");
        using (CancellationTokenSource cts = new CancellationTokenSource())
        {
            await cts.CancelAsync();

            await Assert.ThrowsAsync<OperationCanceledException>(() => outbox.EnqueueAsync(new object(), cts.Token));
        }
    }

    [Fact]
    public async Task CommitAsync_throws_when_canceled()
    {
        using IMishimaDatabase db = CreateDatabase();
        MishimaOutbox outbox = new MishimaOutbox(db, null, "Outbox");
        using (CancellationTokenSource cts = new CancellationTokenSource())
        {
            await cts.CancelAsync();

            await Assert.ThrowsAsync<OperationCanceledException>(() => outbox.CommitAsync(cts.Token));
        }
    }

    [Fact]
    public async Task EnqueueAsync_returns_error_when_serialization_fails()
    {
        using IMishimaDatabase db = CreateDatabase();
        MishimaOutbox outbox = new MishimaOutbox(db, null, "Outbox");
        CyclicOutboxMessage cyclic = new CyclicOutboxMessage();
        cyclic.Self = cyclic;

        Robotico.Result.Result result = await outbox.EnqueueAsync(cyclic);

        Assert.True(result.IsError(out IError? err));
        Assert.IsType<ExceptionError>(err);
    }
}
