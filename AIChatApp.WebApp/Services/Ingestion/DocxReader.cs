using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DataIngestion;

namespace AIChatApp.WebApp.Services.Ingestion;

internal sealed class DocxReader : IngestionDocumentReader
{
    public override Task<IngestionDocument> ReadAsync(Stream source, string identifier, string mediaType, CancellationToken cancellationToken = default)
    {
        var document = new IngestionDocument(identifier);
        var section = new IngestionDocumentSection { PageNumber = 1 };

        using var wordDoc = WordprocessingDocument.Open(source, false);
        var body = wordDoc?.MainDocumentPart?.Document?.Body;
        if (body != null)
        {
            foreach (var para in body.Elements<Paragraph>())
            {
                var text = para.InnerText;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    section.Elements.Add(new IngestionDocumentParagraph(text) { Text = text });
                }
            }
        }
        
        document.Sections.Add(section);
        return Task.FromResult(document);
    }
}
