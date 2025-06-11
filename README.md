# PracticeApi

A .NET 9 Web API for managing and searching products, featuring SQL Server and Elasticsearch integration.

## Features

- **CRUD operations** for products (Create, Read, Update, Delete)
- **Search products** using:
  - Entity Framework (SQL Server)
  - Elasticsearch (full-text search)
- **Automatic product seeding** (200,000+ fake products on first run)
- **Response time middleware** for performance tracking
- **Swagger/OpenAPI** documentation

## Technologies

- ASP.NET Core (.NET 9)
- Entity Framework Core (SQL Server)
- Elasticsearch (via NEST)
- Bogus (data seeding)
- Swagger (API docs)

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Elasticsearch](https://www.elastic.co/downloads/elasticsearch) (default: `http://localhost:9200`)

### Configuration

Edit `PracticeApi/appsettings.json` to set your SQL Server connection string:
"ConnectionStrings": { "DefaultConnection": "Server=localhost;Database=SalesDB;User Id=sa;Password=abc@123;TrustServerCertificate=True;" }


### Running the API

1. Restore dependencies: `dotnet restore`
2. Build and run the project: `dotnet run --project PracticeApi`
3. The API will automatically:
- `Apply database migrations`
- `Seed the database with products (if empty)`
- `Sync products to Elasticsearch`

4. Access Swagger UI at: `https://localhost:5001/swagger`

## API Endpoints

- `POST /api/products/create` - Create a product
- `PUT /api/products/update/{id}` - Update a product
- `DELETE /api/products/delete/{id}` - Delete a product
- `GET /api/products/getbyid/{id}` - Get product by ID
- `GET /api/products/getall` - List all products
- `GET /api/products/searchef?query=...` - Search products (SQL)
- `GET /api/products/searches?query=...` - Search products (Elasticsearch)

## License

This project is provided for educational purposes.

Summary of content:
•	Describes the project, features, and tech stack.
•	Provides setup, configuration, and run instructions.
•	Lists main API endpoints.
•	Notes on licensing and usage.
Let me know if you want to add usage examples or further details.
