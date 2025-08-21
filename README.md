# Simple Expense Management Application

A modern expense management application built with **ASP.NET Core 8** and **Angular**, featuring OData support for efficient data querying and a clean, responsive UI.

## 🚀 Quick Start

Get the application running in under 5 minutes!

### Prerequisites
- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download here](https://nodejs.org/)
- **SQL Server Express** (or LocalDB) - [Download here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## 🛠️ Setup & Run

### 1. Clone & Navigate
```bash
git clone <your-repo-url>
cd SimpleExpenseManagement
```

### 2. Start the Backend
```bash
dotnet run --project .\SimpleExpenseManagement.Server\
```

The backend will automatically:
- Create the database
- Seed initial data (People: Anton, Steve | Categories: Office, Home)
- Start on `http://localhost:5251`

### 3. Start the Frontend
```bash
# In a new terminal
cd simpleexpensemanagement.client
npm install
npm start
```

The Angular app will open automatically at `https://localhost:62574`

## ✨ What You Get

- **Expense Management**: Add, view, and delete expenses
- **Person Assignment**: Link expenses to Anton or Steve
- **Category Management**: Assign multiple categories per expense
- **OData Support**: Advanced querying with filtering, sorting, and pagination
- **Modern UI**: Clean, responsive interface with real-time validation

## 🔧 Database Configuration

The app automatically uses SQL Server Express. If you need to change this:

**For LocalDB** (Windows):
```json
// SimpleExpenseManagement.Server/appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SimpleExpenseManagement;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

**For SQL Server with different instance**:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\YOUR_INSTANCE;Database=SimpleExpenseManagement;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;MultipleActiveResultSets=true"
}
```

## 🌐 API Endpoints

### OData Endpoints
- **Expenses**: `/api/Expenses` - Full CRUD with OData querying
- **People**: `/api/People` - Person management
- **Categories**: `/api/Categories` - Category management

### Example Queries
```bash
# Get expensive items
GET /api/Expenses?$filter=amount gt 50

# Get recent expenses
GET /api/Expenses?$orderby=date desc&$top=10

# Get expenses with details
GET /api/Expenses?$expand=person,categories
```

## 🏗️ Project Structure

```
SimpleExpenseManagement/
├── SimpleExpenseManagement.Server/     # ASP.NET Core API
│   ├── Controllers/                    # OData controllers
│   ├── Models/                         # Data models
│   └── Data/                          # Entity Framework context
└── simpleexpensemanagement.client/     # Angular frontend
    ├── src/app/components/             # UI components
    └── src/app/services/               # API services
```

## 🐛 Troubleshooting

### Database Issues
- **SQL Server not running**: Start SQL Server service
- **Connection errors**: Check instance name in connection string
- **SSL errors**: Connection string includes `TrustServerCertificate=true`

### Frontend Issues
- **Port conflicts**: Change port in `angular.json` or kill process on 62574
- **Build errors**: Clear cache with `npm cache clean --force`

### Backend Issues
- **Port conflicts**: Change port in `launchSettings.json`
- **Database errors**: Check SQL Server is running and accessible

## 🚀 Development

### Backend Development
```bash
cd SimpleExpenseManagement.Server
dotnet watch run  # Auto-reload on changes
```

### Frontend Development
```bash
cd simpleexpensemanagement.client
npm run build    # Production build
npm test         # Run tests
```

## 🛠️ Technologies

- **Backend**: ASP.NET Core 8, Entity Framework Core, OData
- **Frontend**: Angular 19, TypeScript, CSS3
- **Database**: SQL Server Express/LocalDB
- **Architecture**: RESTful OData API with full CRUD operations

## 📝 License

This project is created for the Ocerra Practical Test.

---

**Need help?** Check the troubleshooting section above or open an issue!
