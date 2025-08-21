# Simple Expense Management API

This project uses Microsoft Restier to provide a RESTful API for expense management.

## Features

- **CRUD Operations**: Create, Read, Update, and Delete expenses
- **Category Management**: Organize expenses by categories
- **Summary Reports**: Get expense summaries and category breakdowns
- **Data Validation**: Input validation using Data Annotations
- **Entity Framework Core**: Data persistence with EF Core

## API Endpoints

### Expenses

- `GET /api/Expenses` - Get all expenses (ordered by date descending)
- `GET /api/Expenses/{id}` - Get a specific expense by ID
- `POST /api/Expenses` - Create a new expense
- `PUT /api/Expenses/{id}` - Update an existing expense
- `DELETE /api/Expenses/{id}` - Delete an expense

### Additional Endpoints

- `GET /api/Expenses/categories` - Get all unique categories
- `GET /api/Expenses/summary` - Get expense summary and category breakdown

### Restier Endpoints

Restier automatically provides additional endpoints:
- `GET /api/Expenses?$filter=...` - Filter expenses
- `GET /api/Expenses?$orderby=...` - Order expenses
- `GET /api/Expenses?$top=...` - Limit results
- `GET /api/Expenses?$skip=...` - Skip results for pagination

## Data Models

### Expense
```json
{
  "id": 1,
  "description": "Lunch at restaurant",
  "amount": 25.50,
  "date": "2024-01-15T00:00:00",
  "category": "Food",
  "notes": "Business lunch with client",
  "createdAt": "2024-01-15T10:00:00Z",
  "updatedAt": null
}
```

### CreateExpenseDto
```json
{
  "description": "Office supplies",
  "amount": 45.75,
  "date": "2024-01-15T00:00:00",
  "category": "Office",
  "notes": "Printer paper and ink"
}
```

### UpdateExpenseDto
```json
{
  "description": "Updated description",
  "amount": 50.00,
  "date": "2024-01-15T00:00:00",
  "category": "Office",
  "notes": "Updated notes"
}
```

## Database Configuration

The project is configured to use an in-memory database for development. To use SQL Server:

1. Update the connection string in `appsettings.json`
2. Uncomment the SQL Server configuration in `Program.cs`
3. Run Entity Framework migrations

## Getting Started

1. Build the project
2. Run the application
3. Navigate to `/swagger` to see the API documentation
4. Use the API endpoints to manage expenses

## Dependencies

- Microsoft.Restier.AspNetCore
- Microsoft.Restier.EntityFrameworkCore
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.InMemory
- Microsoft.EntityFrameworkCore.Tools
