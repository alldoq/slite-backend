# Database Setup Guide

This project now includes PostgreSQL database support using Entity Framework Core. The database integration is designed to be cross-platform and scalable.

## Prerequisites

1. PostgreSQL server installed and running
2. .NET 8 SDK
3. AWS SAM CLI (for deployment)

## Local Development Setup

### 1. Install PostgreSQL

**Windows:**
```bash
# Using chocolatey
choco install postgresql

# Using installer from postgresql.org
# Download and run the installer
```

**macOS:**
```bash
# Using Homebrew
brew install postgresql
brew services start postgresql

# Using MacPorts
sudo port install postgresql16-server
```

**Linux (Ubuntu/Debian):**
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

### 2. Create Database

```bash
# Connect to PostgreSQL as superuser
sudo -u postgres psql

# Create database and user
CREATE DATABASE slitedb;
CREATE USER sliteuser WITH PASSWORD 'slitepassword';
GRANT ALL PRIVILEGES ON DATABASE slitedb TO sliteuser;
\q
```

### 3. Environment Variables

Set the following environment variables for local development:

```bash
export DB_HOST=localhost
export DB_PORT=5432
export DB_NAME=slitedb
export DB_USERNAME=sliteuser
export DB_PASSWORD=slitepassword
```

## Database Schema

The application includes the following entities:

### Users Table
- `id` (Primary Key, Auto-increment)
- `name` (Required, Max 100 characters)
- `email` (Required, Unique, Max 255 characters)
- `created_at` (Timestamp)
- `updated_at` (Timestamp)

### Companies Table
- `id` (Primary Key, Auto-increment)
- `name` (Required, Max 200 characters)
- `description` (Optional, Max 2000 characters)
- `email` (Required, Unique, Max 255 characters)
- `phone` (Optional, Max 20 characters)
- `website` (Optional, Max 500 characters)
- `address` (Optional, Max 500 characters)
- `city` (Optional, Max 100 characters)
- `country` (Optional, Max 100 characters)
- `is_active` (Boolean, Default: true)
- `founded_year` (Optional, Integer)
- `employee_count` (Optional, Integer)
- `created_at` (Timestamp)
- `updated_at` (Timestamp)

### Services Table
- `id` (Primary Key, Auto-increment)
- `name` (Required, Max 200 characters)
- `description` (Optional, Max 1000 characters)
- `price` (Decimal, Required)
- `company_id` (Foreign Key to Companies, Required)
- `duration_hours` (Optional, Integer)
- `category` (Optional, Max 100 characters)
- `is_active` (Boolean, Default: true)
- `created_at` (Timestamp)
- `updated_at` (Timestamp)

## API Endpoints

### GET /hello
Returns the original hello world message with location.

### Users Endpoints

#### GET /users
Returns all users from the database.

#### POST /users
Creates a new user. Required fields:
- `name` (string)
- `email` (string)

Example request body:
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com"
}
```

### Companies Endpoints

#### GET /companies
Returns all companies from the database.

#### POST /companies
Creates a new company. Required fields:
- `name` (string)
- `email` (string)

Example request body:
```json
{
  "name": "Tech Solutions Inc",
  "email": "contact@techsolutions.com",
  "description": "Leading provider of technology consulting services",
  "phone": "+1-555-0123",
  "website": "https://techsolutions.com",
  "address": "123 Business St",
  "city": "San Francisco",
  "country": "USA",
  "foundedYear": 2020,
  "employeeCount": 50
}
```

### Services Endpoints

#### GET /services
Returns all services from the database (includes company information).

#### POST /services
Creates a new service. Required fields:
- `name` (string)
- `companyId` (integer)
- `price` (decimal)

Example request body:
```json
{
  "name": "Cloud Migration Consulting",
  "description": "Complete cloud infrastructure migration service",
  "price": 5000.00,
  "companyId": 1,
  "durationHours": 40,
  "category": "Consulting",
  "isActive": true
}
```

#### GET /companies/{companyId}/services
Returns all services offered by a specific company.

## Deployment Configuration

### AWS SAM Parameters

When deploying with AWS SAM, you can configure database connection parameters:

```bash
sam deploy --parameter-overrides \
  DatabaseHost=your-db-host \
  DatabasePort=5432 \
  DatabaseName=slitedb \
  DatabaseUsername=your-username \
  DatabasePassword=your-password
```

### Production Considerations

1. **Security**: Use AWS Secrets Manager or Parameter Store for database credentials
2. **Scalability**: Consider using Amazon RDS PostgreSQL for production workloads
3. **Connection Pooling**: The current implementation uses Entity Framework's built-in connection pooling
4. **Monitoring**: Add CloudWatch metrics and logging for database operations

## Testing the Database Integration

1. Build the project:
```bash
dotnet build
```

2. Run locally using SAM:
```bash
sam local start-api
```

3. Test the endpoints:
```bash
# Test hello endpoint
curl http://localhost:3000/hello

# Test get users
curl http://localhost:3000/users

# Test create user
curl -X POST http://localhost:3000/users \
  -H "Content-Type: application/json" \
  -d '{"name":"John Doe","email":"john@example.com"}'

# Test get companies
curl http://localhost:3000/companies

# Test create company
curl -X POST http://localhost:3000/companies \
  -H "Content-Type: application/json" \
  -d '{"name":"Tech Solutions Inc","email":"contact@techsolutions.com"}'

# Test get services
curl http://localhost:3000/services

# Test create service (requires existing company)
curl -X POST http://localhost:3000/services \
  -H "Content-Type: application/json" \
  -d '{"name":"Cloud Migration","price":5000.00,"companyId":1}'

# Test get services by company
curl http://localhost:3000/companies/1/services
```

## Troubleshooting

### Connection Issues
- Verify PostgreSQL is running: `pg_isready`
- Check connection string format
- Ensure database exists and user has proper permissions

### Entity Framework Issues
- Database will be created automatically on first run
- Check logs for specific Entity Framework errors
- Verify all required packages are installed

### AWS Lambda Issues
- Ensure Lambda has network access to database
- Check environment variables are properly set
- Review CloudWatch logs for detailed error information