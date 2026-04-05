using Microsoft.Extensions.DataIngestion;

namespace AIChatApp.WebApp.Services.Ingestion;

internal sealed class TxtReader : IngestionDocumentReader
{
    public override async Task<IngestionDocument> ReadAsync(Stream source, string identifier, string mediaType, CancellationToken cancellationToken = default)
    {
        var document = new IngestionDocument(identifier);
        var section = new IngestionDocumentSection { PageNumber = 1 };

        using var reader = new StreamReader(source);
        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                section.Elements.Add(new IngestionDocumentParagraph(line) { Text = line });
            }
        }

        document.Sections.Add(section);
        return document;
    }
}
