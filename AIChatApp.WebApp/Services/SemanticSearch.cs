using AIChatApp.WebApp.Services.Ingestion;
using Microsoft.Extensions.VectorData;

namespace AIChatApp.WebApp.Services;

public class SemanticSearch(
    VectorStore vectorStore,
    UserSessionContext sessionContext)
{
    public Task LoadDocumentsAsync() => Task.CompletedTask;

    public async Task<IReadOnlyList<IngestedChunk>> SearchAsync(string text, string? documentIdFilter, int maxResults)
    {
        var options = new VectorSearchOptions<IngestedChunk>
        {
            Filter = documentIdFilter is { Length: > 0 } ? record => record.DocumentId == documentIdFilter : null,
        };

        var combinedResults = new List<VectorSearchResult<IngestedChunk>>();
        string sessionId = sessionContext.SessionId;
        
        if (!string.IsNullOrEmpty(sessionId) && sessionId != "System")
        {
            var userCollection = vectorStore.GetCollection<string, IngestedChunk>($"{IngestedChunk.CollectionName}_{sessionId}");
            if (await userCollection.CollectionExistsAsync())
            {
                var userMatches = userCollection.SearchAsync(text, maxResults, options);
                await foreach (var match in userMatches)
                {
                    combinedResults.Add(match);
                }
            }
        }

        return combinedResults.OrderBy(r => r.Score).Take(maxResults).Select(r => r.Record).ToList();
    }
}
