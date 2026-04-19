using System.Text.Json;
using MishimaDocs;
using Robotico.Result.Errors;

namespace Robotico.Outbox.Mishima;

/// <summary>
/// MishimaDocs implementation of <see cref="Robotico.Outbox.IOutbox"/> storing one JSON document per enqueued message.
/// </summary>
/// <remarks>
/// <para>Without a write batch, each enqueue persists immediately via <see cref="IMishimaCollection.Insert"/>; <see cref="CommitAsync"/> returns success without additional work.</para>
/// <para>With a write batch from <c>IMishimaDatabase.BeginWriteBatch()</c>, enqueue records upserts on that batch; call <see cref="CommitAsync"/> once to persist atomically with other batch operations.</para>
/// </remarks>
public sealed class MishimaOutbox : Robotico.Outbox.IOutbox
{
    private readonly IMishimaDatabase _database;
    private readonly IMishimaWriteBatch? _writeBatch;
    private readonly string _collectionName;

    /// <summary>
    /// Initializes a new outbox bound to a Mishima database and optional write batch.
    /// </summary>
    /// <param name="database">Database handle.</param>
    /// <param name="writeBatch">Optional batch from <c>BeginWriteBatch()</c> on the same database.</param>
    /// <param name="collectionName">Target collection name (default <c>Outbox</c>).</param>
    public MishimaOutbox(IMishimaDatabase database, IMishimaWriteBatch? writeBatch = null, string collectionName = "Outbox")
    {
        ArgumentNullException.ThrowIfNull(database);
        ArgumentNullException.ThrowIfNull(collectionName);
        if (collectionName.Length == 0)
        {
            throw new ArgumentException("Collection name must not be empty.", nameof(collectionName));
        }

        _database = database;
        _writeBatch = writeBatch;
        _collectionName = collectionName;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
    public Task<Robotico.Result.Result> EnqueueAsync(object message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            string payload = JsonSerializer.Serialize(message, message.GetType(), MishimaOutboxJsonOptions.Instance);
            string messageType = message.GetType().AssemblyQualifiedName ?? message.GetType().FullName ?? "Unknown";
            string documentId = Guid.NewGuid().ToString("N", null);
            MishimaOutboxMessageDocument doc = new()
            {
                Id = documentId,
                MessageType = messageType,
                Payload = payload,
                CreatedAtUtc = DateTime.UtcNow,
            };
            JsonElement element = JsonSerializer.SerializeToElement(doc, MishimaOutboxJsonOptions.Instance);
            if (_writeBatch is null)
            {
                IMishimaCollection collection = _database.GetCollection(_collectionName);
                collection.Insert(documentId, element);
            }
            else
            {
                _writeBatch.Upsert(_collectionName, documentId, element);
            }

            return Task.FromResult(Robotico.Result.Result.Success());
        }
        catch (MishimaPersistenceException ex)
        {
            return Task.FromResult(Robotico.Result.Result.Error(new ExceptionError(ex)));
        }
        catch (JsonException ex)
        {
            return Task.FromResult(Robotico.Result.Result.Error(new ExceptionError(ex)));
        }
    }

    /// <inheritdoc />
    public Task<Robotico.Result.Result> CommitAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_writeBatch is null)
        {
            return Task.FromResult(Robotico.Result.Result.Success());
        }

        try
        {
            _writeBatch.Commit();
            return Task.FromResult(Robotico.Result.Result.Success());
        }
        catch (MishimaPersistenceException ex)
        {
            return Task.FromResult(Robotico.Result.Result.Error(new ExceptionError(ex)));
        }
    }
}
