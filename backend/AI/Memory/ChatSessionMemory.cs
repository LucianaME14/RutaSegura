using System.Collections.Concurrent;

namespace RutaSegura.AI.Memory;

/// <summary>Memoria de conversación en proceso (últimos turnos por usuario).</summary>
public class ChatSessionMemory
{
    private readonly ConcurrentDictionary<int, List<ChatTurn>> _sessions = new();
    private const int MaxTurns = 12;

    public IReadOnlyList<ChatTurn> GetHistory(int userId)
    {
        return _sessions.TryGetValue(userId, out var list)
            ? list.AsReadOnly()
            : Array.Empty<ChatTurn>();
    }

    public void AddTurn(int userId, string userMessage, string botAnswer)
    {
        var list = _sessions.GetOrAdd(userId, _ => new List<ChatTurn>());
        lock (list)
        {
            list.Add(new ChatTurn(userMessage, botAnswer, DateTime.UtcNow));
            while (list.Count > MaxTurns)
                list.RemoveAt(0);
        }
    }

    public void Clear(int userId) => _sessions.TryRemove(userId, out _);
}

public record ChatTurn(string User, string Bot, DateTime At);
