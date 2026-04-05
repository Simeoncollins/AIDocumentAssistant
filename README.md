# Proxima AI Chat Application

Proxima is a modern, responsive, and friction-less Retrieval-Augmented Generation (RAG) chat application built with Blazor Interactive Server. It provides visitors with a premium, drop-in experience to upload documents and instantly converse with an intelligent AI aware of their custom context.

## Features

- **Anonymous Sessions**: Instantly tracks user chats and documents without requiring logins or signups utilizing secure HTTP-only cookies.
- **Drag-and-Drop Document Ingestion**: Upload `.txt`, `.pdf`, `.md`, and `.docx` files through an intuitive Drag-and-Drop responsive UI.
- **Multi-Conversation Persistence**: A built-in SQLite repository securely saves, categorizes, and restores unlimited distinct chat histories per session isolation block.
- **AI Auto-Titling**: Automatically generates concise summary titles for your active communication threads using lightweight, background LLM queries.
- **Responsive Mobile Flyout Sidebar**: Engineered to guarantee a pristine Desktop side-push UI that effortlessly converts to an absolute, touch-friendly Flyout on mobile displays.
- **Advanced RAG Engine**: System-prompted to prevent hallucination. Responses strictly rely on uploaded context utilizing intelligent Semantic Search vectors.

## Technology Stack

- **Framework**: C# 13 / .NET 9 ASP.NET Core Blazor (Interactive Server)
- **Database**: SQLite (`Microsoft.Data.Sqlite`)
- **Semantic Vector Store**: `Microsoft.Extensions.VectorData`
- **Document Processors**: `DocumentFormat.OpenXml` (Word), PDF/MD Readers
- **Styling**: Pure CSS (Dark Mode Glassmorphism & Desktop-Mobile Media Queries)

## Running the Application Locally

1. Clone the repository and navigate to the project directory.
2. Restore any necessary NuGet packages:
   ```bash
   dotnet restore
   ```
3. Build and execute the application:
   ```bash
   dotnet run
   ```
4. Navigate to the local URL (usually `http://localhost:5215`) to immediately start chatting with your uploaded documents.
