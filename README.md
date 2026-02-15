# books-history-api

A small ASP.NET Core Web API that manages books and records change history. Every update to a book (title, authors, description, publish date) produces timestamped history entries. The API supports pagination, filtering and ordering for history queries.

## Features
- Create, read and update books
- Track change history across all books/for a single book
- History queries with server-side pagination, filtering and sorting
- Swagger UI (OpenAPI)
- SQLite with EF Core migrations

## Endpoints

- POST /api/books : Create a book
- GET /api/books : Get all books (supports filtering/sorting/pagination via query params: titleOrDescription, author, publishedFrom, publishedTo, page, pageSize, orderBy, orderDir)
- GET /api/books/{id} : Get book details by id
- PATCH /api/books/{id} : Update a book (changes generate book events)

- GET /api/history/{bookId} : Get events for a specific book (query params: target, type, description, occuredFrom, occuredTo, page, pageSize, orderBy, orderDir)
- GET /api/history : Get events across all books (supports the same query params as GET /api/history/{bookId})

## Getting started

Prerequisites: .NET 8 SDK

1. Clone the repo `git clone https://github.com/Aglena/books-history-api.git`
2. Run the API
- `dotnet restore`
- `dotnet run`
3. On startup the API uses seeding to insert initial data into the SQLite database.
4. Open Swagger UI to explore and try endpoints
`https://localhost:<port>/swagger`
