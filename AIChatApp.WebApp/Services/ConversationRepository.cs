using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.AI;

namespace AIChatApp.WebApp.Services;

public class ConversationRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SessionId { get; set; } = string.Empty;
    public string Title { get; set; } = "New Conversation";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<ChatMessage> Messages { get; set; } = new();
}

public class ConversationRepository
{
    private readonly string _connectionString;

    public ConversationRepository()
    {
        _connectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, "user_conversations.db")}";
    }

    public async Task InitializeAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        // Drop the old schema since it only had SessionId and MessagesJson
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS UserConversations (
                Id TEXT PRIMARY KEY, 
                SessionId TEXT, 
                Title TEXT, 
                CreatedAt DATETIME, 
                MessagesJson TEXT
            )";
        await command.ExecuteNonQueryAsync();
        
        // Also support returning if it existed before so we don't drop everything arbitrarily, 
        // we'll just switch the table name to UserConversations to cleanly migrate.
    }

    public async Task SaveConversationAsync(ConversationRecord record)
    {
        var dtoList = record.Messages.Select(m => new ChatMessageDto(m.Role.Value, m.Text ?? "")).ToList();
        var json = JsonSerializer.Serialize(dtoList);

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO UserConversations (Id, SessionId, Title, CreatedAt, MessagesJson) 
            VALUES (@id, @sessionId, @title, @createdAt, @json) 
            ON CONFLICT(Id) DO UPDATE SET 
                Title=excluded.Title, 
                MessagesJson=excluded.MessagesJson";
        
        command.Parameters.AddWithValue("@id", record.Id);
        command.Parameters.AddWithValue("@sessionId", record.SessionId);
        command.Parameters.AddWithValue("@title", record.Title);
        command.Parameters.AddWithValue("@createdAt", record.CreatedAt.ToString("o"));
        command.Parameters.AddWithValue("@json", json);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<ConversationRecord>> GetConversationsBySessionIdAsync(string sessionId)
    {
        var results = new List<ConversationRecord>();
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, SessionId, Title, CreatedAt, MessagesJson FROM UserConversations WHERE SessionId = @id ORDER BY CreatedAt DESC";
        command.Parameters.AddWithValue("@id", sessionId);
        
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var id = reader.GetString(0);
            var sid = reader.GetString(1);
            var title = reader.GetString(2);
            var createdAt = DateTime.Parse(reader.GetString(3));
            var json = reader.GetString(4);
            
            var dtoList = JsonSerializer.Deserialize<List<ChatMessageDto>>(json) ?? new();
            var messages = dtoList.Select(d => new ChatMessage(new ChatRole(d.Role), d.Text)).ToList();
            
            results.Add(new ConversationRecord { Id = id, SessionId = sid, Title = title, CreatedAt = createdAt, Messages = messages });
        }
        return results;
    }

    public async Task DeleteConversationAsync(string id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM UserConversations WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
    }

    private record ChatMessageDto(string Role, string Text);
}
